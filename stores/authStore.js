import { defineStore } from "pinia";

/**
 * @typedef {import('~/types/auth').User} User
 * @typedef {import('~/types/auth').LoginCredentials} LoginCredentials
 * @typedef {import('~/types/auth').RegisterData} RegisterData
 * @typedef {import('~/types/auth').AuthResponse} AuthResponse
 */

export const useAuthStore = defineStore("auth", {
  state: () => ({
    /** @type {User | null} */
    user: null,
    /** @type {string | null} */
    token: null,
    /** @type {boolean} */
    isAuthenticated: false,
    /** @type {string | null} */
    refreshToken: null,
    /** @type {boolean} */
    isLoading: false,
    /** @type {string | null} */
    error: null,
  }),

  getters: {
    /** @returns {User | null} */
    currentUser: (state) => state.user,
    /** @returns {string | null} */
    authToken: (state) => state.token,
    /** @returns {boolean} */
    isAdmin: (state) => state.user?.type === "admin",
    /** @returns {boolean} */
    isIndividual: (state) => state.user?.type === "individual",
    /** @returns {boolean} */
    isEntity: (state) => state.user?.type === "entity",
    /** @returns {string | null} */
    userType: (state) => state.user?.type || null,
    /** @returns {string} */
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
    /** @returns {boolean} */
    canAccessBothDashboards: (state) =>
      ["admin", "super-admin"].includes(state.user?.type || ""),
  },

  actions: {
    /** @returns {Promise<void>} */
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

    /**
     * @param {LoginCredentials} credentials
     * @returns {Promise<boolean>}
     */
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

    /**
     * @param {RegisterData} userData
     * @returns {Promise<boolean>}
     */
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

    /**
     * @param {AuthResponse} authData
     */
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

    /** @returns {Promise<boolean>} */
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

    /** @returns {Promise<void>} */
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

    /** @returns {Promise<boolean>} */
    async fetchUser() {
      if (!this.token) return false;

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
