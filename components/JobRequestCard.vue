<template>
  <div class="job-card border bg-white rounded-3xl overflow-hidden relative p-6 lg:p-8" v-bind="$attrs">
    <div class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100" x-status="waiting">
       <span class="text-xs"> قيد المراجعة</span>
    </div>
    <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
      <h1 class="job-title text-base font-bold text-[#161614]">
        مشرف تنظيم حشود
      </h1>
      <p class="job-desc text-xs text-[#161614]/70">
        فرصة مميزة للعمل الموسمي ضمن فريق متخصص في تنظيم الحشود وخدمة ضيوف
        الرحمن.
      </p>

      <div class="flex items-center gap-2">
        <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
          <img src="/images/partner-3.svg" class="w-10 h-6 object-cover" />
        </span>
        <span class="company-name text-sm text-[#161614]">
          شركة نسك لخدمات الحجاج
        </span>
      </div>
    </div>
    <div class="flex gap-4 mt-6">
      <div class="flex-1 text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <CalenderIcon width="20" height="20" />
          </span>
          <span class="text-[#667178]">تاريخ التقديم</span>
        </div>
        <div class="font-bold ps-7">10 مايو 2025</div>
      </div>

      <div class="flex-1 text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <ClockIcon width="20" height="20" />
          </span>
          <span class="text-[#667178]">آخر تحديث</span>
        </div>
        <div class="font-bold ps-7">اليوم</div>
      </div>
    </div>
  </div>
</template>
<script setup>
import CalenderIcon from "./icons/calender.vue";
import ClockIcon from "./icons/clock.vue";

</script>
<style lang="scss" scoped>
.job-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
