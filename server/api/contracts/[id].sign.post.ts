export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)
  const id = Number(getRouterParam(event, 'id'))

  const contract = contracts.find(c => c.id === id)
  if (!contract) {
    throw createError({ statusCode: 404, statusMessage: 'العقد غير موجود' })
  }

  if (contract.userId !== session.userId) {
    throw createError({ statusCode: 403, statusMessage: 'لا يمكنك توقيع هذا العقد' })
  }

  if (contract.status !== 'sent') {
    throw createError({ statusCode: 400, statusMessage: 'العقد قد تم توقيعه مسبقاً' })
  }

  contract.status = 'signed'
  contract.signedAt = new Date().toISOString()

  const app = applications.find(a => a.id === contract.applicationId)
  if (app) app.status = 'accepted'

  return contract
})
