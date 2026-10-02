<script setup>
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";
import { buildImageUrl } from "~/services/help";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "individual"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'تفاصيل الوظيفة',
})

const breadcrumbs = computed(() => [
  {
    label: "استكشف الوظائف",
    to: "/dashboard/jobs-explore",
    active: true,
  },
  {
    label: job?.title || 'الوظيفة',
    active: false,
  },
]);

const applyDialog = ref(false);
const loading = ref(false);
const error = ref(null);
const job = ref(null);
const route = useRoute();

const logoSrc = computed(() => buildImageUrl(job.value?.entityLogo, '/images/partner-3.svg'));

function onApplied() {
  if (job.value) job.value.isApplied = true;
}

onMounted(async () => {
  loading.value = true;
  error.value = null;
  const jobId = route.query.id || route.params.id || 1
  try {
    const { data, error } = await useApi().get(`/jobs/${jobId}`);
    if (error) {
      error.value = error;
      useToast().show(error, "error");
      return;
    }
    if (data) job.value = data;
  } catch (e) {
    error.value = e?.message || 'حدث خطأ أثناء تحميل بيانات الوظيفة';
    useToast().show("حدث خطأ أثناء تحميل بيانات الوظيفة", "error");
  } finally {
    loading.value = false;
  }
});
</script>
<template>
  <div class="px-4 lg:px-0">
    <Breadcrumbs :items="breadcrumbs" />
    <UiLoadingSkeleton v-if="loading" :count="1" height="400px" rounded="2xl" />
    <UiErrorState v-else-if="error" :message="error" @retry="onMounted" />
    <UiEmptyState v-else-if="!job" title="الوظيفة غير موجودة" description="لم نتمكن من العثور على الوظيفة المطلوبة" />
    <template v-else>
    <!--  -->
    <div class="grid grid-cols-7 gap-6 items-start py-4 mb-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8">
          <div class="flex flex-col gap-6">
            <h1 class="job-title text-lg lg:text-xl font-bold text-dark">
              {{ job?.title }}
            </h1>
            <p class="job-desc text-sm text-dark/70 leading-[2]">
              {{ job?.description }}
            </p>

            <div class="flex items-center gap-2">
              <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
                <img :src="logoSrc" class="w-10 h-6 object-cover" />
              </span>
              <span class="company-name text-sm text-dark">
                {{ job?.entityName }}
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
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                مميزات خاصة
              </h3>
              <ul v-if="job?.benefits?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="benefit in job.benefits" :key="benefit" class="text-sm text-muted mb-4">
                  {{ benefit }}
                </li>
              </ul>
            </div>
          </template>

          <template #conditions>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                شروط القبول
              </h3>
              <ul v-if="job?.conditions?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="condition in job.conditions" :key="condition" class="text-sm text-muted mb-4">
                  {{ condition }}
                </li>
              </ul>
            </div>
          </template>

          <template #tasks>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                المهام والمسؤوليات
              </h3>
              <ul v-if="job?.responsibilities?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="resp in job.responsibilities" :key="resp" class="text-sm text-muted mb-4">
                  {{ resp }}
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
                {{ job?.location }}
              </span>
          </div>
          <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="21" height="20" />
              </span>
              <span class="text-icon-muted text-xs"> {{ job?.hours }} </span>
          </div>
          <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="21" height="20" />
              </span>
              <span class="text-icon-muted text-xs">
                {{ job?.duration }}
              </span>
          </div>
          <div class="flex gap-2 items-center">
            <span>
              <MoneyIcon />
            </span>
            <span class="text-xs text-icon-muted"> {{ job?.salary }} </span>
          </div>
          <div class="flex gap-2 items-center">
              <span>
                <PersonIcon width="20" height="20" color="#696C68" />
              </span>
              <span class="text-xs text-icon-muted">
                {{ job?.gender === 'male' ? 'الذكور فقط' : job?.gender === 'female' ? 'الإناث فقط' : 'رجال ونساء' }}
              </span>
          </div>
        </div>
        <div class="mt-4">
          <div
            v-if="job.isApplied"
            class="mx-auto text-center text-sm w-full max-w-[300px] px-4 py-2 rounded-full bg-[#E7F6EC] text-[#1D9A4E] font-medium"
          >
            تم التقديم على هذه الوظيفة
          </div>
          <button
            v-else
            class="btn-primary mx-auto text-sm w-full max-w-[300px]"
            @click="applyDialog = true"
          >
            قدم الآن
          </button>
        </div>
      </div>
    </div>

    <!--  -->
    <ApplyJobDialog v-model="applyDialog" :job-id="job?.id" @applied="onApplied" />
  </template>
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
