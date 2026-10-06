export default defineEventHandler(async (event) => {
  const session = requireAuth(event)
  await delay()
  const userApps = applications.filter(a => a.userId === session.userId)
  return {
    totalApplications: userApps.length,
    pendingApps: userApps.filter(a => ['new', 'shortlisted'].includes(a.status)).length,
    interviews: interviews.filter(i => i.userId === session.userId).length,
    signedContracts: contracts.filter(c => c.userId === session.userId && c.status === 'signed').length,
  }
})
