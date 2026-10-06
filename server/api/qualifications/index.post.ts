export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const body = await readBody(event)
  const newQual = {
    id: qualifications.length + 1,
    userId: session.userId,
    type: body?.qualificationType || '',
    specialization: body?.specialization || '',
    institution: body?.institution || '',
    graduationYear: body?.graduationYear || null,
    grade: body?.grade || '',
  }
  qualifications.push(newQual)
  return { id: newQual.id }
})