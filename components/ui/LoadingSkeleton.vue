<script setup>
defineProps({
  rows: { type: Number, default: 1 },
  columns: { type: Number, default: 1 },
  height: { type: String, default: '200px' },
  rounded: { type: String, default: 'xl' },
})
</script>

<template>
  <div
    class="grid gap-4"
    :style="{ gridTemplateColumns: `repeat(${columns}, 1fr)` }"
  >
    <div v-for="r in rows" :key="r">
      <div
        v-for="c in columns"
        :key="c"
        class="bg-gray-100 animate-pulse"
        :style="{ height, borderRadius: `var(--radius-${rounded})` }"
      >
        <div class="p-4 space-y-3">
          <div class="h-4 bg-gray-200 rounded w-3/4" />
          <div class="h-3 bg-gray-200 rounded w-1/2" />
          <div class="h-3 bg-gray-200 rounded w-full" />
          <div class="h-3 bg-gray-200 rounded w-2/3" />
        </div>
      </div>
    </div>
  </div>
</template>
