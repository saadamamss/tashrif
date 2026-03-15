import { useAuthStore } from "~/stores/authStore";

export default defineNuxtRouteMiddleware(async (to) => {
  const authStore = useAuthStore();

  if (!authStore.isAuthenticated) {
    if (to.query.from) {
      // delete to.fullPath.from;
      const from = to.query.from;
      delete to.query.from;
      const query = new URLSearchParams(to.query).toString();
      const fullPath = query ? to.path + "?" + query : to.path;

      return navigateTo(from + "?redirect=" + encodeURIComponent(fullPath));
    }
    return navigateTo("/?redirect=" + encodeURIComponent(to.fullPath));
  }
});
