export default defineEventHandler(async (event) => {
  await delay()
  const body = await readBody(event)
  const { nationalId, password } = body || {}

  if (!nationalId || !password) {
    throw createError({ statusCode: 400, statusMessage: 'البريد الإلكتروني وكلمة المرور مطلوبان' })
  }

  const user = users.find(u => u.nationalId === nationalId)
  if (!user || user.password !== hashPassword(password)) {
    throw createError({ statusCode: 401, statusMessage: 'بيانات الدخول غير صحيحة' })
  }

  const accessToken = generateToken(user.id, user.type)
  const refreshToken = generateToken(user.id, user.type)

  setCookie(event, 'access_token', accessToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 2,
  })

  setCookie(event, 'refresh_token', refreshToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 24 * 7,
  })

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
