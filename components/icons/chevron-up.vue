<template>
    <svg aria-hidden="true" focusable="false" :width="width" :height="height" viewBox="0 0 14 8" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M1.58332 7.08398L6.99999 1.25065L12.4167 7.08398" :stroke="color" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
    </svg>
</template>

<script setup>
defineProps({
  width: { type: [Number, String], default: 14 },
  height: { type: [Number, String], default: 8 },
  color: { type: String, default: '#929A9F' },
})
</script>
