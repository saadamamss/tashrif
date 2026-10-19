<template>
  <svg aria-hidden="true" focusable="false" :width="width" :height="height" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path opacity="0.4" d="M8 4.5H6C4.89543 4.5 4 5.39543 4 6.5V18C4 19.1046 4.89543 20 6 20H18C19.1046 20 20 19.1046 20 18V6.5C20 5.39543 19.1046 4.5 18 4.5H16" :fill="color" />
    <rect x="8" y="2.5" width="8" height="4" rx="1.5" :fill="color" />
    <path d="M8.5 11H15.5" :stroke="color" stroke-width="1.8" stroke-linecap="round" />
    <path d="M8.5 14.5H13.5" :stroke="color" stroke-width="1.8" stroke-linecap="round" />
  </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 24 },
  height: { type: [Number, String], default: 24 },
  color: { type: String, default: '#929A9F' },
})
</script>
