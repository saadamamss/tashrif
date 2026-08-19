export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const expIndex = experiences.findIndex(e => e.id === id && e.userId === session.userId)
  if (expIndex === -1) {
    throw createError({ statusCode: 404, statusMessage: 'الخبرة غير موجودة' })
  }

  experiences.splice(expIndex, 1)

  return { message: 'تم حذف الخبرة بنجاح' }
})