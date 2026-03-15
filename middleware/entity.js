export default defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore();

  if (userStore.userType != "entity") {
    return false;
  }
});
