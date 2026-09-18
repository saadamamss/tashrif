<template>
  <svg aria-hidden="true" focusable="false" :width="width" :height="height" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path opacity="0.4" d="M5 21H19C19.5523 21 20 20.5523 20 20C20 19.4477 19.5523 19 19 19H5C4.44772 19 4 19.4477 4 20C4 20.5523 4.44772 21 5 21Z" :fill="color" />
    <rect x="5" y="10" width="3" height="6" rx="1" :fill="color" />
    <rect x="10.5" y="6" width="3" height="10" rx="1" :fill="color" />
    <rect x="16" y="12" width="3" height="4" rx="1" :fill="color" />
    <path d="M3.75 3.75C3.75 3.33579 3.41421 3 3 3C2.58579 3 2.25 3.33579 2.25 3.75V20.25C2.25 20.6642 2.58579 21 3 21C3.41421 21 3.75 20.6642 3.75 20.25V3.75Z" :fill="color" />
    <path d="M21.75 3.75C21.75 3.33579 21.4142 3 21 3C20.5858 3 20.25 3.33579 20.25 3.75V20.25C20.25 20.6642 20.5858 21 21 21C21.4142 21 21.75 20.6642 21.75 20.25V3.75Z" :fill="color" />
  </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 24 },
  height: { type: [Number, String], default: 24 },
  color: { type: String, default: '#929A9F' },
})
</script>
