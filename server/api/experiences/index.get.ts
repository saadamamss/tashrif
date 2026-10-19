export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const userExps = experiences.filter(e => e.userId === session.userId)
  return { items: userExps }
})
