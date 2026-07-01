<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import JobRequestCard from "~/components/JobRequestCard.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global", "auth-guard", "individual"],
});

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};

const loading = ref(false);
const error = ref(null);
const items = ref([]);

onMounted(async () => {
  loading.value = true;
  error.value = null;
  try {
    const { data } = await useApi().get('/applications');
    items.value = data?.items || [];
  } catch (err) {
    error.value = err?.message || 'حدث خطأ في تحميل الطلبات';
  } finally {
    loading.value = false;
  }
});

function retry() {
  window.location.reload();
}
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex flex-row gap-6 justify-between items-start">
        <div>
          <h1 class="text-base font-semibold mb-3">تصفية</h1>
          <p class="text-sm text-muted">
            قم بتخصيص نتائج البحث لعرض الوظائف التي تناسبك بشكل أفضل.
          </p>
        </div>
        <div>
          <button
            @click="toggleFilterAria"
            class="text-sm h-10 w-10 px-0 bg-bg-light rounded-full flex justify-center items-center border border-[#fff]/0 hover:border-primary transition"
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
              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> تاريخ التقديم </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="تاريخ التقديم"
                 
                  key="select-2"
                />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> نوع الوظيفة </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="اختر"
                 
                  key="select-1"
                />
              </div>

              <!-- Employer -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الموقع </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="الموقع"
                 
                  key="select-4"
                />
              </div>

              <!-- Gender -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الشركة </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="الشركة"
                 
                  key="select-3"
                />
              </div>
            </div>
          </div>
          <div class="py-4">
            <hr />
          </div>
          <div class="flex justify-between items-center">
            <button class="btn-outline text-sm px-10">إعادة تعيين</button>

            <button class="btn-primary text-sm px-10">تطبيق</button>
          </div>
        </div>
      </ExpandArea>
    </div>
    <!--  -->
    <div>
      <div class="flex flex-col md:flex-row gap-6 justify-between items-start">
        <div>
          <h1 class="text-base font-semibold mb-3">عرض {{ items.length }} طلب عمل</h1>
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
        <LoadingSkeleton v-if="loading" :rows="4" :columns="3" height="200px" />
        <ErrorState v-else-if="error" :message="error" @retry="retry" />
        <EmptyState v-else-if="!items.length" title="لا توجد طلبات" description="لم تتقدم لأي وظيفة بعد" />
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <JobRequestCard
            v-for="item in items"
            :key="item.id"
            class="border border-[#fff]/0 hover:border-primary transition"
            role="button"
            @click="$router.push('/dashboard/job-requests/details')"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
