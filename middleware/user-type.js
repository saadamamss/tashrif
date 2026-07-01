import { useAuthStore } from "~/stores/authStore";

/**
 * @param {import('vue-router').RouteLocationNormalized} to
 */
export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore();

  if (!["individual", "entity"].includes(authStore.userType)) {
    return navigateTo("/dashboard");
  }
});
