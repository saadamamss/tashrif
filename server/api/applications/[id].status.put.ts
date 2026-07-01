export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))
  const body = await readBody(event)
  const { status } = body || {}

  const validStatuses = ['new', 'shortlisted', 'interview', 'contract_sent', 'accepted', 'refused']
  if (!status || !validStatuses.includes(status)) {
    throw createError({ statusCode: 400, statusMessage: 'حالة غير صالحة' })
  }

  const app = applications.find(a => a.id === id)
  if (!app) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const job = jobs.find(j => j.id === app.jobId)
  if (!job || job.entityId !== session.userId) {
    throw createError({ statusCode: 403, statusMessage: 'لا تملك صلاحية تعديل هذا الطلب' })
  }

  app.status = status as Application['status']

  return app
})
