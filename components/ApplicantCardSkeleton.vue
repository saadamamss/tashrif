<script setup>
// Card-shaped loading placeholder mirroring ApplicantCard.vue anatomy
// (avatar + name + badge → 5 detail rows) so the loading → data swap
// is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="applicant-card-skeleton">
    <div class="rounded-2xl bg-white p-4 sm:p-5">
      <div class="pb-4 border-b-2">
        <div class="flex items-center gap-2">
          <UiSkeleton variant="circle" width="48px" height="48px" />
          <div class="flex-1">
            <UiSkeleton variant="rect" width="120px" height="14px" class="mb-1" />
            <UiSkeleton variant="text" width="80px" height="12px" />
          </div>
          <UiSkeleton variant="rect" width="72px" height="40px" rounded="rounded-xl" />
        </div>
      </div>
      <div class="pt-6">
        <div class="grid grid-cols-2 gap-y-6">
          <div v-for="i in 5" :key="i" class="text-xs">
            <div class="flex gap-2 items-center mb-2">
              <UiSkeleton variant="rect" width="20px" height="20px" />
              <UiSkeleton variant="text" width="60px" height="12px" />
            </div>
            <UiSkeleton variant="rect" width="80px" height="14px" class="ms-7" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
