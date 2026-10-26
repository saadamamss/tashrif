<script setup>
// 1. imports
import UiSkeleton from '~/components/ui/UiSkeleton.vue'

// 2. props
defineProps({
  title: { type: String, required: true },
  items: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
  skeletonCount: { type: Number, default: 8 },
})
</script>

<template>
  <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
    <div class="py-6 border-b-2 flex justify-between items-center">
      <h1 class="text-base font-bold">{{ title }}</h1>
      <slot name="header-action" />
    </div>
    <div class="py-6">
      <div v-if="loading" class="grid xs:grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-6">
        <div v-for="i in skeletonCount" :key="i" class="flex items-start gap-2">
          <UiSkeleton variant="rect" width="20px" height="20px" />
          <div class="flex-1">
            <UiSkeleton variant="text" width="60px" height="12px" class="mb-2" />
            <UiSkeleton variant="rect" width="100px" height="14px" />
          </div>
        </div>
      </div>
      <div v-else class="grid xs:grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-6">
        <div
          v-for="(item, index) in items"
          :key="index"
          class="flex items-start gap-2"
        >
          <component :is="item.icon" v-if="item.icon" />
          <div class="text-xs">
            <span class="block text-icon-muted mb-2">{{ item.key }}</span>
            <span class="block text-slate-800 font-bold">{{ item.value }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
