export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const body = await readBody(event)
  return { success: true, id: Date.now() }
})
