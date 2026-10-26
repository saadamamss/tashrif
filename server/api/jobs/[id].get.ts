export default defineEventHandler(async (event) => {
  await delay()
  const id = Number(getRouterParam(event, 'id'))
  const job = jobs.find(j => j.id === id)

  if (!job) {
    throw createError({ statusCode: 404, statusMessage: 'الوظيفة غير موجودة' })
  }

  const session = getUserFromToken(event)
  const isApplied = !!session && applications.some(a => a.jobId === id && a.userId === session.userId)
  const applicantCount = applications.filter(a => a.jobId === id).length

  return { ...job, isApplied: isApplied, applicantCount }
})
