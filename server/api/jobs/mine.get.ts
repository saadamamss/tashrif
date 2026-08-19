export default defineEventHandler(async (event) => {
  await delay(400)
  const session = requireAuth(event)
  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'لا تملك صلاحية الوصول' })
  }

  const query = getQuery(event)
  let result = jobs.filter(j => j.entityId === session.userId)

  if (query.status) result = result.filter(j => j.status === query.status)
  if (query.type) result = result.filter(j => j.type === query.type)

  const page = Number(query.page) || 1
  const limit = Number(query.limit) || 10
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  const applicantCounts = new Map<number, number>()
  for (const a of applications) {
    applicantCounts.set(a.jobId, (applicantCounts.get(a.jobId) || 0) + 1)
  }

  return {
    items: items.map(j => ({ ...j, isApplied: false, applicantCount: applicantCounts.get(j.id) || 0 })),
    total, page, totalPages,
  }
})
