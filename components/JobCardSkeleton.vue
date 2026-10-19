<script setup>
// Card-shaped loading placeholder mirroring JobCard.vue anatomy
// (title → description → logo row → 3 detail rows → 2 buttons) so the
// loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="job-card-skeleton">
    <div class="relative overflow-hidden border bg-white rounded-2xl p-6 lg:p-8">
      <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
        <UiSkeleton variant="rect" width="75%" height="20px" />
        <div>
          <UiSkeleton variant="text" width="100%" height="12px" class="mb-2" />
          <UiSkeleton variant="text" width="66%" height="12px" />
        </div>

        <div class="flex items-center gap-2">
          <UiSkeleton variant="rect" width="48px" height="28px" />
          <UiSkeleton variant="rect" width="96px" height="14px" />
        </div>
      </div>
      <div class="py-6 flex flex-col gap-4">
        <div v-for="i in 3" :key="i" class="flex gap-2 items-center">
          <UiSkeleton variant="rect" width="20px" height="20px" />
          <UiSkeleton variant="text" width="50%" height="12px" />
        </div>
      </div>

      <div class="flex gap-4">
        <UiSkeleton variant="rect" width="100%" height="44px" rounded="rounded-full" />
        <UiSkeleton variant="rect" width="100%" height="44px" rounded="rounded-full" />
      </div>
    </div>
  </div>
</template>
