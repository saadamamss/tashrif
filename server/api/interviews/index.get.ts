import type { Interview } from '~~/server/utils/db'
export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const query = getQuery(event)

  let result: Interview[]
  if (session.type === 'individual') {
    result = interviews.filter(i => i.userId === session.userId)
  } else {
    result = interviews.filter(i => i.entityId === session.userId)
  }

  if (query.status) {
    result = result.filter(i => i.status === query.status)
  }

  const page = Number(query.page) || 1
  const limit = Number(query.limit) || 10
  const total = result.length
  const totalPages = Math.ceil(total / limit)
  const start = (page - 1) * limit
  const items = result.slice(start, start + limit)

  return { items, total, page, totalPages }
})
