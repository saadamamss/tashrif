export default defineEventHandler(async (event) => {
  await delay()

  deleteCookie(event, 'access_token', {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
  })

  deleteCookie(event, 'refresh_token', {
    httpOnly: true,
    secure: true,
    sameSite: 'none',
    path: '/',
  })

  return { message: 'تم تسجيل الخروج بنجاح' }
})
