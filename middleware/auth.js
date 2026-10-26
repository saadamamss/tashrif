export default defineNuxtRouteMiddleware((to) => {
  const { isAuthenticated, user } = useAuth()

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  const isGuest = to.meta.guest === true

  if (requiresAuth && !isAuthenticated.value) {
    return navigateTo({ path: "/", query: { redirect: to.fullPath } })
  }

  // F4 — email-verification gate (D2/D6): an authenticated-but-unverified user
  // must be looking at /verify-email, not app pages. Strict `=== false` so
  // seed/mock users predating the flag are never bounced; skip the target
  // page itself to avoid a redirect loop.
  if (
    requiresAuth &&
    isAuthenticated.value &&
    user.value?.isEmailVerified === false &&
    to.path !== "/verify-email"
  ) {
    return navigateTo("/verify-email")
  }

  if (isGuest && isAuthenticated.value) {
    return navigateTo("/dashboard")
  }
})

