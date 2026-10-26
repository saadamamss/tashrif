export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'individual') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const body = await readBody(event)
  const user = users.find(u => u.id === session.userId)

  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  if (body.name) user.name = body.name
  if (body.email) user.email = body.email
  if (body.phone) user.phone = body.phone
  if (body.gender) user.gender = body.gender
  if (body.nationality) user.nationality = body.nationality

  let profile = individualProfiles.find(p => p.userId === session.userId)

  if (!profile) {
    profile = {
      id: individualProfiles.length + 1,
      userId: session.userId,
      birthDate: '',
      city: '',
      zone: '',
      district: '',
      street: '',
      zipcode: '',
      jobTitle: '',
      profileCompletionPct: 0,
    }
    individualProfiles.push(profile)
  }

  if (body.birthDate !== undefined) profile.birthDate = body.birthDate
  if (body.city !== undefined) profile.city = body.city
  if (body.zone !== undefined) profile.zone = body.zone
  if (body.district !== undefined) profile.district = body.district
  if (body.street !== undefined) profile.street = body.street
  if (body.zipcode !== undefined) profile.zipcode = body.zipcode
  if (body.jobTitle !== undefined) profile.jobTitle = body.jobTitle

  const completionFields = [profile.city, profile.zone, profile.district, profile.street, profile.zipcode, profile.jobTitle]
  const filled = completionFields.filter(f => f && String(f).trim() !== '').length
  profile.profileCompletionPct = Math.round(filled * 100 / completionFields.length)

  return {
    id: user.id,
    name: user.name,
    email: user.email,
    phone: user.phone,
    type: user.type,
    gender: user.gender,
    nationality: user.nationality,
    nationalId: user.nationalId,
    birthDate: profile.birthDate,
    city: profile.city,
    zone: profile.zone,
    district: profile.district,
    street: profile.street,
    zipcode: profile.zipcode,
    jobTitle: profile.jobTitle,
    profileCompletionPct: profile.profileCompletionPct,
    stats: {
      totalApplications: applications.filter(a => a.userId === session.userId).length,
      pendingApps: applications.filter(a => a.userId === session.userId && a.status === 'new').length,
      interviews: interviews.filter(i => i.userId === session.userId).length,
      contracts: contracts.filter(c => c.userId === session.userId).length,
    },
  }
})