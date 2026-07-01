export default defineEventHandler(async (event) => {
  await delay(400)
  const query = getQuery(event)
  let result = jobs.filter(j => j.status === 'active')

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

  return { items, total, page, totalPages }
})
