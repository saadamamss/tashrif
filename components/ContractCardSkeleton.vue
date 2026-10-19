<script setup>
// Card-shaped loading placeholder mirroring ContractCard.vue anatomy
// (logo row → 2 detail rows → file box → notes line → action button) so the
// loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="contract-card-skeleton">
    <div class="shadow-md bg-white relative overflow-hidden rounded-2xl p-6">
      <div class="pb-4 border-b-2">
        <div class="flex items-center gap-2">
          <UiSkeleton variant="rect" width="48px" height="28px" />
          <UiSkeleton variant="rect" width="96px" height="14px" />
        </div>
      </div>
      <div class="py-6 flex flex-col gap-4">
        <div v-for="i in 2" :key="i" class="flex gap-2 items-center">
          <UiSkeleton variant="rect" width="18px" height="18px" />
          <UiSkeleton variant="text" width="50%" height="12px" />
        </div>
      </div>

      <div class="p-4 rounded-xl bg-bg-subtle mb-2">
        <div class="flex justify-between gap-4 items-center">
          <div class="flex items-center gap-2 flex-1">
            <UiSkeleton variant="rect" width="36px" height="36px" />
            <div class="flex-1">
              <UiSkeleton variant="rect" width="70%" height="14px" class="mb-1" />
              <UiSkeleton variant="text" width="40%" height="12px" />
            </div>
          </div>
          <UiSkeleton variant="rect" width="36px" height="36px" />
        </div>
      </div>
      <UiSkeleton variant="text" width="80%" height="12px" class="mb-6" />

      <UiSkeleton variant="rect" width="100%" height="44px" rounded="rounded-full" />
    </div>
  </div>
</template>
