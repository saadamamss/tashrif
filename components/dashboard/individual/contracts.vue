<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import ExpandArea from "~/components/ExpandArea.vue";
import Pdf from "~/components/icons/pdf.vue";
import SignContract from "~/components/SignContract.vue";

const statusOptions = ['الكل', 'قادمة', 'منتهية', 'ملغية']
const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};
const loading = ref(false);
const contracts = ref([]);
const signContractOpen = ref(false);
const signContractTarget = ref(null);

onMounted(async () => {
  loading.value = true
  try {
    const { data, error } = await useApi().get('/contracts')
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data?.items) contracts.value = data.items
  } finally {
    loading.value = false
  }
})

const openSignContract = (contract) => {
  signContractTarget.value = contract
  signContractOpen.value = true
}
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex flex-row gap-6 justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-3">تصفية</h1>
          <p class="text-sm text-muted">
            قم بتخصيص نتائج البحث لعرض الوظائف التي تناسبك بشكل أفضل.
          </p>
        </div>
        <div>
          <button @click="toggleFilterAria"
            class="text-sm h-10 w-10 px-0 bg-bg-light rounded-full flex justify-center items-center border border-[#fff]/0 hover:border-primary transition"
            :aria-expanded="filterAreaExpands" aria-label="تصفية">
            <ChevronUp />
          </button>
        </div>
      </div>
      <!--  -->
      <ExpandArea :expand="filterAreaExpands">
        <div>
          <div class="py-4">
            <hr />
          </div>
          <div class="py-4">
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 w-full">
              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> تاريخ المقابلة </label>
                <CustomSelect :items="statusOptions" placeholder="تاريخ المقابلة" key="select-2" />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> نوع الوظيفة </label>
                <CustomSelect :items="statusOptions" placeholder="اختر" key="select-1" />
              </div>

              <!-- Employer -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الموقع </label>
                <CustomSelect :items="statusOptions" placeholder="الموقع" key="select-4" />
              </div>

              <!-- Gender -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الشركة </label>
                <CustomSelect :items="statusOptions" placeholder="الشركة" key="select-3" />
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
      <div class="flex flex-col sm:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-3">عرض {{ contracts.length }} عقد عمل</h1>
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
          <ContractCard v-for="contract in contracts" :key="contract.id" :contract="contract"
            @open-sign-contract="openSignContract" />
        </div>
      </div>
    </div>

    <!--  -->
    <SignContract v-model="signContractOpen" :contract="signContractTarget"
      :contract-id="signContractTarget?.id || null"
    />
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
