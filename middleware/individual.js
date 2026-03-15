export default defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore();

  if (userStore.userType != "individual") {
    return false;
  }
});
