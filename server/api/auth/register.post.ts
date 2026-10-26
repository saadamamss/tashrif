import type { User } from '~~/server/utils/db'
export default defineEventHandler(async (event) => {
  await delay()
  const body = await readBody(event)

  if (!body || !body.nationalId || !body.password) {
    throw createError({ statusCode: 400, statusMessage: 'بيانات التسجيل غير كاملة' })
  }

  const existing = users.find(u => u.nationalId === body.nationalId)
  if (existing) {
    throw createError({ statusCode: 409, statusMessage: 'رقم الهوية مسجل مسبقاً' })
  }

  const newUser: User = {
    id: users.length + 1,
    nationalId: body.nationalId,
    password: hashPassword(body.password),
    name: body.name || '',
    email: body.email || '',
    phone: body.phone || '',
    type: body.type || 'individual',
    gender: body.gender,
    nationality: body.nationality,
    createdAt: new Date().toISOString(),
    isEmailVerified: false,
  }

  users.push(newUser)

  return {
    message: 'تم إنشاء الحساب بنجاح. يرجى التحقق من بريدك الإلكتروني لتفعيل الحساب.',
  }
})
