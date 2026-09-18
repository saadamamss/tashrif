<template>
  <svg aria-hidden="true" focusable="false" :width="width" :height="height" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M17.65 6.35A7.958 7.958 0 0012 4a8 8 0 108 8h-2a6 6 0 11-1.76-4.24" :stroke="color" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M17.65 6.35L14 10h6V4l-2.35 2.35z" :fill="color"/>
  </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 24 },
  height: { type: [Number, String], default: 24 },
  color: { type: String, default: '#929A9F' },
})
</script>
