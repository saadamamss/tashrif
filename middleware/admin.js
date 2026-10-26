export default defineNuxtRouteMiddleware((to) => {
  const { userType } = useAuth()

  if (userType.value !== 'admin') {
    return navigateTo('/dashboard')
  }
})
