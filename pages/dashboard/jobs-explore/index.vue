<script setup>
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import JobCard from "~/components/JobCard.vue";
import Pagination from "~/components/Pagination.vue";
import { jobGenderLabel } from "~/services/analyticsLabels";
import { cityLabel, workTypeLabel } from "~/services/jobLabels";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "individual"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'استكشف الوظائف',
})

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};

const applyDialog = ref(false);
const selectedJobId = ref(null);
const loading = ref(false);
const error = ref(null);
const items = ref([]);

const filterOptions = ref({
  workTypes: [],
  locations: [],
  genders: [],
  entities: [],
})

const filters = ref({
  type: '',
  location: '',
  gender: '',
  entityId: '',
})

const genderOptions = computed(() =>
  (filterOptions.value.genders || []).map((g) =>
    typeof g === 'string' ? { value: g, label: jobGenderLabel(g) } : g
  )
)

const typeOptions = computed(() =>
  (filterOptions.value.workTypes || []).map((t) =>
    typeof t === 'string' ? { value: t, label: workTypeLabel(t) } : t
  )
)

const locationOptions = computed(() =>
  (filterOptions.value.locations || []).map((l) =>
    typeof l === 'string' ? { value: l, label: cityLabel(l) } : l
  )
)

const { page, perPage, total, totalPages, goToPage, onPerPageChange } = usePagination({ perPage: 9 })

async function fetchFilterOptions() {
  try {
    const { data, error } = await useApi().get('/jobs/filter-options')
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data) {
      filterOptions.value = data
    }
  } catch (err) {
    console.error('Failed to load filter options:', err)
  }
}

async function fetchJobs() {
  loading.value = true;
  error.value = null;
  try {
    const params = { page: page.value, limit: perPage.value }
    if (filters.value.type) params.type = filters.value.type
    if (filters.value.location) params.location = filters.value.location
    if (filters.value.gender) params.gender = filters.value.gender
    if (filters.value.entityId) params.entityId = filters.value.entityId

    const { data, error } = await useApi().get('/jobs', params);
    if (error) {
      error.value = error;
      useToast().show(error, "error");
      return;
    }
    items.value = data?.items || [];
    total.value = data?.total || 0;
  } catch (err) {
    error.value = err?.message || 'حدث خطأ في تحميل الوظائف';
    useToast().show("حدث خطأ في تحميل الوظائف", "error");
  } finally {
    loading.value = false;
  }
}

function applyFilters() {
  page.value = 1
  fetchJobs()
}

function resetFilters() {
  filters.value = { type: '', location: '', gender: '', entityId: '' }
  page.value = 1
  fetchJobs()
}

function handlePageChange(p) {
  goToPage(p)
  fetchJobs()
}

onMounted(() => {
  fetchFilterOptions()
  fetchJobs()
})

function retry() {
  window.location.reload();
}

function markJobApplied(jobId) {
  const item = items.value.find(j => j.id === jobId)
  if (item) item.isApplied = true
}
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex gap-6 flex-row justify-between items-start">
        <div>
          <h1 class="md:text-base font-semibold mb-3">تصفية</h1>
          <p class="text-sm text-muted">
            قم بتخصيص نتائج البحث لعرض الوظائف التي تناسبك بشكل أفضل.
          </p>
        </div>
        <div>
          <button
            @click="toggleFilterAria"
            class="text-sm h-10 w-10 px-0 bg-bg-light rounded-full flex justify-center items-center border border-[#fff]/0 hover:border-primary transition"
            :aria-expanded="filterAreaExpands"
            aria-label="تصفية"
          >
            <ChevronUp />
          </button>
        </div>
      </div>

      <ExpandArea :expand="filterAreaExpands">
        <div>
          <div class="py-4">
            <hr />
          </div>
          <div class="py-4">
            <div
              class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 w-full"
            >
              <!-- Gender -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الجنس </label>
                <CustomSelect
                  :items="genderOptions"
                  placeholder="الجنس"
                  v-model="filters.gender"
                  key="select-2"
                />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> نوع الوظيفة </label>
                <CustomSelect
                  :items="typeOptions"
                  placeholder="اختر"
                  v-model="filters.type"
                  key="select-1"
                />
              </div>

              <!-- Location -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الموقع </label>
                <CustomSelect
                  :items="locationOptions"
                  placeholder="الموقع"
                  v-model="filters.location"
                  key="select-4"
                />
              </div>

              <!-- Company -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الشركة </label>
                <CustomSelect
                  :items="filterOptions.entities"
                  placeholder="الشركة"
                  v-model="filters.entityId"
                  key="select-3"
                />
              </div>
            </div>
          </div>
          <div class="py-4">
            <hr />
          </div>
          <div class="flex justify-between items-center">
            <button class="btn-outline text-sm px-10" @click="resetFilters">إعادة تعيين</button>

            <button class="btn-primary text-sm px-10" @click="applyFilters">تطبيق</button>
          </div>
        </div>
      </ExpandArea>
    </div>
    <!--  -->
    <div>
      <div class="flex gap-6 flex-col md:flex-row justify-between items-start">
        <div class="">
          <h1 class="text-base font-semibold mb-3">عرض {{ items.length }} وظيفة</h1>
          <p class="text-sm text-muted">بناءً على ملفك الشخصي وتفضيلاتك</p>
        </div>
        <div class="self-end">
          <button class="text-sm btn-outline gap-2">
            <SortBars />

            <span> بحلول تاريخ الإغلاق </span>
          </button>
        </div>
      </div>
      <div class="jobs-container py-8">
        <UiLoadingSkeleton v-if="loading" :count="12" :columns="3" height="280px" />
        <UiErrorState v-else-if="error" :message="error" @retry="retry" />
        <UiEmptyState v-else-if="!items.length" title="لا توجد وظائف" description="لم يتم العثور على وظائف متاحة حالياً" />
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <JobCard
            v-for="item in items"
            :key="item.id"
            :job="item"
            @open-apply-form="(id) => { selectedJobId = id; applyDialog = true }"
          />
        </div>
      </div>

      <div class="pt-6" v-if="totalPages > 1">
        <Pagination
          :current-page="page"
          :total-pages="totalPages"
          :per-page="perPage"
          @page-changed="handlePageChange"
          @per-page-change="onPerPageChange"
        />
      </div>
    </div>

    <!--  -->
    <ApplyJobDialog
      v-model="applyDialog"
      :job-id="selectedJobId"
      @applied="markJobApplied"
    />
  </div>
</template>

<style></style>
