<script setup>
import CompanyContractCard from "~/components/CompanyContractCard.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import TextInput from "~/components/elements/TextInput.vue";
import ExpandArea from "~/components/ExpandArea.vue";
import Pdf from "~/components/icons/pdf.vue";
import SignContract from "~/components/SignContract.vue";

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};

//
const signContractOpen = ref(false);
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-3">تصفية</h1>
          <p class="text-sm text-[#667178]">
            قم بتخصيص نتائج البحث لعرض الوظائف التي تناسبك بشكل أفضل.
          </p>
        </div>
        <div>
          <button
            @click="toggleFilterAria"
            class="text-sm h-10 w-10 px-0 bg-[#f5f5f5] rounded-full flex justify-center items-center border border-[#fff]/0 hover:border-[#ecb42b] transition"
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
              <!-- Job Type -->
              <div class="w-full">
                <TextInput
                  class="text-sm placeholder:text-xs w-full px-3 rounded-2xl h-[38px] bg-[#f5f5f5] focus:outline-none border border-[#f5f5f]/0 focus:border-[#ecb42b] transition"
                  placeholder="اسم الوظيفة "
                  label="اسم الوظيفة "
                />
              </div>

              <div class="w-full">
                <TextInput
                  class="text-sm placeholder:text-xs w-full px-3 rounded-2xl h-[38px] bg-[#f5f5f5] focus:outline-none border border-[#f5f5f]/0 focus:border-[#ecb42b] transition"
                  placeholder="اسم المتقدم "
                  label="اسم المتقدم "
                />
              </div>

              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> تاريخ الإرسال </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="تاريخ الإرسال"
                 
                  key="select-2"
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
      <div class="flex flex-col sm:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-2">عرض 84 عقد عمل</h1>
          <p class="text-sm text-[#667178]">بناءً على ملفك الشخصي وتفضيلاتك</p>
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
            v-for="i in 12"
            :key="i"
            @showContract="signContractOpen = true"
          />
        </div>
      </div>
    </div>

    <!--  -->
    <SignContract v-model="signContractOpen" readonly />
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
