<script setup>
import CompanyInterviewCard from "~/components/CompanyInterviewCard.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import ExpandArea from "~/components/ExpandArea.vue";

const statusOptions = [
  { value: 'upcoming', label: 'قادمة' },
  { value: 'past', label: 'فات موعدها' },
  { value: 'completed', label: 'منتهية' },
]
const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};
const loading = ref(false);
const items = ref([]);

const filters = ref({
  status: '',
})

async function fetchInterviews() {
  loading.value = true
  try {
    const params = {}
    if (filters.value.status) params.status = filters.value.status
    const { data, error } = await useApi().get('/interviews', params)
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data?.items) items.value = data.items
  } finally {
    loading.value = false
  }
}

function applyFilters() {
  fetchInterviews()
}

function resetFilters() {
  filters.value = { status: '' }
  fetchInterviews()
}

onMounted(fetchInterviews)
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex flex-row gap-6 justify-between items-start">
        <div>
          <h1 class="text-base font-semibold mb-3">تصفية</h1>
          <p class="text-sm text-muted">
            قم بتخصيص نتائج البحث لعرض المقابلات التي تناسبك.
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
              class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 w-full"
            >
              <!-- Status -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الحالة </label>
                <CustomSelect
                  :items="statusOptions"
                  placeholder="الحالة"
                  v-model="filters.status"
                  key="select-1"
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
      <div class="flex flex-col md:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-2">عرض {{ items.length }} مقابلة عمل</h1>
          <p class="text-sm text-muted">بناءً على ملفك الشخصي وتفضيلاتك</p>
        </div>
        <div class="self-end">
          <button class="text-sm btn-outline gap-2">
            <SortBars />

            <span> بحلول تاريخ المقابلة </span>
          </button>
        </div>
      </div>
      <div class="jobs-container py-8">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <CompanyInterviewCard v-for="item in items" :key="item.id" :interview="item" />
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
