import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({
    userType: null,
    userData: null,
  }),

  getters: {
    defaultRoute: (state) => {
      switch (state.userType) {
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
      ["admin", "super-admin"].includes(state.userType || ""),
  },

  actions: {
    async fetchUserProfile() {
      try {
        // const { data } = await useFetch("/api/user/profile");
        // this.userType = data.value?.type;
        // this.userData = data.value;
        this.userType = "individual";
        this.userData = { id: 1, name: "abdullah" };
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
      }
    },
    setUser(userData) {
      this.userData = userData;
      this.userType = userData.type;
    },
  },
});
