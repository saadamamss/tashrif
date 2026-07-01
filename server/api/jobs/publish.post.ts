import type { Job } from '~~/server/utils/db'
export default defineEventHandler(async (event) => {
  await delay(500)
  const session = requireAuth(event)

  if (session.type !== 'entity') {
    throw createError({ statusCode: 403, statusMessage: 'فقط الجهات يمكنها نشر وظائف' })
  }

  const body = await readBody(event)
  const entity = users.find(u => u.id === session.userId)

  const newJob: Job = {
    id: jobs.length + 1,
    entityId: session.userId,
    entityName: entity?.name || '',
    entityLogo: '',
    title: body.title || '',
    description: body.description || '',
    location: body.location || '',
    type: body.type || 'field',
    target: body.target || '',
    vacancies: Number(body.vacancies) || 1,
    qualification: body.qualification || '',
    salary: body.salary || '',
    benefits: body.benefits || [],
    responsibilities: body.responsibilities || [],
    conditions: body.conditions || [],
    gender: body.gender || '',
    hours: body.hours || '',
    duration: body.duration || '',
    status: 'active',
    publishDate: new Date().toISOString().split('T')[0],
    endDate: body.endDate || '',
    createdAt: new Date().toISOString(),
  }

  jobs.unshift(newJob)

  return newJob
})
