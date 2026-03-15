<template>
  <button @click="addToCalendar" class="flex-1 text-center btn-primary text-sm">
    إضافةالي تقويم جوجل
  </button>
</template>
<script setup>
const generateGoogleCalendarLink = () => {
  const event = {
    title: "مقابلة شخصية لوظيفة [اسم الوظيفة]",
    start: "20230724T090000", // تاريخ ووقت البداية (تنسيق: YYYYMMDDTHHMMSS)
    end: "20230724T100000", // تاريخ ووقت النهاية
    details: "مقابلة شخصية مع [اسم الشركة]",
    location: "عنوان المقابلة أو رابط إذا كانت عن بعد",
  };

  return `https://www.google.com/calendar/render?action=TEMPLATE&text=${encodeURIComponent(
    event.title
  )}&dates=${event.start}/${event.end}&details=${encodeURIComponent(
    event.details
  )}&location=${encodeURIComponent(event.location)}`;
};
const addToCalendar = () => {
  window.open(generateGoogleCalendarLink(), "_blank");
};
</script>
