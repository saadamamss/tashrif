<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import AdminUserDetail from "~/components/dashboard/admin/user-detail.vue";
import { buildImageUrl } from "~/services/help";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "admin"],
  meta: { requiresAuth: true },
});

useHead({ title: "إدارة المستخدمين" });

const loading = ref(false);
const error = ref(null);
const items = ref([]);
const selectedUser = ref(null);
const confirmUserId = ref(null);
const confirmLoading = ref(false);

const filters = ref({ search: "", type: "", status: "" });
const { page, perPage, total, totalPages, goToPage, onPerPageChange } = usePagination({ perPage: 10 });

const typeOptions = [
  { label: "الكل", value: "" },
  { label: "فرد", value: "individual" },
  { label: "جهة", value: "entity" },
  { label: "مدير", value: "admin" },
];

const statusOptions = [
  { label: "الكل", value: "" },
  { label: "نشط", value: "active" },
  { label: "معطل", value: "deactivated" },
];

const typeBadgeClass = {
  individual: "bg-blue-100 text-blue-700",
  entity: "bg-purple-100 text-purple-700",
  admin: "bg-amber-100 text-amber-700",
};

const typeLabel = { individual: "فرد", entity: "جهة", admin: "مدير" };

const fetchUsers = async () => {
  loading.value = true;
  error.value = null;
  try {
    const params = { page: page.value, limit: perPage.value };
    if (filters.value.type) params.type = filters.value.type;
    if (filters.value.status) params.status = filters.value.status;
    if (filters.value.search) params.search = filters.value.search;

    const { data, error: apiError } = await useApi().get("/admin/users", params);
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

const viewUser = async (userId) => {
  const { data, error: apiError } = await useApi().get(`/admin/users/${userId}`);
  if (apiError) {
    useToast().show(apiError, "error");
    return;
  }
  selectedUser.value = data;
  history.replaceState(null, "", `?id=${userId}`);
};

const closeDetail = () => {
  selectedUser.value = null;
  history.replaceState(null, "", location.pathname);
};

const deactivateUser = async (userId) => {
  confirmLoading.value = true;
  const { error: apiError } = await useApi().put(`/admin/users/${userId}/deactivate`);
  confirmLoading.value = false;
  confirmUserId.value = null;
  if (apiError) {
    useToast().show(apiError, "error");
    return;
  }
  useToast().show("تم تعطيل الحساب بنجاح", "success");
  if (selectedUser.value?.id === userId) {
    selectedUser.value.isDeleted = true;
  }
  fetchUsers();
};

const activateUser = async (userId) => {
  const { error: apiError } = await useApi().put(`/admin/users/${userId}/activate`);
  if (apiError) {
    useToast().show(apiError, "error");
    return;
  }
  useToast().show("تم تفعيل الحساب بنجاح", "success");
  if (selectedUser.value?.id === userId) {
    selectedUser.value.isDeleted = false;
  }
  fetchUsers();
};

const applyFilters = () => {
  page.value = 1;
  fetchUsers();
};

const resetFilters = () => {
  filters.value = { search: "", type: "", status: "" };
  page.value = 1;
  fetchUsers();
};

const handlePageChange = (p) => {
  goToPage(p);
  fetchUsers();
};

onMounted(() => {
  const route = useRoute();
  const id = route.query.id;
  fetchUsers();
  if (id) viewUser(id);
});
</script>

<template>
  <div class="px-4 lg:px-0">
    <AdminUserDetail
      v-if="selectedUser"
      :user="selectedUser"
      @close="closeDetail"
      @deactivated="fetchUsers"
      @activated="fetchUsers"
    />

    <template v-else>
      <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div class="w-full">
            <label class="text-sm mb-2 block">بحث</label>
            <input
              v-model="filters.search"
              type="text"
              placeholder="الاسم، البريد، أو رقم الهوية"
              class="h-[40px] w-full border rounded-2xl px-4 text-sm bg-bg-light outline-none focus:border-primary transition"
              @keyup.enter="applyFilters"
            />
          </div>
          <div class="w-full">
            <label class="text-sm mb-2 block">نوع المستخدم</label>
            <CustomSelect v-model="filters.type" :items="typeOptions" placeholder="الكل" />
          </div>
          <div class="w-full">
            <label class="text-sm mb-2 block">حالة الحساب</label>
            <CustomSelect v-model="filters.status" :items="statusOptions" placeholder="الكل" />
          </div>
        </div>
        <div class="flex justify-between items-center mt-6">
          <button class="btn-outline text-sm px-10" @click="resetFilters">إعادة تعيين</button>
          <button class="btn-primary text-sm px-10" @click="applyFilters">تطبيق</button>
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8">
        <h1 class="text-base font-semibold mb-6">قائمة المستخدمين</h1>

        <UiLoadingSkeleton v-if="loading" :count="6" :columns="2" height="64px" />
        <UiErrorState v-else-if="error" :message="error" @retry="fetchUsers" />
        <UiEmptyState
          v-else-if="!items.length"
          title="لا يوجد مستخدمون"
          description="لم يتم العثور على مستخدمين مطابقين للفلاتر المحددة"
        />
        <template v-else>
          <div class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="text-muted text-xs border-b">
                  <th class="text-right py-3 px-2 font-medium">المستخدم</th>
                  <th class="text-right py-3 px-2 font-medium">النوع</th>
                  <th class="text-right py-3 px-2 font-medium">رقم الهوية</th>
                  <th class="text-right py-3 px-2 font-medium">الحالة</th>
                  <th class="text-right py-3 px-2 font-medium">إجراءات</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="user in items"
                  :key="user.id"
                  class="border-b last:border-0 hover:bg-bg-light/50 transition cursor-pointer"
                  @click="viewUser(user.id)"
                >
                  <td class="py-3 px-2">
                    <div class="flex items-center gap-3">
                      <img
                        :src="buildImageUrl(user.avatarUrl, '/images/avatar.png')"
                        class="w-9 h-9 rounded-full object-cover"
                        alt=""
                      />
                      <div>
                        <p class="font-semibold">{{ user.name }}</p>
                        <p class="text-xs text-muted">{{ user.email }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="py-3 px-2">
                    <span class="text-xs px-3 py-1 rounded-full" :class="typeBadgeClass[user.type]">
                      {{ typeLabel[user.type] || user.type }}
                    </span>
                  </td>
                  <td class="py-3 px-2 text-muted">{{ user.nationalId || "-" }}</td>
                  <td class="py-3 px-2">
                    <span
                      class="text-xs px-3 py-1 rounded-full"
                      :class="user.isDeleted ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700'"
                    >
                      {{ user.isDeleted ? "معطل" : "نشط" }}
                    </span>
                  </td>
                  <td class="py-3 px-2">
                    <button
                      v-if="!user.isDeleted"
                      class="text-xs px-3 py-1.5 rounded-full border border-red-200 text-red-600 hover:bg-red-50 transition"
                      @click.stop="confirmUserId = user.id"
                    >
                      تعطيل
                    </button>
                    <button
                      v-else
                      class="text-xs px-3 py-1.5 rounded-full border border-green-200 text-green-700 hover:bg-green-50 transition"
                      @click.stop="activateUser(user.id)"
                    >
                      تفعيل
                    </button>
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
    </template>

    <ConfirmDialog
      v-model="confirmUserId"
      title="تعطيل الحساب"
      message="هل أنت متأكد من تعطيل هذا الحساب؟ يمكن التراجع عن هذه الخطوة لاحقاً."
      confirm-text="تعطيل"
      :is-loading="confirmLoading"
      @confirm="deactivateUser(confirmUserId)"
    />
  </div>
</template>
