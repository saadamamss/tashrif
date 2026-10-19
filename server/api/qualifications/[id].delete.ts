export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const qualIndex = qualifications.findIndex(q => q.id === id && q.userId === session.userId)
  if (qualIndex === -1) {
    throw createError({ statusCode: 404, statusMessage: 'المؤهل غير موجود' })
  }

  qualifications.splice(qualIndex, 1)

  return { message: 'تم حذف المؤهل بنجاح' }
})