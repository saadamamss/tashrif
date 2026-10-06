export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'individual') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const user = users.find(u => u.id === session.userId)
  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  const profile = individualProfiles.find(p => p.userId === session.userId)

  const myApps = applications.filter(a => a.userId === session.userId)
  const stats = {
    totalApplications: myApps.length,
    pendingApps: myApps.filter(a => a.status === 'new').length,
    interviews: interviews.filter(i => i.userId === session.userId).length,
    contracts: contracts.filter(c => c.userId === session.userId).length,
  }

  return {
    id: user.id,
    name: user.name,
    email: user.email,
    phone: user.phone,
    type: user.type,
    gender: user.gender,
    nationality: user.nationality,
    nationalId: user.nationalId,
    ...profile ? {
      birthDate: profile.birthDate,
      city: profile.city,
      zone: profile.zone,
      district: profile.district,
      street: profile.street,
      zipcode: profile.zipcode,
      jobTitle: profile.jobTitle,
      profileCompletionPct: profile.profileCompletionPct,
    } : {},
    stats,
  }
})
