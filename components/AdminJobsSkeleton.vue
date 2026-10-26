<script setup>
// Table-shaped loading placeholder mirroring
// pages/dashboard/admin/jobs.vue jobs table
// (title+desc, entity, location, status badge, applicant count, action)
// so the loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="admin-jobs-skeleton overflow-x-auto">
    <div class="grid grid-cols-6 gap-2 pb-3 border-b min-w-[720px]">
      <UiSkeleton v-for="i in 6" :key="i" variant="text" width="70px" height="12px" />
    </div>
    <div
      v-for="i in 9"
      :key="i"
      class="grid grid-cols-6 gap-2 items-center py-3 border-b last:border-0 min-w-[720px]"
    >
      <div>
        <UiSkeleton variant="rect" width="120px" height="14px" class="mb-2" />
        <UiSkeleton variant="text" width="160px" height="12px" />
      </div>
      <UiSkeleton variant="text" width="90px" height="12px" />
      <UiSkeleton variant="text" width="70px" height="12px" />
      <UiSkeleton variant="rect" width="64px" height="24px" rounded="rounded-full" />
      <UiSkeleton variant="text" width="32px" height="12px" />
      <UiSkeleton variant="rect" width="72px" height="28px" rounded="rounded-full" />
    </div>
  </div>
</template>
