export default defineEventHandler(async () => {
  await delay()
  const workTypes = [...new Set(jobs.map(j => {
    const map = { field: 'ميداني', office: 'مكتبي', remote: 'عن بعد' }
    return map[j.type as keyof typeof map] || j.type
  }))]
  const locations = [...new Set(jobs.map(j => j.location.split(' – ')[0]))]
  const genders = [{ value: '', label: 'الكل' }, { value: 'male', label: 'رجال' }, { value: 'female', label: 'نساء' }]
  const entities = [...new Set(jobs.map(j => ({ value: j.entityId, label: j.entityName }))),]
    .filter((v, i, a) => a.findIndex(t => t.value === v.value) === i)
  const statuses = [{ value: '', label: 'الكل' }, { value: 'active', label: 'نشط' }, { value: 'closed', label: 'مغلق' }, { value: 'draft', label: 'مسودة' }]
  return { workTypes, locations, genders, entities, statuses }
})
