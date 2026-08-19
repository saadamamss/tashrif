export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const cvIndex = cvs.findIndex(c => c.id === id && c.userId === session.userId)
  if (cvIndex === -1) {
    throw createError({ statusCode: 404, statusMessage: 'السيرة الذاتية غير موجودة' })
  }

  cvs.splice(cvIndex, 1)

  return { message: 'تم حذف السيرة الذاتية بنجاح' }
})