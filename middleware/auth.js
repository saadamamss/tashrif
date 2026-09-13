export default defineNuxtRouteMiddleware((to) => {
  const { isAuthenticated, mustChangePassword } = useAuth()

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  const isGuest = to.meta.guest === true

  if (requiresAuth && !isAuthenticated.value) {
    return navigateTo({ path: "/", query: { redirect: to.fullPath } })
  }

  if (isGuest && isAuthenticated.value) {
    return navigateTo("/dashboard")
  }

  // Force password change before accessing dashboard
  if (requiresAuth && isAuthenticated.value && mustChangePassword.value) {
    // Allow the change-password page itself and logout
    if (to.path !== "/dashboard/change-password" && to.path !== "/") {
      return navigateTo("/dashboard/change-password")
    }
  }
})
