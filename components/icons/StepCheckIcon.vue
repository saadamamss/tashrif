<template>
  <svg :width="width" :height="height" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M26.6668 8L12.0002 22.6667L5.3335 16" :stroke="color" stroke-width="2.8" stroke-linecap="round" stroke-linejoin="round" />
  </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 32 },
  height: { type: [Number, String], default: 32 },
  color: { type: String, default: '#FFF5E8' },
})
</script>
