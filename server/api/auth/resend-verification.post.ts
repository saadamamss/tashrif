// F5 (spec-03) — mock POST /auth/resend-verification — mirrors the real
// controller contract (D8). The body MUST carry `nationalId` (the same
// credential the user logs in with, never the email) — anything else is a
// 400 like the real ResendVerificationDto validation. Success always answers
// with the same generic message whether or not the id exists (no
// enumeration oracle). Mock mode does not actually send mail.

export default defineEventHandler(async (event) => {
  await delay()

  const body = await readBody(event)
  const { nationalId } = body || {}

  if (!nationalId || typeof nationalId !== 'string') {
    throw createError({ statusCode: 400, statusMessage: 'رقم الهوية مطلوب' })
  }

  return {
    message: 'إذا كان الحساب موجوداً، فسيتم إرسال رسالة تفعيل جديدة.',
  }
})
