<template>
  <div class="interview-card border bg-white relative overflow-hidden rounded-2xl p-6 lg:p-8">
    <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
      x-status="coming">
      <span class="text-xs">{{ statusLabel }}</span>
    </div>
    <div class="pb-4 border-b-2">
      <div class="flex items-center gap-2">
        <span class="company-logo w-10 h-10 rounded-full relative overflow-hidden">
          <img  :src="logoSrc" class="w-full h-full object-cover" />
        </span>
        <span class="company-name font-medium text-sm text-dark">
          {{ interview.userName }}
        </span>
      </div>
    </div>
    <div class="details py-6 flex flex-col gap-4">
      <div class="flex gap-2 items-center">
        <span>
          <Calender />
        </span>
        <span class="text-icon-muted text-xs"> {{ formatDate(interview.date) }} </span>
      </div>
      <div class="flex gap-2 items-center">
        <span>
          <Clock />
        </span>
        <span class="text-icon-muted text-xs"> {{ formatTime(interview.time) }} </span>
      </div>
      <div class="flex gap-2 items-center">
        <span>
          <Location />
        </span>
        <span class="text-icon-muted text-xs"> {{ interview.location }} </span>
      </div>
    </div>

    <div class="flex gap-4">

      <AddToCalendar />

      <button v-if="interview?.finished"
        class="flex-1 py-3 px-3 text-sm rounded-full bg-danger text-white disabled:text-danger disabled:bg-danger/10 hover:bg-danger/85 transition">
        لم يحضر
      </button>

      <button v-if="interview?.finished"
        class="flex-1 py-3 px-3 text-sm rounded-full bg-success text-white disabled:text-success disabled:bg-success/10 hover:bg-success/85 transition">
        حضر
      </button>
    </div>
  </div>
</template>
<script setup>
import { buildImageUrl, formatDate, formatTime } from '~/services/help.js';
import AddToCalendar from './AddToCalendar.vue';

import { computed } from "vue";

/** @type {{ interview: import('~/types/interview').Interview|null }} */
const props = defineProps({
  interview: {
    type: Object,
    default: null,
  },
});

const logoSrc = computed(()=> buildImageUrl(props.interview?.userAvatar, ""))
const statusLabel = computed(() => {
  const labels = { completed: 'منتهية', upcoming: 'قادمة', past: 'فات موعدها' }
  return labels[props.interview?.status] || 'قادمة'
})
</script>
<style lang="scss" scoped>
.interview-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
