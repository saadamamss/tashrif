export default defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore();

  if (!["individual", "entity"].includes(userStore.userType)) {
    return false;
  }
});
