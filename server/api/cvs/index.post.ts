export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  const form = await readFormData(event)
  const file = form?.get('file')

  const newCv: Cv = {
    id: Math.max(0, ...cvs.map(c => c.id)) + 1,
    userId: session.userId,
    fileName: file instanceof File ? file.name : 'السيرة الذاتية.pdf',
    filePath: `/cvs/cv-${Math.max(0, ...cvs.map(c => c.id)) + 1}.pdf`,
    fileSize: file instanceof File ? file.size : 0,
    isDefault: false,
    uploadedAt: new Date().toISOString(),
  }

  cvs.unshift(newCv)

  return newCv
})