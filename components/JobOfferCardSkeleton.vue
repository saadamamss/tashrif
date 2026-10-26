<script setup>
// Card-shaped loading placeholder mirroring JobOfferCard.vue anatomy
// (status badge → title → description → 2x2 detail grid) so the
// loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="job-offer-card-skeleton">
    <div class="border bg-white rounded-3xl overflow-hidden relative p-6 lg:p-8">
      <UiSkeleton
        variant="rect"
        width="96px"
        height="32px"
        rounded="rounded-r-3xl rounded-t-[0px]"
        class="absolute top-0 left-0"
      />
      <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
        <UiSkeleton variant="rect" width="70%" height="20px" />
        <div>
          <UiSkeleton variant="text" width="100%" height="12px" class="mb-2" />
          <UiSkeleton variant="text" width="66%" height="12px" />
        </div>
      </div>
      <div class="grid grid-cols-2 gap-x-4 gap-y-6 mt-6">
        <div v-for="i in 4" :key="i" class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <UiSkeleton variant="rect" width="20px" height="20px" />
            <UiSkeleton variant="text" width="60px" height="12px" />
          </div>
          <UiSkeleton variant="rect" width="80px" height="14px" class="ms-7" />
        </div>
      </div>
    </div>
  </div>
</template>
