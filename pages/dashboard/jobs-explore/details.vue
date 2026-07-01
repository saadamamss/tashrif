<script setup>
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";
import JobCard from "~/components/JobCard.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global", "auth-guard", "individual"],
});
const breadcrumbs = [
  {
    label: "استكشف الوظائف",
    to: "/dashboard/jobs-explore",
    active: true,
  },

  {
    label: "مشرف حجاج",
    active: false,
  },
];

const applyDialog = ref(false);
const loading = ref(false);

onMounted(async () => {
  loading.value = true;
  try {
    await useApi().get('/jobs/1');
  } finally {
    loading.value = false;
  }
});
</script>
<template>
  <div class="px-4 lg:px-0">
    <Breadcrumbs :items="breadcrumbs" />
    <LoadingSkeleton v-if="loading" :rows="1" height="400px" rounded="2xl" />
    <template v-else>
    <!--  -->
    <div class="grid grid-cols-7 gap-6 items-start py-4 mb-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8">
          <div class="flex flex-col gap-6">
            <h1 class="job-title text-lg lg:text-xl font-bold text-dark">
              مشرف حجاج
            </h1>
            <p class="job-desc text-sm text-dark/70 leading-[2]">
              تبحث شركة الإسناد الموسمي لخدمات الحجاج عن أفراد مؤهلين للانضمام
              إلى فريقها كمشرفين ميدانيين خلال موسم الحج. ستكون مسؤولاً عن تنظيم
              وإرشاد مجموعة من الحجاج أثناء تنقلهم بين المشاعر المقدسة، وضمان
              التزامهم بالتعليمات والخطط التشغيلية.
            </p>

            <div class="flex items-center gap-2">
              <span
                class="company-logo border rounded-md overflow-hidden py-1 px-2"
              >
                <img
                  src="/images/partner-3.svg"
                  class="w-10 h-6 object-cover"
                />
              </span>
              <span class="company-name text-sm text-dark">
                شركة نسك لخدمات الحجاج
              </span>
            </div>
          </div>
        </div>

        <!--  -->
        <CustomTabs
          :tabs="[
            { id: 'benefits', title: 'المزايا والمكافأة' },
            { id: 'conditions', title: 'شروط القبول' },
            { id: 'tasks', title: 'المهام والمسؤوليات' },
          ]"
          initial-tab="benefits"
        >
          <!-- Named slots for each tab content -->
          <template #benefits>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                مميزات خاصة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-muted mb-4">
                  الإشراف اليومي على مجموعة محددة من الحجاج.
                </li>
                <li class="text-sm text-muted mb-4">
                  التأكد من التزام الحجاج بخطط التنقل وجدول الحركة بين المشاعر.
                </li>
                <li class="text-sm text-muted mb-4">
                  التنسيق المستمر مع فرق النقل والدعم اللوجستي.
                </li>
                <li class="text-sm text-muted mb-4">
                  التعامل مع الحالات الطارئة ورفع التقارير إلى المسؤول المباشر.
                </li>
                <li class="text-sm text-muted mb-4">
                  ضمان سلامة وراحة الحجاج خلال تنقلهم وإقامتهم.
                </li>
              </ul>
            </div>
          </template>

          <template #conditions>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                شروط القبول
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-muted mb-4">
                  أن يكون المتقدم سعودي الجنسية.
                </li>
                <li class="text-sm text-muted mb-4">
                  العمر بين 22 و45 سنة.
                </li>
                <li class="text-sm text-muted mb-4">
                  القدرة على العمل الميداني المكثف لساعات طويلة.
                </li>
                <li class="text-sm text-muted mb-4">
                  يفضّل من لديه خبرة سابقة في العمل الموسمي أو الإشراف الميداني.
                </li>
                <li class="text-sm text-muted mb-4">
                  الالتزام بالأخلاقيات المهنية والسلوكيات المناسبة.
                </li>
                <li class="text-sm text-muted mb-4">
                  الأولوية لسكان منطقة مكة المكرمة لتسهيل التنقل السريع.
                </li>
              </ul>
            </div>
          </template>

          <template #tasks>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                المزايا والمكافأة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-muted mb-4">
                  مكافأة مقطوعة قدرها 3000 ريال سعودي.
                </li>
                <li class="text-sm text-muted mb-4">
                  شهادة خبرة بعد انتهاء المهمة بنجاح.
                </li>
                <li class="text-sm text-muted mb-4">
                  تشمل بدل السكن والتنقل.
                </li>
              </ul>
            </div>
          </template>
        </CustomTabs>
      </div>

      <div
        class="col-span-7 lg:col-span-3 xl:col-span-2 bg-white rounded-xl p-6"
      >
        <div class="pb-4 border-b-2">
          <h3 class="text-lg font-medium">تفاصيل الوظيفة</h3>
        </div>
        <div class="space-y-4 py-4 border-b-2">
          <div class="flex gap-2 items-center">
              <span>
                <Location width="20" height="20" />
              </span>
              <span class="text-xs text-icon-muted">
                مكة المكرمة – المشاعر المقدسة (منى – مزدلفة – عرفات).
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
            <span class="text-icon-muted text-xs">
              10 أيام (من 1 ذو الحجة حتى 10 ذو الحجة)
            </span>
          </div>
          <div class="flex gap-2 items-center">
            <span>
              <MoneyIcon />
            </span>
            <span class="text-xs text-icon-muted"> مرتب 3000 ريال سعودي </span>
          </div>
          <div class="flex gap-2 items-center">
              <span>
                <PersonIcon width="20" height="20" color="#696C68" />
              </span>
              <span class="text-xs text-icon-muted">
                الذكور فقط لهذه الوظيفة.
              </span>
          </div>
        </div>
        <div class="mt-4">
          <button
            class="btn-primary mx-auto text-sm w-full max-w-[300px]"
            @click="applyDialog = true"
          >
            قدم الآن
          </button>
        </div>
      </div>
    </div>

    <!--  -->
    <ApplyJobDialog v-model="applyDialog" />
  </template>
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
