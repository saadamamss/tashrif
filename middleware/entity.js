import { useAuthStore } from "~/stores/authStore";
export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore();

  if (authStore.userType !== "entity") {
    return navigateTo("/dashboard");
  }
});
