<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "admin"],
  meta: { requiresAuth: true },
});

useHead({ title: "سجل المراجعة" });

const loading = ref(false);
const error = ref(null);
const items = ref([]);

const filters = ref({
  userId: null,
  action: "",
  entityType: "",
  from: "",
  to: "",
});
const userSearchRef = ref(null);

const { page, perPage, total, totalPages, goToPage, onPerPageChange } = usePagination({ perPage: 12 });

const entityTypeOptions = [
  { label: "الكل", value: "" },
  { label: "طلبات العمل", value: "applications" },
  { label: "الوظائف", value: "jobs" },
  { label: "العقود", value: "contracts" },
  { label: "المقابلات", value: "interviews" },
  { label: "المستخدمون", value: "users" },
];

const actionLabel = {
  create: "إنشاء",
  update: "تحديث",
  delete: "حذف",
  status_change: "تغيير حالة",
  login: "تسجيل دخول",
};

const actionBadgeClass = {
  create: "bg-green-100 text-green-700",
  update: "bg-amber-100 text-amber-700",
  status_change: "bg-amber-100 text-amber-700",
  delete: "bg-red-100 text-red-700",
  login: "bg-blue-100 text-blue-700",
};

const formatDate = (date) => {
  if (!date) return "-";
  return new Date(date).toLocaleString("ar-SA");
};

const fetchLogs = async () => {
  loading.value = true;
  error.value = null;
  try {
    const params = { page: page.value, limit: perPage.value };
    if (filters.value.userId) params.userId = filters.value.userId;
    if (filters.value.action) params.action = filters.value.action;
    if (filters.value.entityType) params.entityType = filters.value.entityType;
    // Backend needs ISO 8601 UTC (Npgsql rejects date-only strings — see conventions)
    if (filters.value.from) params.from = new Date(filters.value.from).toISOString();
    if (filters.value.to) params.to = new Date(filters.value.to).toISOString();

    const { data, error: apiError } = await useApi().get("/admin/audit-logs", params);
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

const applyFilters = () => {
  page.value = 1;
  fetchLogs();
};

const resetFilters = () => {
  filters.value = { userId: null, action: "", entityType: "", from: "", to: "" };
  userSearchRef.value?.clear?.();
  page.value = 1;
  fetchLogs();
};

const handlePageChange = (p) => {
  goToPage(p);
  fetchLogs();
};

onMounted(fetchLogs);

const retry = () => fetchLogs();
</script>

<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <h1 class="text-base font-semibold mb-2">تصفية سجل المراجعة</h1>
      <p class="text-sm text-muted mb-6">
        سجل كامل لجميع العمليات الحساسة التي تمت على المنصة
      </p>

      <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
        <div class="w-full">
          <label class="text-sm mb-2 block">المستخدم</label>
          <UserSearchSelect
            ref="userSearchRef"
            v-model="filters.userId"
            placeholder="ابحث بالاسم أو البريد..."
          />
        </div>
        <div class="w-full">
          <label class="text-sm mb-2 block">نوع الكيان</label>
          <CustomSelect v-model="filters.entityType" :items="entityTypeOptions" placeholder="الكل" />
        </div>
        <div class="w-full">
          <label class="text-sm mb-2 block">من تاريخ</label>
          <input
            v-model="filters.from"
            type="date"
            class="h-[40px] w-full border rounded-2xl px-4 text-sm bg-bg-light outline-none focus:border-primary transition"
          />
        </div>
        <div class="w-full">
          <label class="text-sm mb-2 block">إلى تاريخ</label>
          <input
            v-model="filters.to"
            type="date"
            class="h-[40px] w-full border rounded-2xl px-4 text-sm bg-bg-light outline-none focus:border-primary transition"
          />
        </div>
      </div>

      <div class="flex justify-between items-center mt-6">
        <button class="btn-outline text-sm px-10" @click="resetFilters">إعادة تعيين</button>
        <button class="btn-primary text-sm px-10" @click="applyFilters">تطبيق</button>
      </div>
    </div>

    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8">
      <h1 class="text-base font-semibold mb-6">سجل العمليات</h1>

      <UiLoadingSkeleton v-if="loading" :count="8" :columns="1" height="56px" />
      <UiErrorState v-else-if="error" :message="error" @retry="retry" />
      <UiEmptyState
        v-else-if="!items.length"
        title="لا توجد سجلات"
        description="لم يتم العثور على عمليات مطابقة للفلاتر المحددة"
      />
      <template v-else>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="text-muted text-xs border-b">
                <th class="text-right py-3 px-2 font-medium">التاريخ</th>
                <th class="text-right py-3 px-2 font-medium">المستخدم</th>
                <th class="text-right py-3 px-2 font-medium">العملية</th>
                <th class="text-right py-3 px-2 font-medium">الكيان</th>
                <th class="text-right py-3 px-2 font-medium">التغيير</th>
                <th class="text-right py-3 px-2 font-medium">عنوان IP</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in items" :key="log.id" class="border-b last:border-0">
                <td class="py-3 px-2 text-muted whitespace-nowrap">{{ formatDate(log.createdAt) }}</td>
                <td class="py-3 px-2">
                  <p class="font-semibold">{{ log.userName || `مستخدم #${log.userId}` }}</p>
                </td>
                <td class="py-3 px-2">
                  <span
                    class="text-xs px-3 py-1 rounded-full whitespace-nowrap"
                    :class="actionBadgeClass[log.action] || 'bg-gray-100 text-gray-700'"
                  >
                    {{ actionLabel[log.action] || log.action }}
                  </span>
                </td>
                <td class="py-3 px-2 text-muted whitespace-nowrap">
                  {{ log.entityType }} #{{ log.entityId }}
                </td>
                <td class="py-3 px-2">
                  <span v-if="log.oldValue || log.newValue" class="text-xs">
                    <span class="text-red-600 line-through">{{ log.oldValue || "—" }}</span>
                    <span class="mx-1">←</span>
                    <span class="text-green-700">{{ log.newValue || "—" }}</span>
                  </span>
                  <span v-else class="text-muted text-xs">-</span>
                </td>
                <td class="py-3 px-2 text-xs text-muted" dir="ltr">{{ log.ipAddress || "-" }}</td>
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
  </div>
</template>
