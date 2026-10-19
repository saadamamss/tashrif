<script setup>
// Full-page loading placeholder mirroring pages/dashboard/analytics.vue layout
// (stat cards → funnel → top-jobs/timeline → demographics) so the
// loading → data swap keeps the page shape with no jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="analytics-skeleton space-y-6">
    <div class="grid grid-cols-2 lg:grid-cols-5 gap-4">
      <div v-for="i in 5" :key="i" class="bg-white rounded-xl border p-5 text-center">
        <UiSkeleton variant="rect" width="56px" height="32px" class="mx-auto mb-1" />
        <UiSkeleton variant="text" width="72px" height="12px" class="mx-auto" />
      </div>
    </div>

    <div class="bg-white rounded-xl border p-5 sm:p-6">
      <UiSkeleton variant="rect" width="120px" height="20px" class="mb-4" />
      <div class="space-y-3">
        <div v-for="(w, i) in ['100%', '85%', '70%', '55%', '40%', '28%', '18%']" :key="i" class="flex items-center gap-3">
          <UiSkeleton variant="text" width="80px" height="12px" class="shrink-0" />
          <UiSkeleton variant="rect" :width="w" height="24px" />
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white rounded-xl border p-5 sm:p-6">
        <UiSkeleton variant="rect" width="140px" height="20px" class="mb-4" />
        <div class="space-y-3">
          <UiSkeleton v-for="i in 4" :key="i" variant="rect" width="100%" height="52px" />
        </div>
      </div>
      <div class="bg-white rounded-xl border p-5 sm:p-6">
        <UiSkeleton variant="rect" width="140px" height="20px" class="mb-4" />
        <UiSkeleton variant="rect" width="100%" height="180px" />
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div v-for="i in 2" :key="i" class="bg-white rounded-xl border p-5 sm:p-6">
        <UiSkeleton variant="rect" width="100px" height="20px" class="mb-4" />
        <div class="space-y-3">
          <div v-for="j in 3" :key="j" class="flex items-center gap-3">
            <UiSkeleton variant="text" width="80px" height="12px" class="shrink-0" />
            <UiSkeleton variant="rect" width="100%" height="16px" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
