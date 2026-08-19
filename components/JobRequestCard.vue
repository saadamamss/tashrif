<template>
  <div class="job-card border bg-white rounded-3xl overflow-hidden relative p-6 lg:p-8" v-bind="$attrs">
    <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
      :x-status="props.application?.status">
      <span class="text-xs">{{ statusLabel }}</span>
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
          <img :src="job.entityLogo || '/images/partner-3.svg'" class="w-10 h-6 object-cover" />
        </span>
        <span class="company-name text-sm text-dark">
          {{ job.entityName }}
        </span>
      </div>
    </div>
    <div class="flex gap-4 mt-6">
      <div class="flex-1 text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <CalenderIcon width="20" height="20" />
          </span>
          <span class="text-muted">تاريخ التقديم</span>
        </div>
        <div class="font-bold ps-7">{{ formatDate(application.createdAt) }}</div>
      </div>

      <div class="flex-1 text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <ClockIcon width="20" height="20" />
          </span>
          <span class="text-muted">آخر تحديث</span>
        </div>
        <div class="font-bold ps-7">{{ daysSince }}</div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { formatDate } from "~/services/help.js";
import CalenderIcon from "./icons/calender.vue";
import ClockIcon from "./icons/clock.vue";
import { computed } from "vue";

const props = defineProps({
  application: { type: Object, default: () => ({}) },
  job: { type: Object, default: () => ({}) },
})

const job = computed(() => props.job?.id ? props.job : (props.application?.job || {}))

const statusLabel = computed(() => {
  const labels = { new: 'قيد المراجعة', shortlisted: 'مقبول مبدئي', interview: 'مقابلة', contract_sent: 'تم إرسال العقد', accepted: 'مقبول', refused: 'مرفوض' }
  return labels[props.application.status] || 'قيد المراجعة'
})
const daysSince = computed(() => {
  if (!props.application.createdAt) return '-'
  const days = Math.floor((Date.now() - new Date(props.application.createdAt).getTime()) / (1000 * 60 * 60 * 24))
  return 'منذ ' + days + ' يوم'
})

</script>
<style lang="scss" scoped>
.job-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
