export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))
  const body = await readBody(event)

  const acc = bankAccounts.find(b => b.id === id && b.userId === session.userId)
  if (!acc) {
    throw createError({ statusCode: 404, statusMessage: 'الحساب البنكي غير موجود' })
  }

  acc.iban = body?.iban || acc.iban
  acc.bankName = body?.bankName || acc.bankName

  return acc
})