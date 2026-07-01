export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'individual') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const body = await readBody(event)
  const user = users.find(u => u.id === session.userId)

  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  if (body.name) user.name = body.name
  if (body.email) user.email = body.email
  if (body.phone) user.phone = body.phone
  if (body.gender) user.gender = body.gender

  return { id: user.id, name: user.name, email: user.email, phone: user.phone, type: user.type }
})
