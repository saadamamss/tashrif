<script setup>
// Card-shaped loading placeholder mirroring CompanyContractCard.vue anatomy
// (status badge → avatar row → 2 detail columns → file box) so the
// loading → data swap is 1:1 with no layout jump.
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
</script>

<template>
  <div class="company-contract-card-skeleton">
    <div class="shadow-md bg-white relative overflow-hidden rounded-2xl px-4 py-6 sm:p-6">
      <UiSkeleton
        variant="rect"
        width="96px"
        height="32px"
        rounded="rounded-r-3xl rounded-t-[0px]"
        class="absolute top-0 left-0"
      />
      <div class="pb-4 border-b-2">
        <div class="flex items-center gap-2">
          <UiSkeleton variant="circle" width="48px" height="48px" />
          <UiSkeleton variant="rect" width="96px" height="14px" />
        </div>
      </div>
      <div class="py-6">
        <div class="grid grid-cols-2 gap-x-4 gap-y-6">
          <div v-for="i in 2" :key="i" class="text-xs">
            <div class="flex gap-2 items-center mb-2">
              <UiSkeleton variant="rect" width="18px" height="18px" />
              <UiSkeleton variant="text" width="60px" height="12px" />
            </div>
            <UiSkeleton variant="rect" width="80px" height="14px" class="ms-7" />
          </div>
        </div>
      </div>
      <div class="p-4 rounded-xl bg-bg-subtle mb-2">
        <div class="flex items-center gap-2">
          <div class="flex-1 flex items-center gap-2">
            <UiSkeleton variant="rect" width="36px" height="36px" />
            <div class="flex-1">
              <UiSkeleton variant="rect" width="70%" height="14px" class="mb-1" />
              <UiSkeleton variant="text" width="40%" height="12px" />
            </div>
          </div>
          <div class="flex items-center gap-2">
            <UiSkeleton v-for="i in 3" :key="i" variant="rect" width="40px" height="40px" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
