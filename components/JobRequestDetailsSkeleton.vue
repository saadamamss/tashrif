<script setup>
// Card-shaped loading placeholder mirroring
// pages/dashboard/job-requests/details.vue anatomy
// (main card + tabs card / sidebar: status + history + job-details)
// so the loading → data swap is 1:1 with no layout jump.
// Interview/contract cards are conditional — skeleton keeps the three
// always-present sidebar cards so the page shape holds.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="job-request-details-skeleton">
    <div class="grid grid-cols-7 gap-6 items-start mt-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8 relative overflow-hidden">
          <UiSkeleton variant="rect" width="96px" height="28px" rounded="rounded-full" class="mb-6" />
          <div class="flex flex-col gap-6">
            <UiSkeleton variant="rect" width="60%" height="24px" />
            <div>
              <UiSkeleton variant="text" width="100%" height="14px" class="mb-2" />
              <UiSkeleton variant="text" width="85%" height="14px" class="mb-2" />
              <UiSkeleton variant="text" width="40%" height="14px" />
            </div>

            <div class="flex items-center gap-2">
              <UiSkeleton variant="rect" width="48px" height="28px" />
              <UiSkeleton variant="rect" width="120px" height="14px" />
            </div>
          </div>
        </div>

        <div class="flex gap-2 mb-4">
          <UiSkeleton v-for="i in 3" :key="i" variant="rect" width="120px" height="40px" rounded="rounded-full" />
        </div>
        <div class="p-5 bg-white rounded-2xl border-2">
          <UiSkeleton variant="rect" width="140px" height="18px" class="mb-4" />
          <div class="space-y-3">
            <UiSkeleton v-for="i in 3" :key="i" variant="text" width="90%" height="14px" />
          </div>
        </div>
      </div>

      <div class="col-span-7 lg:col-span-3 xl:col-span-2 space-y-6">
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <UiSkeleton variant="rect" width="100px" height="20px" />
          </div>
          <div class="space-y-4 pt-4">
            <UiSkeleton variant="rect" width="100%" height="80px" rounded="rounded-xl" />
            <UiSkeleton variant="text" width="70%" height="12px" />
            <UiSkeleton variant="rect" width="100%" height="40px" rounded="rounded-full" />
          </div>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <UiSkeleton variant="rect" width="100px" height="20px" />
          </div>
          <div class="pt-4 space-y-3">
            <div v-for="i in 4" :key="i" class="flex gap-2 items-center">
              <UiSkeleton variant="circle" width="20px" height="20px" />
              <UiSkeleton variant="text" width="60%" height="12px" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <UiSkeleton variant="rect" width="120px" height="20px" />
          </div>
          <div class="space-y-4 pt-4">
            <div v-for="i in 5" :key="i" class="flex gap-2 items-center">
              <UiSkeleton variant="rect" width="20px" height="20px" />
              <UiSkeleton variant="text" width="55%" height="12px" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
