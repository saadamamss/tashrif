export default defineEventHandler(async (event) => {
  await delay()
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'Forbidden' })
  }

  const body = await readBody(event)
  const user = users.find(u => u.id === session.userId)

  if (!user) {
    throw createError({ statusCode: 404, statusMessage: 'User not found' })
  }

  if (body.name) user.name = body.name
  if (body.phone) user.phone = body.phone

  let profile = entityProfiles.find(p => p.userId === session.userId)

  if (!profile) {
    profile = {
      id: entityProfiles.length + 1,
      userId: session.userId,
      companyField: '',
      sector: '',
      companySize: '',
      commercialReg: '',
      country: '',
      city: '',
      zone: '',
      district: '',
      street: '',
      zipcode: '',
      website: '',
      facebookUrl: '',
      twitterUrl: '',
      youtubeUrl: '',
      logoUrl: '',
      profileCompletionPct: 0,
    }
    entityProfiles.push(profile)
  }

  if (body.companyField !== undefined) profile.companyField = body.companyField
  if (body.companySize !== undefined) profile.companySize = body.companySize
  if (body.commercialReg !== undefined) profile.commercialReg = body.commercialReg
  if (body.sector !== undefined) profile.sector = body.sector
  if (body.country !== undefined) profile.country = body.country
  if (body.city !== undefined) profile.city = body.city
  if (body.zone !== undefined) profile.zone = body.zone
  if (body.district !== undefined) profile.district = body.district
  if (body.street !== undefined) profile.street = body.street
  if (body.zipcode !== undefined) profile.zipcode = body.zipcode
  if (body.website !== undefined) profile.website = body.website
  if (body.facebookAccount !== undefined) profile.facebookUrl = body.facebookAccount
  if (body.twitterAccount !== undefined) profile.twitterUrl = body.twitterAccount
  if (body.youtubeAccount !== undefined) profile.youtubeUrl = body.youtubeAccount
  if (body.logoUrl !== undefined) profile.logoUrl = body.logoUrl

  const completionFields = [
    profile.logoUrl, profile.companyField, profile.sector, profile.companySize,
    profile.commercialReg, profile.country, profile.city, profile.zone,
    profile.district, profile.street, profile.zipcode, profile.website,
    profile.facebookUrl, profile.twitterUrl, profile.youtubeUrl,
  ]
  const filled = completionFields.filter(f => f && String(f).trim() !== '').length
  profile.profileCompletionPct = Math.round(filled * 100 / completionFields.length)

  const myJobs = jobs.filter(j => j.entityId === session.userId)

  return {
    id: user.id,
    name: user.name,
    email: user.email,
    phone: user.phone,
    type: user.type,
    companyField: profile.companyField,
    sector: profile.sector,
    companySize: profile.companySize,
    commercialReg: profile.commercialReg,
    country: profile.country,
    city: profile.city,
    zone: profile.zone,
    district: profile.district,
    street: profile.street,
    zipcode: profile.zipcode,
    website: profile.website,
    facebookUrl: profile.facebookUrl,
    twitterUrl: profile.twitterUrl,
    youtubeUrl: profile.youtubeUrl,
    logoUrl: profile.logoUrl,
    profileCompletionPct: profile.profileCompletionPct,
    stats: {
      totalJobs: myJobs.length,
      activeJobs: myJobs.filter(j => j.status === 'active').length,
      totalApplicants: applications.filter(a => myJobs.some(j => j.id === a.jobId)).length,
    },
  }
})