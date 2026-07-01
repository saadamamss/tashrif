import { useAuthStore } from "~/stores/authStore";

/**
 * @param {import('vue-router').RouteLocationNormalized} to
 */
export default defineNuxtRouteMiddleware(async (to) => {
  const authStore = useAuthStore();

  await authStore.initialize();
  // If user data not loaded, fetch it
  if (authStore.isAuthenticated) {
    return navigateTo("/");
  }
});
