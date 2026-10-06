export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const jobId = Number(getRouterParam(event, 'id'))
  const query = getQuery(event)

  const job = jobs.find(j => j.id === jobId)
  if (!job) {
    throw createError({ statusCode: 404, statusMessage: 'الوظيفة غير موجودة' })
  }

  if (job.entityId !== session.userId) {
    throw createError({ statusCode: 403, statusMessage: 'لا تملك صلاحية الوصول' })
  }

  let result = applications.filter(a => a.jobId === jobId)

  if (query.status) {
    result = result.filter(a => a.status === query.status)
  }
  if (query.search) {
    const s = (query.search as string).toLowerCase()
    result = result.filter(a => a.userName.includes(s))
  }

  const page = Number(query.page) || 1
  const limit = Number(query.limit) || 20
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  return { items, total, page, totalPages }
})
