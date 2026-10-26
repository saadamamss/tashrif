<script setup>
// 1. imports — none (Nuxt auto-imports `computed`)
// 2. props
const props = defineProps({
  variant: {
    type: String,
    default: 'rect',
    validator: (v) => ['circle', 'rect', 'text'].includes(v),
  },
  width: { type: String, default: '100%' },
  height: { type: String, default: '16px' },
  rounded: { type: String, default: '' },
})

// 3. reactive state
const radiusClass = computed(() => {
  if (props.rounded) return props.rounded
  if (props.variant === 'circle') return 'rounded-full'
  if (props.variant === 'text') return 'rounded'
  return 'rounded-xl'
})
</script>

<template>
  <div
    class="animate-pulse bg-gray-200"
    :class="radiusClass"
    :style="{ width, height }"
    aria-hidden="true"
  />
</template>
