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

  const token = generateToken(user.id, user.type)
  const refreshToken = generateToken(user.id, user.type)

  return {
    user: {
      id: user.id,
      name: user.name,
      email: user.email,
      type: user.type,
      phone: user.phone,
      nationalId: user.nationalId,
      gender: user.gender,
      nationality: user.nationality,
    },
    token,
    refreshToken,
  }
})
