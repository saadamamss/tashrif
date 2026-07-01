<template>
  <div
    class="job-card border bg-white rounded-3xl overflow-hidden relative p-6 lg:p-8"
  >
    <div
      class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
      x-status="published"
    >
      <span class="text-xs"> نشرت </span>
    </div>
    <div class="border-b-2 border-[#E9F1F2] flex flex-col gap-4 pb-6">
      <h1 class="job-title text-base lg:text-lg font-semibold text-dark">
        مشرف تنظيم حشود
      </h1>
      <p class="job-desc text-xs text-dark/70 leading-[2.4]">
        فرصة مميزة للعمل الموسمي ضمن فريق متخصص في تنظيم الحشود وخدمة ضيوف
        الرحمن.
      </p>
    </div>
    <div class="grid grid-cols-2 gap-x-4 gap-y-6 mt-6">
      <div class="text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <BriefcaseIcon width="21" height="16" />
          </span>
          <span class="text-muted">متقدم</span>
        </div>
        <div class="font-bold ps-7">58</div>
      </div>
        <div class="text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <ClockIcon width="20" height="20" />
          </span>
          <span class="text-muted">آخر تحديث</span>
        </div>
        <div class="font-bold ps-7">اليوم</div>
      </div>
      <div class="text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <CalenderIcon width="20" height="20" />
          </span>
          <span class="text-muted">تاريخ النشر</span>
        </div>
        <div class="font-bold ps-7">10 مايو 2025</div>
      </div>
      <div class="text-xs">
        <div class="flex gap-2 items-center mb-2">
          <span>
            <CalenderIcon width="20" height="20" />
          </span>
          <span class="text-muted">تاريخ الإنتهاء</span>
        </div>
        <div class="font-bold ps-7">30 مايو 2025</div>
      </div>
    </div>
  </div>
</template>
<script setup></script>
<style lang="scss" scoped>
.job-card {
  box-shadow: 2.4px 12.8px 35.2px rgba(7, 15, 66, 0.05);
}
</style>
