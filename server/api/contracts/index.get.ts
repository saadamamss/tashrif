import type { Contract } from '~~/server/utils/db'
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
  const items = result.slice(start, start + limit).map(c => {
    const user = users.find(u => u.id === c.userId)
    const job = jobs.find(j => j.id === c.jobId)
    const entity = users.find(u => u.id === c.entityId)
    return {
      ...c,
      userName: user?.name || '',
      userAvatar: user?.avatarUrl || '',
      jobTitle: job?.title || '',
      entityName: entity?.name || '',
      entityLogo: entity?.avatarUrl || '',
      fileName: c.fileUrl ? c.fileUrl.split('/').pop() : undefined,
    }
  })

  return { items, total, page, totalPages }
})
