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
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="اختر"
                 
                  key="select-1"
                />
              </div>

              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  المنطقة <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="اختر"
                 
                  key="select-2"
                />
              </div>

              <!-- Gender -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  الجنس <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="اختر"
                 
                  key="select-3"
                />
              </div>

              <!-- Employer -->
              <div class="w-full">
                <label class="text-sm mb-2 block">
                  الجهة الموظفة <span class="text-red-500">*</span>
                </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="اختر"
                 
                  key="select-4"
                />
              </div>

              <!-- Search Button -->
              <div
                class="w-full mt-4 lg:mt-0 sm:col-span-2 lg:col-span-1 flex items-end justify-center"
              >
                <button class="btn-primary text-sm h-[42px]">
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
            <h2 class="text-lg font-semibold mb-3">عرض {{ items.length }} نتيجة وظيفة</h2>
            <p class="text-sm text-muted">
              بناءً على ملفك الشخصي وتفضيلاتك
            </p>
          </div>
          <button class="self-end btn-outline text-sm gap-2">
            <SortBars />
            <span> بحلول تاريخ الإغلاق </span>
          </button>
        </div>
      </div>
      <LoadingSkeleton v-if="loading" :rows="4" :columns="3" height="280px" />
      <ErrorState v-else-if="error" :message="error" @retry="retry" />
      <EmptyState v-else-if="!items.length" title="لا توجد نتائج" description="لم يتم العثور على وظائف تطابق معايير البحث" />
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <JobCard v-for="item in items" :key="item.id" @open-apply-form="openApplyForm" />
      </div>

      <div class="pt-6">
        <Pagination
          :current-page="currentPage"
          :total-pages="totalPages"
          :per-page="9"
          @page-changed="handlePageChange"
        />
      </div>
    </section>

    <!--  -->
    <ApplyJobDialog v-model="applyJobDialog" v-if="authStore.isAuthenticated" />
  </div>
</template>

<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import Pagination from "~/components/Pagination.vue";
definePageMeta({
  middleware: ["auth-global"],
  auth:false
});
// 
useScrollSpy();

const authStore = useAuthStore();
const { showModal, closeModal } = useLoginModal();
const applyJobDialog = ref(false);
const openApplyForm = () => {
  if (authStore.isAuthenticated) {
    applyJobDialog.value = true;
    return;
  }
  showModal();
};
const currentPage = ref(1);
const totalPages = ref(10);
const handlePageChange = (page) => {
  currentPage.value = page;
};
const loading = ref(false);
const error = ref(null);
const items = ref([]);

onMounted(async () => {
  loading.value = true;
  error.value = null;
  try {
    const { data } = await useApi().get('/jobs');
    items.value = data?.items || [];
  } catch (err) {
    error.value = err?.message || 'حدث خطأ في تحميل الوظائف';
  } finally {
    loading.value = false;
  }
});

function retry() {
  window.location.reload();
}
</script>
<style lang="scss" scoped>
.job-page {
  .hero-section {
    .section-content {
      min-height: 610px;
      background: linear-gradient(
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
