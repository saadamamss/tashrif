export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const userCvs = cvs.filter(c => c.userId === session.userId)
  return { items: userCvs }
})
