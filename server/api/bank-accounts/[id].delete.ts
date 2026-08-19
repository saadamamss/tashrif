export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const accIndex = bankAccounts.findIndex(b => b.id === id && b.userId === session.userId)
  if (accIndex === -1) {
    throw createError({ statusCode: 404, statusMessage: 'الحساب البنكي غير موجود' })
  }

  bankAccounts.splice(accIndex, 1)

  return { message: 'تم حذف الحساب البنكي بنجاح' }
})