export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const userQuals = qualifications.filter(q => q.userId === session.userId)
  return { items: userQuals }
})
