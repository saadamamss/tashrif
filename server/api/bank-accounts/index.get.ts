export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const userAccounts = bankAccounts.filter(b => b.userId === session.userId)
  return { items: userAccounts }
})
