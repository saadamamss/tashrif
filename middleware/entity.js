export default defineNuxtRouteMiddleware((to) => {
  const { userType } = useAuth()

  if (userType.value !== 'entity') {
    return navigateTo('/dashboard')
  }
})
