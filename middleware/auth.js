export default defineNuxtRouteMiddleware((to) => {
  const { isAuthenticated } = useAuth()

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  const isGuest = to.meta.guest === true

  if (requiresAuth && !isAuthenticated.value) {
    return navigateTo({ path: "/", query: { redirect: to.fullPath } })
  }

  if (isGuest && isAuthenticated.value) {
    return navigateTo("/dashboard")
  }
})
