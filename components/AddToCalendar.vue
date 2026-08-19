<template>
  <button @click="addToCalendar" class="flex-1 text-center btn-primary text-sm">
    إضافة إلى تقويم جوجل
  </button>
</template>
<script setup>
const props = defineProps({
  title: { type: String, default: 'مقابلة شخصية' },
  start: { type: String, default: '' },
  startTime: { type: String, default: '10:00' },
  durationHours: { type: Number, default: 1 },
  location: { type: String, default: '' },
})

const toGoogleFormat = (dateStr, timeStr) => {
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  const [h, m] = (timeStr || '10:00').split(':')
  d.setHours(parseInt(h, 10), parseInt(m, 10), 0, 0)
  const pad = (n) => String(n).padStart(2, '0')
  return `${d.getFullYear()}${pad(d.getMonth() + 1)}${pad(d.getDate())}T${pad(d.getHours())}${pad(d.getMinutes())}00`
}

const generateGoogleCalendarLink = () => {
  const start = toGoogleFormat(props.start, props.startTime)
  if (!start) return 'https://www.google.com/calendar/render?action=TEMPLATE&text=' + encodeURIComponent(props.title)

  const d = new Date(props.start)
  const end = new Date(d.getTime() + props.durationHours * 60 * 60 * 1000)
  const pad = (n) => String(n).padStart(2, '0')
  const endStr = `${end.getFullYear()}${pad(end.getMonth() + 1)}${pad(end.getDate())}T${pad(end.getHours())}${pad(end.getMinutes())}00`

  return `https://www.google.com/calendar/render?action=TEMPLATE&text=${encodeURIComponent(
    props.title
  )}&dates=${start}/${endStr}&details=${encodeURIComponent(props.title)}&location=${encodeURIComponent(props.location)}`
}
const addToCalendar = () => {
  window.open(generateGoogleCalendarLink(), "_blank")
}
</script>
