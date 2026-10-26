// F5 (spec-03) — mock GET /auth/verify-email — mirrors the real backend:
//
//   - garbage/missing token → 400 Arabic «رابط التحقق غير صالح»
//   - otherwise: mark the account verified, set the session cookies exactly
//     like mock login (2h/7d HttpOnly), and return the flat UserDto with
//     isEmailVerified: true — the real backend auto-logs the user in on
//     verify (D7/A2), so the mock must too.
//
// Mock-mode simplification: there is no token table, so any sufficiently long
// token redeems; it targets the newest still-unverified registered mock user
// (falling back to the first seed user), matching the register → email →
// click journey demo mode actually shows.
export default defineEventHandler(async (event) => {
  await delay()

  const token = getQuery(event).token
  if (!token || typeof token !== 'string' || token.trim().length < 6) {
    throw createError({ statusCode: 400, statusMessage: 'رابط التحقق غير صالح' })
  }

  const unverified = [...users].reverse().find(u => u.isEmailVerified === false)
  const target = unverified ?? users[0]
  target.isEmailVerified = true

  const accessToken = generateToken(target.id, target.type)
  const refreshToken = generateToken(target.id, target.type)

  setCookie(event, 'access_token', accessToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 2,
  })

  setCookie(event, 'refresh_token', refreshToken, {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
    maxAge: 60 * 60 * 24 * 7,
  })

  return {
    id: target.id,
    name: target.name,
    email: target.email,
    type: target.type,
    phone: target.phone,
    nationalId: target.nationalId,
    gender: target.gender,
    nationality: target.nationality,
    isEmailVerified: true,
  }
})
