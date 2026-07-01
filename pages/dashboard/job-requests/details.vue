<script setup>
import AddToCalendar from "~/components/AddToCalendar.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";
import Pdf from "~/components/icons/pdf.vue";
import SignContract from "~/components/SignContract.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global", "auth-guard", "individual"],
});

const breadcrumbs = [
  {
    label: "طلبات العمل",
    to: "/dashboard/job-requests",
    active: true,
  },

  {
    label: "مشرف حجاج",
    active: false,
  },
];

const signContractOpen = ref(false);
const currentStatus = ref("preliminary");
const requestStatus = ref({
  pending: "قيد المراجعة",
  preliminary: "قبول مبدئي",
  accepted: "مقبول",
});
</script>
<template>
  <div class="px-4 lg:px-0 mb-8">
    <Breadcrumbs :items="breadcrumbs" />
    <!--  -->
    <div class="grid grid-cols-7 gap-6 items-start mt-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div
          class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8 relative overflow-hidden"
        >
          <div
            class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
            :x-status="currentStatus"
          >
            <span class="text-xs">
              {{ requestStatus[currentStatus] }}
            </span>
          </div>
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

      <div class="col-span-7 lg:col-span-3 xl:col-span-2 space-y-6">
        <!-- status -->
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div v-if="currentStatus == 'pending'">
            <div class="pb-4 border-b-2">
              <h3 class="text-lg font-medium">حالة الطلب</h3>
            </div>
            <div class="space-y-4 pt-4">
              <div class="p-4 rounded-xl badge" x-status="pending">
                <h3 class="text-sm mb-3">قيد المراجعة</h3>
                <p class="text-xs text-muted">
                  نحن بانتظار رد الجهة المعلنة. سيتم إشعارك فور تحديث الحالة.
                </p>
              </div>
              <p class="text-xs text-muted">
                تم التقديم في تاريخ: 7 يوليو 2025
              </p>
            </div>
          </div>
          <div v-if="currentStatus == 'preliminary'">
            <div class="pb-4 border-b-2">
              <h3 class="text-lg font-medium">حالة الطلب</h3>
            </div>
            <div class="space-y-4 pt-4">
              <div class="p-4 rounded-xl badge" x-status="preliminary">
                <h3 class="text-sm mb-3">تم القبول المبدئي</h3>
                <p class="text-xs text-muted">
                  🎉 تهانينا! لقد تم ترشيحك مبدئيًا لوظيفة مشرف حجاج.
                </p>
              </div>
              <p class="text-xs text-muted">
                تم التقديم في تاريخ: 7 يوليو 2025
              </p>
            </div>
          </div>
          <div v-if="currentStatus == 'accepted'">
            <div class="pb-4 border-b-2">
              <h3 class="text-lg font-medium">حالة الطلب</h3>
            </div>
            <div class="space-y-4 pt-4">
              <div class="p-4 rounded-xl badge" x-status="accepted">
                <h3 class="text-sm mb-3">تم القبول</h3>
                <p class="text-xs text-muted">
                  🎉 تهانينا، تم قبولك! لقد تم قبولك نهائيًا لوظيفة مشرف حجاج
                  ضمن فريق شركة الإسناد الموسمي لخدمات الحجاج.
                </p>
              </div>
              <p class="text-xs text-muted">
                تم التقديم في تاريخ: 7 يوليو 2025
              </p>
            </div>
          </div>
        </div>
        <!-- interview -->
        <div
          v-if="currentStatus == 'preliminary'"
          class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6"
        >
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">موعد المقابلة الشخصية</h3>
          </div>
          <div class="details py-6 flex flex-col gap-4">
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="18" height="18" />
              </span>
              <span class="text-icon-muted text-xs">
                الأربعاء 10 يوليو 2025
              </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <Clock />
              </span>
              <span class="text-icon-muted text-xs"> الساعة 10:00 صباحًا </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <Location />
              </span>
              <span class="text-icon-muted text-xs">
                حي العزيزية، مكة المكرمة
              </span>
            </div>
          </div>

          <div class="p-4 rounded-xl bg-bg-subtle mb-4">
            <h3 class="text-sm mb-3 text-surface">📌 ملاحظات مهمة</h3>
            <div class="text-xs text-muted">
              <p class="mb-2">يرجى الحضور قبل الموعد بـ15 دقيقة.</p>
              <p class="mb-2">إحضار أصل الهوية الوطنية والسيرة الذاتية.</p>
              <p class="">الالتزام بالزي الرسمي.</p>
            </div>
          </div>

          <div class="flex gap-4">
            <AddToCalendar />
          </div>
        </div>

        <!-- contract -->
        <div
          v-if="currentStatus == 'accepted'"
          class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6"
        >
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">العقد الوظيفى</h3>
          </div>
          <div class="details py-6 flex flex-col gap-4">
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="18" height="18" />
              </span>
              <span class="text-icon-muted text-xs">
                5 – 13 ذو الحجة 1446هـ
              </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <Location />
              </span>
              <span class="text-icon-muted text-xs">
                حي العزيزية، مكة المكرمة
              </span>
            </div>
          </div>

          <div class="p-4 rounded-xl bg-bg-subtle mb-2">
            <div class="flex justify-between gap-4 items-center">
              <div class="flex flex-wrap items-center gap-2">
                <span class="block p-2 bg-white rounded-xl">
                  <Pdf />
                </span>
                <div>
                  <span class="block text-slate-900 text-sm mb-1">
                    pdf السيرة الذاتية
                  </span>
                  <span class="text-xs block text-slate-400"> 1.2Mb </span>
                </div>
              </div>
              <div>
                <button class="block shadow-sm p-2 bg-white rounded-lg">
                  <Download />
                </button>
              </div>
            </div>
          </div>
          <p class="text-xs text-muted mb-4">
            يجب توقيع العقد قبل تاريخ 15 ذو القعدة 1446هـ لتأكيد انضمامك رسميًا.
          </p>

          <div class="flex gap-4">
            <button
              @click="signContractOpen = true"
              class="flex-1 text-center btn-primary text-sm"
            >
              توقيع العقد الإلكترونى
            </button>
          </div>
        </div>

        <!-- job details -->
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">تفاصيل الوظيفة</h3>
          </div>
          <div class="space-y-4 pt-4">
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
        </div>
      </div>
    </div>

    <!--  -->
    <SignContract v-model="signContractOpen" />
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
