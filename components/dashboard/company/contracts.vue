<script setup>
import CompanyContractCard from "~/components/CompanyContractCard.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import ExpandArea from "~/components/ExpandArea.vue";
import Pdf from "~/components/icons/pdf.vue";
import SignContract from "~/components/SignContract.vue";
import UpdateContract from "~/components/UpdateContract.vue";

const statusOptions = [
  { value: 'sent', label: 'مرسلة' },
  { value: 'signed', label: 'موقعة' },
  { value: 'expired', label: 'منتهية' },
]
const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};
const loading = ref(false);
const contracts = ref([]);
const signContractOpen = ref(false);
const signContractTarget = ref(null);
const editContractOpen = ref(false);
const editContractTarget = ref(null);

const openEditContract = (contract) => {
  editContractTarget.value = contract
  editContractOpen.value = true
}

const filters = ref({
  status: '',
})

async function fetchContracts() {
  loading.value = true
  try {
    const params = {}
    if (filters.value.status) params.status = filters.value.status
    const { data, error } = await useApi().get('/contracts', params)
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data?.items) contracts.value = data.items
  } finally {
    loading.value = false
  }
}

function applyFilters() {
  fetchContracts()
}

function resetFilters() {
  filters.value = { status: '' }
  fetchContracts()
}

onMounted(fetchContracts)
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex flex-row justify-between items-start gap-6">
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
      <div class="flex flex-col sm:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-2">عرض {{ contracts.length }} عقد عمل</h1>
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
          <CompanyContractCard
            v-for="contract in contracts"
            :key="contract.id"
            :contract="contract"
            @showContract="(c) => { signContractTarget = c; signContractOpen = true }"
            @edit-contract="openEditContract"
          />
        </div>
      </div>
    </div>

    <!--  -->
    <SignContract v-model="signContractOpen" readonly :contract="signContractTarget" />
    <UpdateContract v-model="editContractOpen" :contract="editContractTarget" @updated="fetchContracts" />
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
