export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const user = users.find(u => u.id === session.userId)
  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  const myJobs = jobs.filter(j => j.entityId === session.userId)
  const stats = {
    totalJobs: myJobs.length,
    activeJobs: myJobs.filter(j => j.status === 'active').length,
    totalApplicants: applications.filter(a => myJobs.some(j => j.id === a.jobId)).length,
  }

  return {
    id: user.id,
    name: user.name,
    email: user.email,
    phone: user.phone,
    type: user.type,
    stats,
  }
})
