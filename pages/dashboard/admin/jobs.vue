<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "admin"],
  meta: { requiresAuth: true },
});

useHead({ title: "إدارة الوظائف" });

const loading = ref(false);
const error = ref(null);
const items = ref([]);
const confirmJobId = ref(null);
const confirmLoading = ref(false);

const filters = ref({ search: "", status: "" });
const { page, perPage, total, totalPages, goToPage, onPerPageChange } = usePagination({ perPage: 9 });

const statusOptions = [
  { label: "الكل", value: "" },
  { label: "نشطة", value: "active" },
  { label: "منتهية", value: "expired" },
  { label: "مغلقة", value: "closed" },
];

const statusLabel = {
  active: "نشطة",
  expired: "منتهية",
  closed: "مغلقة",
};

const statusBadgeClass = {
  active: "bg-green-100 text-green-700",
  expired: "bg-gray-100 text-gray-600",
  closed: "bg-red-100 text-red-700",
};

const fetchJobs = async () => {
  loading.value = true;
  error.value = null;
  try {
    const params = { page: page.value, limit: perPage.value };
    if (filters.value.status) params.status = filters.value.status;
    if (filters.value.search) params.search = filters.value.search;

    const { data, error: apiError } = await useApi().get("/admin/jobs", params);
    if (apiError) {
      error.value = apiError;
      useToast().show(apiError, "error");
      return;
    }
    items.value = data?.items || [];
    total.value = data?.total || 0;
  } finally {
    loading.value = false;
  }
};

const deactivateJob = async (jobId) => {
  confirmLoading.value = true;
  const { error: apiError } = await useApi().put(`/admin/jobs/${jobId}/deactivate`);
  confirmLoading.value = false;
  confirmJobId.value = null;
  if (apiError) {
    useToast().show(apiError, "error");
    return;
  }
  useToast().show("تم تعطيل الوظيفة بنجاح", "success");
  fetchJobs();
};

const applyFilters = () => {
  page.value = 1;
  fetchJobs();
};

const resetFilters = () => {
  filters.value = { search: "", status: "" };
  page.value = 1;
  fetchJobs();
};

const handlePageChange = (p) => {
  goToPage(p);
  fetchJobs();
};

onMounted(fetchJobs);
</script>

<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div class="w-full">
          <label class="text-sm mb-2 block">بحث</label>
          <input
            v-model="filters.search"
            type="text"
            placeholder="عنوان أو وصف الوظيفة"
            class="h-[40px] w-full border rounded-2xl px-4 text-sm bg-bg-light outline-none focus:border-primary transition"
            @keyup.enter="applyFilters"
          />
        </div>
        <div class="w-full">
          <label class="text-sm mb-2 block">الحالة</label>
          <CustomSelect v-model="filters.status" :items="statusOptions" placeholder="الكل" />
        </div>
      </div>
      <div class="flex justify-between items-center mt-6">
        <button class="btn-outline text-sm px-10" @click="resetFilters">إعادة تعيين</button>
        <button class="btn-primary text-sm px-10" @click="applyFilters">تطبيق</button>
      </div>
    </div>

    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8">
      <h1 class="text-base font-semibold mb-6">جميع الوظائف</h1>

      <UiLoadingSkeleton v-if="loading" :count="9" :columns="3" height="200px" />
      <UiErrorState v-else-if="error" :message="error" @retry="fetchJobs" />
      <UiEmptyState
        v-else-if="!items.length"
        title="لا توجد وظائف"
        description="لم يتم العثور على وظائف مطابقة للفلاتر المحددة"
      />
      <template v-else>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="text-muted text-xs border-b">
                <th class="text-right py-3 px-2 font-medium">الوظيفة</th>
                <th class="text-right py-3 px-2 font-medium">الجهة</th>
                <th class="text-right py-3 px-2 font-medium">الموقع</th>
                <th class="text-right py-3 px-2 font-medium">الحالة</th>
                <th class="text-right py-3 px-2 font-medium">المتقدمون</th>
                <th class="text-right py-3 px-2 font-medium">إجراءات</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="job in items" :key="job.id" class="border-b last:border-0 hover:bg-bg-light/50 transition">
                <td class="py-3 px-2">
                  <p class="font-semibold">{{ job.title }}</p>
                  <p class="text-xs text-muted line-clamp-1 max-w-[280px]">{{ job.description }}</p>
                </td>
                <td class="py-3 px-2 text-muted">{{ job.entityName }}</td>
                <td class="py-3 px-2 text-muted">{{ job.location }}</td>
                <td class="py-3 px-2">
                  <span
                    class="text-xs px-3 py-1 rounded-full whitespace-nowrap"
                    :class="statusBadgeClass[job.status] || 'bg-gray-100 text-gray-700'"
                  >
                    {{ statusLabel[job.status] || job.status }}
                  </span>
                </td>
                <td class="py-3 px-2 text-muted">{{ job.applicantCount }}</td>
                <td class="py-3 px-2">
                  <button
                    v-if="job.status !== 'closed'"
                    class="text-xs px-3 py-1.5 rounded-full border border-red-200 text-red-600 hover:bg-red-50 transition whitespace-nowrap"
                    @click="confirmJobId = job.id"
                  >
                    تعطيل
                  </button>
                  <span v-else class="text-xs text-muted">-</span>
                </td>
              </tr>
            </tbody>
          </table>
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
      </template>
    </div>

    <ConfirmDialog
      v-model="confirmJobId"
      title="تعطيل الوظيفة"
      message="هل أنت متأكد من تعطيل هذه الوظيفة؟ لن تظهر للباحثين عن عمل بعد التعطيل."
      confirm-text="تعطيل"
      :is-loading="confirmLoading"
      @confirm="deactivateJob(confirmJobId)"
    />
  </div>
</template>
