<template>
  <div class="job-page max-wrapper mx-auto">
    <section class="hero-section mb-[160px] sm:mb-[80px]" id="jobs">
      <div class="section-content">
        <div class="box flex flex-col justify-between px-4 lg:px-0">
          <div class="max-w-[740px] mx-auto mb-24">
            <div class="text-content text-center text-white">
              <h1
                class="text-3xl md:text-4x1 lg:text-[50px] font-bold leading-[1.4] mb-5"
              >
                فرص موسمية تخدم ضيوف الرحمن – وظيفتك في انتظارك
              </h1>
              <p class="text-sm md:text-lg leading-[1.6]">
                اكتشف الوظائف الموسمية المتاحة في موسم الحج والعمرة، وقدّم على
                الفرص التي تناسب مهاراتك وخبراتك، بخطوات بسيطة وسريعة.
              </p>
            </div>
          </div>

          <!--  -->
          <div
            class="job-filter w-full max-w-[1180px] mx-auto bg-white p-4 md:p-6 lg:p-10 rounded-xl lg:rounded-3xl"
          >
            <div
              class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 w-full"
            >
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  نوع الوظيفة <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="typeOptions"
                  placeholder="اختر"
                  v-model="filters.type"
                  key="select-1"
                />
              </div>

              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  المنطقة <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="locationOptions"
                  placeholder="اختر"
                  v-model="filters.location"
                  key="select-2"
                />
              </div>

              <!-- Gender -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  الجنس <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="genderOptions"
                  placeholder="اختر"
                  v-model="filters.gender"
                  key="select-3"
                />
              </div>

              <!-- Employer -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  الجهة الموظفة <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="filterOptions.entities"
                  placeholder="اختر"
                  v-model="filters.entityId"
                  key="select-4"
                />
              </div>

              <!-- Search Button -->
              <div
                class="w-full mt-4 lg:mt-0 sm:col-span-2 lg:col-span-1 flex items-end justify-center"
              >
                <button
                  class="btn-primary text-sm h-[42px]"
                  @click="applyFilters"
                >
                  البحث عن وظيفة
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section class="jobs-container px-4 sm:px-3 xl:px-0">
      <div class="py-8">
        <div
          class="flex flex-col sm:flex-row items-start justify-between gap-8"
        >
          <div>
            <h2 class="text-lg font-semibold mb-3">
              عرض {{ items.length }} نتيجة وظيفة
            </h2>
            <p class="text-sm text-muted">بناءً على ملفك الشخصي وتفضيلاتك</p>
          </div>
          <button class="self-end btn-outline text-sm gap-2">
            <SortBars />
            <span> بحلول تاريخ الإغلاق </span>
          </button>
        </div>
      </div>
      <UiLoadingSkeleton v-if="loading" :count="12" :columns="3" height="380px" rounded="xl" />
      <UiErrorState v-else-if="error" :message="error" @retry="retry" />
      <UiEmptyState
        v-else-if="!items.length"
        title="لا توجد نتائج"
        description="لم يتم العثور على وظائف تطابق معايير البحث"
      />
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <JobCard
          v-for="item in items"
          :key="item.id"
          :job="item"
          @open-apply-form="openApplyForm"
        />
      </div>

      <div class="pt-6">
        <Pagination
          :current-page="page"
          :total-pages="totalPages"
          :per-page="perPage"
          @page-changed="goToPage"
          @per-page-change="onPerPageChange"
        />
      </div>
    </section>

    <!--  -->
    <ApplyJobDialog
      v-model="applyJobDialog"
      :job-id="selectedJobId"
      v-if="isAuthenticated"
      @applied="markJobApplied"
    />
  </div>
</template>

<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import Pagination from "~/components/Pagination.vue";
import { jobGenderLabel } from "~/services/analyticsLabels";
import { cityLabel, workTypeLabel } from "~/services/jobLabels";
definePageMeta({
  middleware: [],
});
//
useHead({
  title: "الوظائف الموسمية",
  meta: [
    {
      name: "description",
      content:
        "تصفح الوظائف الموسمية المتاحة في موسم الحج والعمرة، وقدّم على الفرص التي تناسب مهاراتك وخبراتك.",
    },
    { property: "og:title", content: "الوظائف الموسمية | منصة تشريف" },
    {
      property: "og:description",
      content:
        "اكتشف الوظائف الموسمية المتاحة في موسم الحج والعمرة، وقدّم على الفرص التي تناسب مهاراتك وخبراتك.",
    },
    { property: "og:type", content: "website" },
    { property: "og:image", content: "/og-image.svg" },
    { name: "twitter:card", content: "summary" },
  ],
});

useScrollSpy();

