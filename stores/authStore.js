import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    token: null,
    isAuthenticated: false,
    refreshToken: null,
    isLoading: false,
    error: null,
  }),

  getters: {
    currentUser: (state) => state.user,
    authToken: (state) => state.token,
    isAdmin: (state) => state.user?.type === "admin",
    isIndividual: (state) => state.user?.type === "individual",
    isEntity: (state) => state.user?.type === "entity",
    userType: (state) => state.user?.type || null,
    defaultRoute: (state) => {
      switch (state.user?.type) {
        case "individual":
          return "/individual-dashboard";
        case "entity":
          return "/entity-dashboard";
        case "admin":
          return "/admin/dashboard";
        default:
          return "/";
      }
    },
    canAccessBothDashboards: (state) =>
      ["admin", "super-admin"].includes(state.user?.type || ""),
  },

  actions: {
    async initialize() {
      if (process.client) {
        const tokenCookie = useCookie("auth:token");
        const refreshCookie = useCookie("auth:refreshToken");

        if (tokenCookie.value && refreshCookie.value) {
          this.token = tokenCookie.value;
          this.refreshToken = refreshCookie.value;
          await this.fetchUser();
        }
      }
    },

    async login(credentials) {
      this.isLoading = true;
      this.error = null;

      try {
        const { data, error } = await useApi().post("/auth/login", credentials);

        if (error) {
          this.error = error;
          return false;
        }

        this.setAuth(data);
        return true;
      } catch (err) {
        this.error = err.message || "Login failed";
        return false;
      } finally {
        this.isLoading = false;
      }
    },

    async register(userData) {
      this.isLoading = true;
      this.error = null;

      try {
        const { data, error } = await useApi().post("/auth/register", userData);

        if (error) {
          this.error = error;
          return false;
        }

        this.setAuth(data);
        return true;
      } catch (err) {
        this.error = err.message || "Registration failed";
        return false;
      } finally {
        this.isLoading = false;
      }
    },

    setAuth(authData) {
      this.user = authData.user;
      this.token = authData.token;
      this.refreshToken = authData.refreshToken;
      this.isAuthenticated = true;

      const tokenCookie = useCookie("auth:token", {
        maxAge: 60 * 60 * 24 * 7,
        sameSite: "lax",
        secure: process.env.NODE_ENV === "production",
      });
      const refreshCookie = useCookie("auth:refreshToken", {
        maxAge: 60 * 60 * 24 * 7,
        sameSite: "lax",
        secure: process.env.NODE_ENV === "production",
      });

      tokenCookie.value = authData.token;
      refreshCookie.value = authData.refreshToken;
    },

    async refresh() {
      if (!this.refreshToken) return false;

      try {
        const { data, error } = await useApi().post("/auth/refresh", {
          refreshToken: this.refreshToken,
        });

        if (error) {
          this.logout();
          return false;
        }

        this.token = data.token;
        this.refreshToken = data.refreshToken;

        const tokenCookie = useCookie("auth:token");
        const refreshCookie = useCookie("auth:refreshToken");
        tokenCookie.value = data.token;
        refreshCookie.value = data.refreshToken;

        return true;
      } catch (err) {
        this.logout();
        return false;
      }
    },

    async logout() {
      try {
        await useApi().post("/auth/logout");
      } catch {
        // ignore logout errors
      }

      const tokenCookie = useCookie("auth:token");
      const refreshCookie = useCookie("auth:refreshToken");
      tokenCookie.value = null;
      refreshCookie.value = null;

      this.$reset();
      navigateTo("/");
    },

    async fetchUser() {
      if (!this.token) return;

      try {
        const { data, error } = await useApi().get("/auth/me");

        if (error) {
          this.logout();
          return false;
        }

        this.user = data;
        this.isAuthenticated = true;
        return true;
      } catch (err) {
        this.logout();
        return false;
      }
    },
  },
});
