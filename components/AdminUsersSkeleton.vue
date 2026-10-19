<script setup>
// Table-shaped loading placeholder mirroring
// pages/dashboard/admin/users.vue user table
// (user avatar+name/email, type badge, nationalId, status badge, action)
// so the loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="admin-users-skeleton overflow-x-auto">
    <div class="grid grid-cols-5 gap-2 pb-3 border-b min-w-[640px]">
      <UiSkeleton v-for="i in 5" :key="i" variant="text" width="70px" height="12px" />
    </div>
    <div
      v-for="i in 6"
      :key="i"
      class="grid grid-cols-5 gap-2 items-center py-3 border-b last:border-0 min-w-[640px]"
    >
      <div class="flex items-center gap-3">
        <UiSkeleton variant="circle" width="36px" height="36px" class="shrink-0" />
        <div>
          <UiSkeleton variant="rect" width="96px" height="14px" class="mb-2" />
          <UiSkeleton variant="text" width="120px" height="12px" />
        </div>
      </div>
      <UiSkeleton variant="rect" width="64px" height="24px" rounded="rounded-full" />
      <UiSkeleton variant="text" width="90px" height="12px" />
      <UiSkeleton variant="rect" width="64px" height="24px" rounded="rounded-full" />
      <UiSkeleton variant="rect" width="72px" height="28px" rounded="rounded-full" />
    </div>
  </div>
</template>
