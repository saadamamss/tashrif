export default defineEventHandler(async (event) => {
  await delay(400)
  const query = getQuery(event)
  let result = jobs.filter(j => j.status === 'active')

  if (query.status) result = result.filter(j => j.status === query.status)
  if (query.type) result = result.filter(j => j.type === query.type)
  if (query.location) result = result.filter(j => j.location.includes(query.location as string))
  if (query.gender) result = result.filter(j => j.gender === query.gender || j.gender === 'رجال ونساء')
  if (query.entityId) result = result.filter(j => j.entityId === Number(query.entityId))
  if (query.search) {
    const s = (query.search as string).toLowerCase()
    result = result.filter(j => j.title.includes(s) || j.description.includes(s))
  }

  const page = Number(query.page) || 1
  const limit = Number(query.limit) || 10
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  const session = getUserFromToken(event)
  const appliedJobIds = new Set(
    (session ? applications.filter(a => a.userId === session.userId) : [])
      .map(a => a.jobId),
  )

  const applicantCounts = new Map<number, number>()
  for (const a of applications) {
    applicantCounts.set(a.jobId, (applicantCounts.get(a.jobId) || 0) + 1)
  }

  return {
    items: items.map(j => ({ ...j, isApplied: !!session && appliedJobIds.has(j.id), applicantCount: applicantCounts.get(j.id) || 0 })),
    total, page, totalPages,
  }
})
