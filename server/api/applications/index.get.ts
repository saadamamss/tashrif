export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const query = getQuery(event)

  let result = applications.filter(a => a.userId === session.userId)

  if (query.status) {
    result = result.filter(a => a.status === query.status)
  }

  const page = Number(query.page) || 1
  const limit = Number(query.limit) || 10
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  return { items, total, page, totalPages }
})
