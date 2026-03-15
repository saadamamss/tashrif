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
  },

  actions: {
    async initialize() {
      const tokenuser = useCookie("user");
      if (process.server) {
        if (tokenuser.value) {
          this.token = "12:jska8fg5f8g8g2f5d8sds5556s9ds8d7sds2d2d2s";
          this.isAuthenticated = true;
        }
      }
      if (process.client) {
        const token = useCookie("auth:token");
        const refreshToken = useCookie("auth:refreshToken");

        if (token.value && refreshToken.value) {
          this.token = token.value;
          this.refreshToken = refreshToken.value;
          await this.refresh();
        }
      }
    },

    async login(credentials) {
      this.isLoading = true;
      this.error = null;

      try {
        // const { data, error } = await useFetch("/api/auth/login", {
        //   method: "POST",
        //   body: credentials,
        // });

        // if (error.value) {
        //   throw error.value;
        // }
        const data = {
          value: {
            id: 1,
            name: "abdullah",
            email: "abdullah@gmail.com",
            type: credentials.includes(1) ? "entity" : "individual",
            token: "12:ysh88fdasaiifd5dklsa2fdasoaklfdf9dks5",
          },
        };

        if (data.value) {
          this.setAuth(data.value);
          return true;
        }
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
        const { data, error } = await useFetch("/api/auth/register", {
          method: "POST",
          body: userData,
        });

        if (error.value) {
          throw error.value;
        }

        if (data.value) {
          this.setAuth(data.value);
          return true;
        }
      } catch (err) {
        this.error = err.message || "Registration failed";
        return false;
      } finally {
        this.isLoading = false;
      }
    },

    setAuth(authData) {
      const tokenCookie = useCookie("user", {
        maxAge: 60 * 60 * 24 * 7, // 1 week
      });
      // const refreshTokenCookie = useCookie("auth:refreshToken", {
      //   maxAge: 60 * 60 * 24 * 30, // 1 month
      // });

      this.user = authData;
      this.token = authData.token;
      // this.refreshToken = authData.refreshToken;
      this.isAuthenticated = true;

      tokenCookie.value = JSON.stringify(authData);
      // refreshTokenCookie.value = authData.refreshToken;
    },

    async refresh() {
      if (!this.refreshToken) return false;

      try {
        const { data } = await useFetch("/api/auth/refresh", {
          method: "POST",
          body: { refreshToken: this.refreshToken },
        });

        if (data.value) {
          this.setAuth(data.value);
          return true;
        }
      } catch (err) {
        this.logout();
        return false;
      }
    },

    async logout() {
      const tokenUser = useCookie("user");

      this.$reset();
      tokenUser.value = null;
      navigateTo("/");
      return;
      const tokenCookie = useCookie("auth:token");
      const refreshTokenCookie = useCookie("auth:refreshToken");

      try {
        await useFetch("/api/auth/logout", {
          method: "POST",
          headers: {
            Authorization: `Bearer ${this.token}`,
          },
        });
      } finally {
        this.$reset();
        tokenCookie.value = null;
        refreshTokenCookie.value = null;
        navigateTo("/login");
      }
    },

    async fetchUser() {
      if (!this.token) return;

      try {
        // const { data } = await useFetch("/api/auth/me", {
        //   headers: {
        //     Authorization: `Bearer ${this.token}`,
        //   },
        // });
        const userdataCookie = useCookie("user");
        const data = { value: userdataCookie.value };
        if (data.value) {
          this.user = data.value;
          return true;
        }
      } catch (err) {
        this.logout();
        return false;
      }
    },
  },
});
