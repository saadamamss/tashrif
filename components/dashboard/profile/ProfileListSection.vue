<script setup>
// 1. imports
import UiSkeleton from '~/components/ui/UiSkeleton.vue'

// 2. props
defineProps({
  title: { type: String, required: true },
  loading: { type: Boolean, default: false },
  skeletonCount: { type: Number, default: 2 },
  skeletonHeight: { type: String, default: '96px' },
})
</script>

<template>
  <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
    <div class="py-6 border-b-2 flex justify-between items-center">
      <h1 class="text-base font-bold">{{ title }}</h1>
      <slot name="header-action" />
    </div>
    <div class="py-6">
      <div v-if="loading" class="space-y-3">
        <UiSkeleton
          v-for="i in skeletonCount"
          :key="i"
          variant="rect"
          width="100%"
          :height="skeletonHeight"
        />
      </div>
      <slot v-else />
    </div>
  </div>
</template>
