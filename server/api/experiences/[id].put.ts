export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))
  const body = await readBody(event)

  const exp = experiences.find(e => e.id === id && e.userId === session.userId)
  if (!exp) {
    throw createError({ statusCode: 404, statusMessage: 'الخبرة غير موجودة' })
  }

  exp.jobTitle = body?.jobTitle || exp.jobTitle
  exp.employer = body?.employer || exp.employer
  exp.duration = body?.duration || exp.duration
  exp.location = body?.location || exp.location
  exp.isCurrent = body?.isCurrent ?? exp.isCurrent
  exp.startDate = body?.startDate ?? exp.startDate
  exp.endDate = body?.endDate ?? exp.endDate

  return exp
})