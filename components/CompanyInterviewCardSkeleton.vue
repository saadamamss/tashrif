<script setup>
// Card-shaped loading placeholder mirroring CompanyInterviewCard.vue anatomy
// (status badge → avatar row → 3 detail rows → action row) so the
// loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="company-interview-card-skeleton">
    <div class="border bg-white relative overflow-hidden rounded-2xl p-6 lg:p-8">
      <UiSkeleton
        variant="rect"
        width="96px"
        height="32px"
        rounded="rounded-r-3xl rounded-t-[0px]"
        class="absolute top-0 left-0"
      />
      <div class="pb-4 border-b-2">
        <div class="flex items-center gap-2">
          <UiSkeleton variant="circle" width="40px" height="40px" />
          <UiSkeleton variant="rect" width="96px" height="14px" />
        </div>
      </div>
      <div class="py-6 flex flex-col gap-4">
        <div v-for="i in 3" :key="i" class="flex gap-2 items-center">
          <UiSkeleton variant="rect" width="20px" height="20px" />
          <UiSkeleton variant="text" width="50%" height="12px" />
        </div>
      </div>

      <UiSkeleton variant="rect" width="100%" height="44px" rounded="rounded-full" />
    </div>
  </div>
</template>
