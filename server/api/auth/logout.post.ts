export default defineEventHandler(async (event) => {
  await delay()
  const auth = getHeader(event, 'authorization')
  if (auth && auth.startsWith('Bearer ')) {
    const token = auth.slice(7)
    delete tokens[token]
  }
  return { success: true }
})
