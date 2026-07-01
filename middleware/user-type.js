import { useAuthStore } from "~/stores/authStore";
export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore();

  if (!["individual", "entity"].includes(authStore.userType)) {
    return navigateTo("/dashboard");
  }
});
