import type { Contract } from '~~/server/utils/db'
export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'فقط الجهات يمكنها إرسال العقود' })
  }

  const form = await readFormData(event)
  const applicationId = Number(form?.get('applicationId'))

  if (!applicationId) {
    throw createError({ statusCode: 400, statusMessage: 'رقم الطلب مطلوب' })
  }

  const app = applications.find(a => a.id === applicationId)
  if (!app) {
    throw createError({ statusCode: 404, statusMessage: 'الطلب غير موجود' })
  }

  const file = form?.get('contractFile')
  const fileSize = file instanceof File ? file.size : 0
  const fileName = file instanceof File ? file.name : `contract-${contracts.length + 1}.pdf`
  const notes = String(form?.get('notes') || '') || undefined
  const endDate = String(form?.get('endDate') || '') || undefined
  const user = users.find(u => u.id === app.userId)
  const job = jobs.find(j => j.id === app.jobId)
  const entity = users.find(u => u.id === session.userId)

  const newContract: Contract = {
    id: contracts.length + 1,
    applicationId,
    jobId: app.jobId,
    userId: app.userId,
    entityId: session.userId,
    fileUrl: `/contracts/${contracts.length + 1}.pdf`,
    fileSize,
    fileName,
    userName: user?.name || '',
    userAvatar: user?.avatarUrl || '',
    jobTitle: job?.title || '',
    entityName: entity?.name || '',
    entityLogo: entity?.avatarUrl || '',
    notes,
    endDate,
    status: 'sent',
    createdAt: new Date().toISOString(),
  }

  contracts.unshift(newContract)
  app.status = 'contract_sent'

  return newContract
})
