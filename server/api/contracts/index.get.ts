export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  let result: Contract[]
  if (session.type === 'individual') {
    result = contracts.filter(c => c.userId === session.userId)
  } else {
    result = contracts.filter(c => c.entityId === session.userId)
  }

  const page = Number(getQuery(event).page) || 1
  const limit = Number(getQuery(event).limit) || 10
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  return { items, total, page, totalPages }
})
