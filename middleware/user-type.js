export default defineNuxtRouteMiddleware((to) => {
  const { userType } = useAuth()

  if (!['individual', 'entity'].includes(userType.value || '')) {
    return navigateTo('/dashboard')
  }
})
