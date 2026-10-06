export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const body = await readBody(event)

  const newAccount: BankAccount = {
    id: Math.max(0, ...bankAccounts.map(b => b.id)) + 1,
    userId: session.userId,
    iban: body?.iban || '',
    bankName: body?.bankName || '',
    ibanStatus: 'pending',
    accountStatus: 'active',
  }

  bankAccounts.push(newAccount)

  return newAccount
})