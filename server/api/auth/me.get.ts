export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const user = users.find(u => u.id === session.userId)

  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  return {
    id: user.id,
    name: user.name,
    email: user.email,
    type: user.type,
    phone: user.phone,
    nationalId: user.nationalId,
    gender: user.gender,
    nationality: user.nationality,
  }
})
