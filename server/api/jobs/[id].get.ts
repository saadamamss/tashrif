export default defineEventHandler(async (event) => {
  await delay()
  const id = Number(getRouterParam(event, 'id'))
  const job = jobs.find(j => j.id === id)

  if (!job) {
    throw createError({ statusCode: 404, statusMessage: 'الوظيفة غير موجودة' })
  }

  return job
})
