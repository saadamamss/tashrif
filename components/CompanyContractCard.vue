<template>
  <div class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl px-4 py-6 sm:p-6">
    <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
      x-status="secondary">
      <span class="text-xs">{{ statusLabel }}</span>
    </div>
    <div class="pb-4 border-b-2">
      <div class="flex items-center gap-2">
        <span class="company-logo w-12 h-12 rounded-full relative overflow-hidden">
          <img src="~/assets/images/avatar-1.png" class="w-full h-full object-cover" />
          <img :src="contract.userAvatar" class="w-full h-full object-cover" />
        </span>
        <span class="company-name font-medium text-sm text-dark">
          {{ contract.userName }}
        </span>
      </div>
    </div>
    <div class="details py-6">
      <div class="grid grid-cols-2 gap-x-4 gap-y-6">
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <CalenderIcon width="18" height="18" />
            </span>
            <span class="text-muted">تاريخ الإرسال</span>
          </div>
          <div class="font-bold ps-7">{{ formatDate(contract.createdAt) }}</div>
        </div>
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <BriefcaseIcon width="18" height="18" />
            </span>
            <span class="text-muted">الوظيفة</span>
          </div>
          <div class="font-bold ps-7">{{ contract.jobTitle }}</div>
        </div>
      </div>
    </div>
    <div class="p-4 rounded-xl bg-bg-subtle mb-2">
      <div class="flex items-center gap-2">
        <div class="flex-1 flex items-center gap-2 min-w-0">
          <span class="p-2 bg-white rounded-xl shrink-0">
            <Pdf />
          </span>
          <div class="flex-1 min-w-0">
            <span class="block text-slate-900 text-sm mb-1 truncate">
              {{ contract.fileName }}
            </span>
            <span class="text-xs block text-slate-400">
              {{ contract.fileSize ? Math.round(contract.fileSize / 1024) + 'Mb' : '' }}
            </span>
          </div>
        </div>
        <div class="flex items-center gap-2 shrink-0">
          <button class="block shadow-sm w-10 h-10 p-3 bg-white rounded-lg">
            <Download />
          </button>

          <button class="block shadow-sm w-10 h-10 p-3 bg-white rounded-lg" @click="$emit('showContract', contract)">
            <EyeIcon />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";
import { formatDate } from "~/services/help";

const props = defineProps({
  contract: { type: Object, default: () => ({}) },
})
const statusLabel = computed(() => {
  const labels = { sent: 'تم إرسال العقد', signed: 'تم التوقيع', cancelled: 'ملغي' }
  return labels[props.contract.status] || 'تم إرسال العقد'
})
</script>
