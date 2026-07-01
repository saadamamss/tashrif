import { useAuthStore } from "~/stores/authStore";

/**
 * @param {import('vue-router').RouteLocationNormalized} to
 */
export default defineNuxtRouteMiddleware(async (to) => {
  const authStore = useAuthStore();

  await authStore.initialize();

  if (authStore.isAuthenticated && !authStore.currentUser) {
    await authStore.fetchUser();
  }
});
