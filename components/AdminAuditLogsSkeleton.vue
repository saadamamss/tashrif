<script setup>
// Table-shaped loading placeholder mirroring
// pages/dashboard/admin/audit-logs.vue logs table
// (date, user, action badge, entity, old→new change, IP)
// so the loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="admin-audit-logs-skeleton overflow-x-auto">
    <div class="grid grid-cols-6 gap-2 pb-3 border-b min-w-[760px]">
      <UiSkeleton v-for="i in 6" :key="i" variant="text" width="70px" height="12px" />
    </div>
    <div
      v-for="i in 8"
      :key="i"
      class="grid grid-cols-6 gap-2 items-center py-3 border-b last:border-0 min-w-[760px]"
    >
      <UiSkeleton variant="text" width="110px" height="12px" />
      <UiSkeleton variant="rect" width="96px" height="14px" />
      <UiSkeleton variant="rect" width="88px" height="24px" rounded="rounded-full" />
      <UiSkeleton variant="text" width="90px" height="12px" />
      <div class="flex items-center gap-1">
        <UiSkeleton variant="text" width="56px" height="12px" />
        <UiSkeleton variant="text" width="56px" height="12px" />
      </div>
      <UiSkeleton variant="text" width="80px" height="12px" />
    </div>
  </div>
</template>
