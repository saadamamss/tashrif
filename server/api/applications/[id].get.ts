export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const app = applications.find(a => a.id === id)
  if (!app) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const job = jobs.find(j => j.id === app.jobId)
  if (app.userId !== session.userId && job?.entityId !== session.userId) {
    throw createError({ statusCode: 403, statusMessage: 'غير مصرح لك بمشاهدة هذا الطلب' })
  }

  return { ...app, job: job || null }
})