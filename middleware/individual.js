export default defineNuxtRouteMiddleware((to) => {
  const { userType } = useAuth()

  if (userType.value !== 'individual') {
    return navigateTo('/dashboard')
  }
})
