<script setup>
defineProps({
  size: { type: String, default: '40px' },
  color: { type: String, default: '#ecb42b' },
})
</script>

<template>
  <div class="flex items-center justify-center py-8">
    <svg
      :style="{ width: size, height: size }"
      class="animate-spin"
      viewBox="0 0 24 24"
      fill="none"
    >
      <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3" opacity="0.2" :stroke="color" />
      <path
        d="M12 2a10 10 0 019.95 9"
        stroke="currentColor"
        stroke-width="3"
        stroke-linecap="round"
        :stroke="color"
      />
    </svg>
  </div>
</template>
