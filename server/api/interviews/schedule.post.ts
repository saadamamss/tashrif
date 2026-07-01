export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'فقط الجهات يمكنها جدولة المقابلات' })
  }

  const body = await readBody(event)
  const { applicationId, method, date, time, location, link, notes } = body || {}

  if (!applicationId || !method || !date || !time) {
    throw createError({ statusCode: 400, statusMessage: 'بيانات المقابلة غير كاملة' })
  }

  const app = applications.find(a => a.id === applicationId)
  if (!app) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const newInterview: Interview = {
    id: interviews.length + 1,
    applicationId,
    jobId: app.jobId,
    userId: app.userId,
    entityId: session.userId,
    method,
    date,
    time,
    location,
    link,
    notes: notes || '',
    status: 'scheduled',
    attendance: 'pending',
    createdAt: new Date().toISOString(),
  }

  interviews.unshift(newInterview)

  app.status = 'interview'

  return newInterview
})
