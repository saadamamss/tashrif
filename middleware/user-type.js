export default defineNuxtRouteMiddleware((to) => {
  const { userType } = useAuth()

  // Admin passes through — admin's /dashboard IS the stats home (D8).
  if (!['individual', 'entity', 'admin'].includes(userType.value || '')) {
    return navigateTo('/dashboard')
  }
})
