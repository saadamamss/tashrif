<script setup lang="ts">
defineProps<{
  label: string
  value: string | number
  accent?: boolean
}>()
</script>

<template>
  <div class="bg-white rounded-xl border p-5 text-center">
    <p class="text-3xl font-extrabold" :class="accent ? 'text-primary' : 'text-dark'">
      {{ value }}
    </p>
    <p class="text-xs text-muted mt-1">{{ label }}</p>
  </div>
</template>
