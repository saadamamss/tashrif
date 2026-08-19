export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const appIndex = applications.findIndex(a => a.id === id)
  if (appIndex === -1) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const app = applications[appIndex]
  const job = jobs.find(j => j.id === app.jobId)
  if (!job || (job.entityId !== session.userId && app.userId !== session.userId)) {
    throw createError({ statusCode: 403, statusMessage: 'لا تملك صلاحية حذف هذا الطلب' })
  }

  applications.splice(appIndex, 1)

  return { message: 'تم حذف الطلب بنجاح' }
})
