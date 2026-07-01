<template>
  <div class="job-card border bg-white rounded-2xl p-6 lg:p-8" v-bind="$attrs">
    <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
      <h1 class="job-title text-base font-bold text-dark">
        مشرف تنظيم حشود
      </h1>
      <p class="job-desc text-xs text-dark/70">
        فرصة مميزة للعمل الموسمي ضمن فريق متخصص في تنظيم الحشود وخدمة ضيوف
        الرحمن.
      </p>

      <div class="flex items-center gap-2">
        <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
          <img src="/images/partner-3.svg" class="w-10 h-6 object-cover" />
        </span>
        <span class="company-name text-sm text-dark">
          شركة نسك لخدمات الحجاج
        </span>
      </div>
    </div>
    <div class="details py-6 flex flex-col gap-4">
      <div class="flex gap-2 items-center">
        <span>
          <Location />
        </span>
        <span class="text-icon-muted text-xs">
          مكة المكرمة – المنطقة المركزية
        </span>
      </div>
      <div class="flex gap-2 items-center">
        <span>
          <CalenderIcon width="21" height="20" />
        </span>
        <span class="text-icon-muted text-xs"> دوام كامل – 8 ساعات </span>
      </div>
      <div class="flex gap-2 items-center">
        <span>
          <CalenderIcon width="21" height="20" />
        </span>
        <span class="text-icon-muted text-xs">
          10 أيام (من 1 ذو الحجة حتى 10 ذو الحجة)
        </span>
      </div>
    </div>

    <div class="flex gap-4">
      <nuxt-link
        :to="{
          path: '/dashboard/jobs-explore/details',
          query: { from: $route.path },
        }"
        class="flex-1 text-center btn-outline text-sm"
      >
        عرض التفاصيل
      </nuxt-link>

      <button
        @click="$emit('openApplyForm')"
        class="flex-1 text-center btn-primary text-sm"
      >
        قدم الآن
      </button>
    </div>
  </div>
</template>
<script setup>
/** @type {import('~/types/job').Job} */
defineProps({
  job: {
    type: Object,
    default: () => ({}),
  },
})

defineEmits(['openApplyForm'])
</script>
<style lang="scss" scoped>
.job-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
