// plugins/axios.ts
import axios from "axios";
import { appendResponseHeader } from "h3";

const AUTH_ENDPOINTS = [
  "/auth/login",
  "/auth/logout",
  "/auth/register",
  "/auth/refresh",
];

function isAuthEndpoint(url: string = ""): boolean {
  return AUTH_ENDPOINTS.some((e) => url.includes(e));
}

function getCsrfToken(): string | null {
  if (import.meta.server) return null;
  const match = document.cookie.match(/csrf_token=([^;]+)/);
  return match ? match[1] : null;
}

const CSRF_HEADER = "X-CSRF-Token";

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig();
  const baseURL = config.public.apiBaseUrl || "/api";

  const ssrCookie = import.meta.server
    ? useRequestHeaders(["cookie"]).cookie
    : undefined;

  const ssrEvent = import.meta.server ? useRequestEvent() : null;

  let isRefreshing = false;
  let failedQueue: Array<{
    resolve: (value?: unknown) => void;
    reject: (reason?: unknown) => void;
  }> = [];

  const processQueue = (error: unknown) => {
    failedQueue.forEach((prom) => {
      if (error) prom.reject(error);
      else prom.resolve();
    });
    failedQueue = [];
  };

  if (baseURL === "/api") {
    console.warn(
      "[Tashrif] Running against mock API (baseURL=/api). " +
      "Set NUXT_PUBLIC_API_BASE_URL=http://localhost:5001/api in .env to use the real backend."
    );
  }

  const api = axios.create({
    baseURL,
    withCredentials: true,
    timeout: 30000,
    headers: { Accept: "application/json" },
  });

  // Request Interceptor
  api.interceptors.request.use((cfg) => {
    if (import.meta.server && ssrCookie && !cfg._retry) {
      cfg.headers.set("cookie", ssrCookie);
    }
    // Attach CSRF token on state-changing requests
    const method = cfg.method?.toUpperCase();
    if (method === "POST" || method === "PUT" || method === "DELETE" || method === "PATCH") {
      const csrfToken = getCsrfToken();
      if (csrfToken) {
        cfg.headers.set(CSRF_HEADER, csrfToken);
      }
    }
    return cfg;
  });

  // Response Interceptor
  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config;

      if (
        !originalRequest ||
        error.response?.status !== 401 ||
        originalRequest._retry ||
        isAuthEndpoint(originalRequest.url)
      ) {
        return Promise.reject(error);
      }

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(() => api(originalRequest));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const response = await axios.post(`${baseURL}/auth/refresh`, null, {
          withCredentials: true,
          headers: import.meta.server && ssrCookie ? { cookie: ssrCookie } : {},
        });        if (import.meta.server && ssrEvent) {
          const rawSetCookie = response.headers["set-cookie"];
          if (rawSetCookie) {
            const cookies = Array.isArray(rawSetCookie)
              ? rawSetCookie
              : [rawSetCookie];

            cookies.forEach((cookieStr) => {
              appendResponseHeader(ssrEvent, "set-cookie", cookieStr);
            });

            const newCookieString = cookies
              .map((c) => c.split(";")[0])
              .join("; ");

            originalRequest.headers.set("cookie", newCookieString);
          }
        }

        processQueue(null);

        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError);
        nuxtApp.runWithContext(() => {
          const { clearUser } = useAuth();
          clearUser();
        });
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    },
  );

  return { provide: { api } };
});
