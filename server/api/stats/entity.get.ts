export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const entityJobs = jobs.filter(j => j.entityId === session.userId)
  const entityJobIds = entityJobs.map(j => j.id)
  const allApplicants = applications.filter(a => entityJobIds.includes(a.jobId))
  return {
    totalJobs: entityJobs.length,
    activeJobs: entityJobs.filter(j => j.status === 'active').length,
    totalApplicants: allApplicants.length,
  }
})
