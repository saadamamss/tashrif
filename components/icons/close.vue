<template>
    <svg aria-hidden="true" focusable="false" :width="width" :height="height" viewBox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg">
        <rect width="15" height="14.9826" :fill="bgColor" />
        <path fill-rule="evenodd" clip-rule="evenodd" d="M2.79354 2.79143C2.9766 2.60859 3.2734 2.60859 3.45646 2.79143L7.5 6.83027L11.5435 2.79143C11.7266 2.60859 12.0234 2.60859 12.2065 2.79143C12.3895 2.97428 12.3895 3.27073 12.2065 3.45357L8.16291 7.49241L12.2065 11.5313C12.3895 11.7141 12.3895 12.0106 12.2065 12.1934C12.0234 12.3762 11.7266 12.3762 11.5435 12.1934L7.5 8.15456L3.45646 12.1934C3.2734 12.3762 2.9766 12.3762 2.79354 12.1934C2.61049 12.0106 2.61049 11.7141 2.79354 11.5313L6.83709 7.49241L2.79354 3.45357C2.61049 3.27073 2.61049 2.97428 2.79354 2.79143Z" :fill="color" />
    </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 15 },
  height: { type: [Number, String], default: 15 },
  color: { type: String, default: '#B1B4B0' },
  bgColor: { type: String, default: '#F6F9F5' },
})
</script>
