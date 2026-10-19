<script setup>
// Card-shaped loading placeholder mirroring
// pages/dashboard/jobs-explore/details.vue anatomy
// (title → description → logo row / tabs card / sidebar rows → button)
// so the loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="job-details-skeleton">
    <div class="grid grid-cols-7 gap-6 items-start py-4 mb-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8">
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

      <div class="col-span-7 lg:col-span-3 xl:col-span-2 bg-white rounded-xl p-6">
        <div class="pb-4 border-b-2">
          <UiSkeleton variant="rect" width="120px" height="20px" />
        </div>
        <div class="space-y-4 py-4 border-b-2">
          <div v-for="i in 5" :key="i" class="flex gap-2 items-center">
            <UiSkeleton variant="rect" width="20px" height="20px" />
            <UiSkeleton variant="text" width="55%" height="12px" />
          </div>
        </div>
        <div class="mt-4">
          <UiSkeleton variant="rect" width="100%" height="44px" rounded="rounded-full" />
        </div>
      </div>
    </div>
  </div>
</template>
