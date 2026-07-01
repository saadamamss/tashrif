export default defineEventHandler(async (event) => {
  await delay()
  const body = await readBody(event)

  if (!body || !body.refreshToken || !tokens[body.refreshToken]) {
    throw createError({ statusCode: 401, statusMessage: 'Refresh token invalid or expired' })
  }

  const session = tokens[body.refreshToken]
  const token = generateToken(session.userId, session.type)
  const refreshToken = generateToken(session.userId, session.type)

  return { token, refreshToken }
})
