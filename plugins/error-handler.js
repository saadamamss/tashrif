export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.config.errorHandler = (error, context) => {
    if (process.client) {
      if (error.statusCode === 401) {
        // Store the original path for redirect after login
        // const route = useRoute();
        // useCookie("auth_redirect").value = route.fullPath;

        // // Redirect to login with custom handling
        // return navigateTo({
        //   path: "/login",
        //   query: { unauthorized: "true" },
        // });
      }
    }
  };
});
