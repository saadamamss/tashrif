export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const profile = entityProfiles.find(p => p.userId === session.userId)
  if (!profile) {
    return { items: [] }
  }

  const persons = contactPersons.filter(c => c.entityId === profile.id)
  return { items: persons }
})
