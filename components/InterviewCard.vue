<template>
  <div class="interview-card border bg-white relative overflow-hidden rounded-2xl p-6 lg:p-8" v-bind="$attrs">
    <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
      x-status="coming">
      <span class="text-xs">{{ statusLabel }}</span>
    </div>
    <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
      <div class="flex items-center gap-2">
        <span class="company-logo border-2 rounded-md overflow-hidden py-1 px-2">
          <img :src="logoSrc" class="w-10 h-6 object-cover" />
        </span>
        <span class="company-name font-medium text-sm text-dark">
          {{ interview.entityName }}
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
    </div>
  </div>
</template>
<script setup>
import { buildImageUrl, formatDate, formatTime } from "~/services/help.js";
import Calender from "./icons/calender.vue";
import Clock from "./icons/clock.vue";
import Location from "./icons/location.vue";
import { computed } from "vue";

const props = defineProps({
  interview: { type: Object, default: () => ({}) },
})

const logoSrc = computed(() => buildImageUrl(props.interview?.entityLogo, '/images/partner-3.svg'))

const statusLabel = computed(() => {
  const labels = { completed: 'منتهية', upcoming: 'قادمة', past: 'فات موعدها' }
  return labels[props.interview.status] || 'قادمة'
})
</script>
<style lang="scss" scoped>
.interview-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
