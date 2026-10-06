export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  const body = await readBody(event)

  const newExp: Experience = {
    id: Math.max(0, ...experiences.map(e => e.id)) + 1,
    userId: session.userId,
    jobTitle: body?.jobTitle || '',
    employer: body?.employer || '',
    duration: body?.duration || '',
    location: body?.location || '',
    isCurrent: !!body?.isCurrent,
    startDate: body?.startDate || '',
    endDate: body?.endDate || '',
  }

  experiences.push(newExp)

  return newExp
})