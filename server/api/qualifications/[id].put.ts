export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))
  const body = await readBody(event)

  const qual = qualifications.find(q => q.id === id && q.userId === session.userId)
  if (!qual) {
    throw createError({ statusCode: 404, statusMessage: 'المؤهل غير موجود' })
  }

  qual.type = body?.qualificationType || qual.type
  qual.specialization = body?.specialization || qual.specialization
  qual.institution = body?.institution || qual.institution
  qual.graduationYear = body?.graduationYear ?? qual.graduationYear
  qual.grade = body?.grade || qual.grade

  return qual
})