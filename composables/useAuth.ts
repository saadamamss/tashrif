import type { User } from "~/types/auth";

export function useAuth() {
  const user = useState<User | null>("auth:user", () => null);
  const isLoading = useState<boolean>("auth:isLoading", () => false);
  const isInitialized = useState<boolean>("auth:isInitialized", () => false);
  const error = useState<string | null>("auth:error", () => null);

  const isAuthenticated = computed(() => !!user.value);
  const userType = computed(() => user.value?.type || null);
  const isIndividual = computed(() => user.value?.type === "individual");
  const isEntity = computed(() => user.value?.type === "entity");
  const isAdmin = computed(() => user.value?.type === "admin");
  const isEmailVerified = computed(() => user.value?.isEmailVerified || false);

  function setUser(u: User | null) {
    user.value = u;
  }

  function clearUser() {
    user.value = null;
    error.value = null;
  }

  function api() {
    return useNuxtApp().$api;
  }

  async function init() {
    try {
      const { data } = await api().get<User>("/auth/me");
      setUser(data);
    } catch (err: any) {
      clearUser();
    } finally {
      isInitialized.value = true;
    }
  }

  async function login(credentials: { nationalId: string; password: string }) {
    isLoading.value = true;
    error.value = null;
    try {
      const { data } = await api().post<User>("/auth/login", credentials);
      setUser(data);
      isInitialized.value = true;
      return { success: true, requiresVerification: false };
    } catch (err: any) {
      const status = err?.response?.status;
      // 403 = account exists but email not verified → not a credentials error
      if (status === 403) {
        error.value = err?.response?.data?.message || "يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول";
        return { success: false, requiresVerification: true };
      }
      error.value =
        err?.response?.data?.message || err?.message || "Login failed";
      return { success: false, requiresVerification: false };
    } finally {
      isLoading.value = false;
    }
  }

  async function register(userData: any) {
    isLoading.value = true;
    error.value = null;
    try {
      // Registration is sessionless: backend returns 201 { message } only,
      // no tokens, no user, no auto-login. Caller routes to check-inbox.
      await api().post("/auth/register", userData);
      isInitialized.value = true;
      return { success: true, requiresVerification: true };
    } catch (err: any) {
      error.value =
        err?.response?.data?.message || err?.message || "Registration failed";
      return { success: false, requiresVerification: false };
    } finally {
      isLoading.value = false;
    }
  }

  async function verifyEmail(token: string) {
    try {
      const { data } = await api().get<User>("/auth/verify-email", {
        params: { token },
      });
      setUser(data);
      isInitialized.value = true;
      return { success: true };
    } catch (err: any) {
      error.value =
        err?.response?.data?.message || err?.message || "فشل تفعيل البريد الإلكتروني";
      return { success: false };
    }
  }

  // D8/B-1 fix (2026-10-22 review): resend is keyed by the NATIONAL ID — the same
  // credential the user logs in with — never by email. The backend's
  // ResendVerificationDto validates `nationalId` ([Required]) and any other payload
  // is rejected with 400 before reaching the service.
  async function resendVerification(nationalId: string) {
    try {
      await api().post("/auth/resend-verification", { nationalId });
      return { success: true };
    } catch (err: any) {
      error.value =
        err?.response?.data?.message || err?.message || "فشل إعادة إرسال رابط التفعيل";
      return { success: false };
    }
  }

  async function logout() {
    try {
      await api().post("/auth/logout");
    } catch {
      /* even if server logout fails, clear local state */
    }
    clearUser();
    isInitialized.value = true;
    navigateTo("/");
  }

  async function changePassword(currentPassword: string, newPassword: string) {
    try {
      const { data } = await api().put<User>("/auth/change-password", {
        currentPassword,
        newPassword,
      });
      setUser(data);
      return { error: null };
    } catch (err: any) {
      const message =
        err?.response?.data?.message || err?.message || "Failed to change password";
      return { error: message };
    }
  }

  return {
    user: readonly(user),
    isLoading: readonly(isLoading),
    isInitialized: readonly(isInitialized),
    error,
    isAuthenticated,
    userType,
    isIndividual,
    isEntity,
    isAdmin,
    isEmailVerified,
    setUser,
    clearUser,
    init,
    login,
    register,
    verifyEmail,
    resendVerification,
    logout,
    changePassword,
  };
}
