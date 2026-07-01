export default defineEventHandler(async (event) => {
  await delay(400)
  const session = requireAuth(event)

  if (session.type !== 'individual') {
    throw createError({ statusCode: 403, statusMessage: 'فقط الأفراد يمكنهم التقديم' })
  }

  const body = await readBody(event)
  const { jobId, qualification, experience } = body || {}

  if (!jobId) {
    throw createError({ statusCode: 400, statusMessage: 'رقم الوظيفة مطلوب' })
  }

  const job = jobs.find(j => j.id === jobId)
  if (!job) {
    throw createError({ statusCode: 404, statusMessage: 'الوظيفة غير موجودة' })
  }

  const existing = applications.find(a => a.jobId === jobId && a.userId === session.userId)
  if (existing) {
    throw createError({ statusCode: 409, statusMessage: 'لقد تقدمت لهذه الوظيفة مسبقاً' })
  }

  const user = users.find(u => u.id === session.userId)

  const newApp: Application = {
    id: applications.length + 1,
    jobId,
    userId: session.userId,
    userName: user?.name || '',
    userGender: user?.gender || '',
    userCity: '',
    qualification: qualification || '',
    status: 'new',
    createdAt: new Date().toISOString(),
  }

  applications.unshift(newApp)

  return newApp
})
