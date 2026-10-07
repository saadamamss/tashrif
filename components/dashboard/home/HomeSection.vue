<script setup>
// 1. imports
import UiLoadingSkeleton from '~/components/ui/LoadingSkeleton.vue'

// 2. props
defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
  loading: { type: Boolean, default: false },
  skeletonCount: { type: Number, default: 3 },
  skeletonHeight: { type: String, default: '180px' },
})
</script>

<template>
  <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
    <div class="container-fluid">
      <div class="pb-8">
        <div class="p-4 xs:p-6 sm:p-8">
          <div class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between">
            <div>
              <h1 class="text-base md:text-lg font-semibold mb-3">{{ title }}</h1>
              <p v-if="subtitle" class="text-sm text-muted">{{ subtitle }}</p>
            </div>
            <slot name="action" />
          </div>
          <div class="py-4">
            <hr />
          </div>
        </div>

        <div v-if="loading" class="p-4 xs:p-6 sm:ps-8">
          <slot name="skeleton">
            <UiLoadingSkeleton :count="skeletonCount" :columns="3" :height="skeletonHeight" />
          </slot>
        </div>
        <slot v-else />
      </div>
    </div>
  </section>
</template>