const { isAuthenticated } = useAuth();
const { showModal, closeModal } = useLoginModal();
const applyJobDialog = ref(false);
const selectedJobId = ref(null);
const openApplyForm = (jobId) => {
  if (isAuthenticated.value) {
    selectedJobId.value = jobId;
    applyJobDialog.value = true;
    return;
  }
  showModal();
};
const loading = ref(true);
const error = ref(null);
const items = ref([]);

function markJobApplied(jobId) {
  const item = items.value.find(j => j.id === jobId)
  if (item) item.isApplied = true
}

const filterOptions = ref({
  workTypes: [],
  locations: [],
  genders: [],
  entities: [],
});

const filters = ref({
  type: "",
  location: "",
  gender: "",
  entityId: "",
});

const genderOptions = computed(() =>
  (filterOptions.value.genders || []).map((g) =>
    typeof g === "string" ? { value: g, label: jobGenderLabel(g) } : g
  )
);

const typeOptions = computed(() =>
  (filterOptions.value.workTypes || []).map((t) =>
    typeof t === "string" ? { value: t, label: workTypeLabel(t) } : t
  )
);

const locationOptions = computed(() =>
  (filterOptions.value.locations || []).map((l) =>
    typeof l === "string" ? { value: l, label: cityLabel(l) } : l
  )
);

const { page, perPage, total, totalPages, goToPage, onPerPageChange } =
  usePagination({ perPage: 9, onChange: () => refreshJobs() });

// SSR rule: loaders RETURN data (payload transfers return values only — ref
// writes inside a loader never reach the client). One apply step then writes
// refs identically on server and client. Same endpoints, same states.
async function loadFilterOptions() {
  try {
    const { data, error } = await useApi().get("/jobs/filter-options");
    if (error) return { options: null, error };
    return { options: data, error: null };
  } catch (err) {
    console.error("Failed to load filter options:", err);
    return { options: null, error: null };
  }
}

async function loadJobsList() {
  const params = { page: page.value, limit: perPage.value };
  if (filters.value.type) params.type = filters.value.type;
  if (filters.value.location) params.location = filters.value.location;
  if (filters.value.gender) params.gender = filters.value.gender;
  if (filters.value.entityId) params.entityId = filters.value.entityId;

  try {
    const { data, error } = await useApi().get("/jobs", params);
    if (error) return { items: [], total: 0, error };
    return { items: data?.items || [], total: data?.total || 0, error: null };
  } catch (err) {
    return { items: [], total: 0, error: err?.message || "حدث خطأ في تحميل الوظائف" };
  }
}

function applyFilters() {
  page.value = 1;
  refreshJobs();
}

// Filters init from the URL — moved from onMounted to setup so SSR honors
// shared links (e.g. /jobs?type=full-time). Same values on client mounts.
const q = useRoute().query
if (q.type) filters.value.type = String(q.type)
if (q.location) filters.value.location = String(q.location)
if (q.gender) filters.value.gender = String(q.gender)
if (q.entityId) filters.value.entityId = isNaN(Number(q.entityId)) ? String(q.entityId) : Number(q.entityId)

// SSR-first (no damage): the SAME two fetches run blocking in setup, so SSR HTML
// already contains options + list. Interactions just re-run the list via
// refreshJobs() — identical states (loading/error/items), identical template.
const [{ data: filterOptionsData }, { data: jobsData, refresh: refreshJobsData }] =
  await Promise.all([
    useAsyncData('public-jobs-filter-options', () => loadFilterOptions()),
    useAsyncData('public-jobs-list', () => loadJobsList()),
  ])

function applySsrState() {
  if (filterOptionsData.value?.options) filterOptions.value = filterOptionsData.value.options
  items.value = jobsData.value?.items || []
  total.value = jobsData.value?.total || 0
  error.value = jobsData.value?.error || null
  loading.value = false
  if (import.meta.client) {
    if (filterOptionsData.value?.error) useToast().show(filterOptionsData.value.error, "error")
    if (jobsData.value?.error) useToast().show(jobsData.value.error, "error")
  }
}
applySsrState()

async function refreshJobs() {
  loading.value = true
  await refreshJobsData()
  applySsrState()
}

function retry() {
  window.location.reload();
}
</script>
<style lang="scss" scoped>
.job-page {
  .hero-section {
    .section-content {
      min-height: 610px;
      background:
        linear-gradient(
          179.24deg,
          rgba(0, 0, 0, 0.7) 12.77%,
          rgba(0, 0, 0, 0) 139.66%
        ),
        url("~/assets/images/jobs-hero-section.png");
      background-repeat: no-repeat;
      background-size: cover;
      background-position: center;
      border-radius: 40px;
    }
    .box {
      position: relative;
      top: 130px;
      min-height: 550px;
      .job-filter {
        box-shadow: 0px 8px 36px rgba(17, 17, 17, 0.06);
      }
    }
  }
}
</style>
