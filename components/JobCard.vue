<template>
  <div class="job-card" v-bind="$attrs">
    <div class="relative overflow-hidden border bg-white rounded-2xl p-6 lg:p-8">
      <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
        v-if="job.isApplied" x-status="applied">
        <span class="text-xs">تم التقديم</span>
      </div>

      <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
        <h1 class="job-title text-base font-bold text-dark">
          {{ job.title }}
        </h1>
        <p class="job-desc text-xs text-dark/70">
          {{ job.description }}
        </p>

        <div class="flex items-center gap-2">
          <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
            <img :src="entityLogo" class="w-10 h-6 object-cover" />
          </span>
          <span class="company-name text-sm text-dark">
            {{ job.entityName }}
          </span>
        </div>
      </div>
      <div class="details py-6 flex flex-col gap-4">
        <div class="flex gap-2 items-center">
          <span>
            <Location />
          </span>
          <span class="text-icon-muted text-xs">
            {{ cityLabel(job.location) }}
          </span>
        </div>
        <div class="flex gap-2 items-center">
          <span>
            <CalenderIcon width="21" height="20" />
          </span>
          <span class="text-icon-muted text-xs"> {{ job.hours }} </span>
        </div>
        <div class="flex gap-2 items-center">
          <span>
            <CalenderIcon width="21" height="20" />
          </span>
          <span class="text-icon-muted text-xs">
            {{ job.duration }}
          </span>
        </div>
      </div>

      <div class="flex gap-4">
        <nuxt-link :to="{
          path: '/dashboard/jobs-explore/details',
          query: { id: job.id, from: $route.path },
        }" class="flex-1 text-center btn-outline text-sm">
          عرض التفاصيل
        </nuxt-link>

        <button v-if="!job.isApplied" @click="$emit('openApplyForm', job.id)"
          class="flex-1 text-center btn-primary text-sm">
          قدم الآن
        </button>
      </div>
    </div>
  </div>
</template>
<script setup>
import { buildImageUrl } from '~/services/help';
import { cityLabel } from '~/services/jobLabels';

/** @type {import('~/types/job').Job} */
const props = defineProps({
  job: {
    type: Object,
    default: () => ({}),
  },
})

defineEmits(['openApplyForm'])

const entityLogo = computed(() => buildImageUrl(props.job.entityLogo, '/images/partner-3.svg'));

</script>
<style lang="scss" scoped>
.job-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
