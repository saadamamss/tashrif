export default defineEventHandler(async (event) => {
  await delay()

  const refreshToken = getCookie(event, 'refresh_token')

  if (!refreshToken || !tokens[refreshToken]) {
    throw createError({ statusCode: 401, statusMessage: 'Refresh token invalid or expired' })
  }

  const session = tokens[refreshToken]
  const newAccessToken = generateToken(session.userId, session.type)
  const newRefreshToken = generateToken(session.userId, session.type)

  setCookie(event, 'access_token', newAccessToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 2,
  })

  setCookie(event, 'refresh_token', newRefreshToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 24 * 7,
  })

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
