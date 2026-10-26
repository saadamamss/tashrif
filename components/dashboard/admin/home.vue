<script setup>
const loading = ref(true);
const stats = ref({
  totalUsers: 0,
  totalIndividuals: 0,
  totalEntities: 0,
  totalAdmins: 0,
  totalJobs: 0,
  activeJobs: 0,
  totalApplications: 0,
  totalContracts: 0,
});
const error = ref(null);

const loadStats = async () => {
  loading.value = true;
  error.value = null;
  const { data, error: apiError } = await useApi().get("/admin/stats");
  if (apiError) {
    error.value = apiError;
    useToast().show(apiError, "error");
  } else if (data) {
    stats.value = data;
  }
  loading.value = false;
};

onMounted(loadStats);

const mainCards = computed(() => [
  { label: "إجمالي المستخدمين", value: stats.value.totalUsers, color: "#ECB42B" },
  { label: "إجمالي الوظائف", value: stats.value.totalJobs, color: "#3B82F6" },
  { label: "إجمالي طلبات العمل", value: stats.value.totalApplications, color: "#10B981" },
  { label: "الوظائف النشطة", value: stats.value.activeJobs, color: "#8B5CF6" },
]);

const breakdownCards = computed(() => [
  { label: "الأفراد", value: stats.value.totalIndividuals, color: "#ECB42B" },
  { label: "الجهات", value: stats.value.totalEntities, color: "#3B82F6" },
  { label: "العقود", value: stats.value.totalContracts, color: "#10B981" },
  { label: "المديرون", value: stats.value.totalAdmins, color: "#8B5CF6" },
]);
</script>

<template>
  <div class="px-4 lg:px-0">
    <section class="bg-white rounded-xl shadow-sm p-4 sm:p-8">
      <div class="flex flex-col sm:flex-row justify-between gap-4 items-start sm:items-center mb-8">
        <div>
          <h1 class="text-lg font-bold mb-2">لوحة تحكم الإدارة</h1>
          <p class="text-sm text-muted">نظرة عامة على حالة المنصة</p>
        </div>
        <button
          class="text-sm h-10 w-10 px-0 bg-bg-light rounded-full flex justify-center items-center border border-[#fff]/0 hover:border-primary transition"
          :disabled="loading"
          aria-label="تحديث الإحصائيات"
          @click="loadStats"
        >
          <UiLoadingSpinner v-if="loading" class="w-5 h-5" />
          <RefreshIcon v-else />
        </button>
      </div>

      <UiErrorState v-if="error" :message="error" @retry="loadStats" />

      <UiLoadingSkeleton v-else-if="loading" :count="4" :columns="2" height="120px" />

      <template v-else>
        <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
          <div
            v-for="card in mainCards"
            :key="card.label"
            class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
          >
            <div>
              <h1 class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]">
                {{ card.value }}
              </h1>
              <h3 class="text-sm text-muted">{{ card.label }}</h3>
            </div>
            <span class="w-3 h-10 rounded-full" :style="{ backgroundColor: card.color }" />
          </div>
        </div>

        <div class="py-6">
          <hr />
        </div>

        <h2 class="text-base font-semibold mb-4">تفاصيل إضافية</h2>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
          <div
            v-for="card in breakdownCards"
            :key="card.label"
            class="p-4 bg-bg-light rounded-xl"
          >
            <h1 class="text-2xl font-extrabold mb-1 text-[#333]">{{ card.value }}</h1>
            <h3 class="text-sm text-muted">{{ card.label }}</h3>
          </div>
        </div>
      </template>
    </section>
  </div>
</template>
