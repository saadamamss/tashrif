export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'فقط الجهات يمكنها إرسال العقود' })
  }

  const body = await readBody(event)
  const { applicationId } = body || {}

  if (!applicationId) {
    throw createError({ statusCode: 400, statusMessage: 'رقم الطلب مطلوب' })
  }

  const app = applications.find(a => a.id === applicationId)
  if (!app) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const newContract: Contract = {
    id: contracts.length + 1,
    applicationId,
    jobId: app.jobId,
    userId: app.userId,
    entityId: session.userId,
    fileUrl: `/contracts/contract-${contracts.length + 1}.pdf`,
    status: 'sent',
    createdAt: new Date().toISOString(),
  }

  contracts.unshift(newContract)
  app.status = 'contract_sent'

  return newContract
})
