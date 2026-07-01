<script setup>
import CompanyInterviewCard from "~/components/CompanyInterviewCard.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import TextInput from "~/components/elements/TextInput.vue";

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};
//
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
                <label class="text-sm mb-2 block"> تاريخ المقابلة </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="تاريخ المقابلة"
                 
                  key="select-2"
                />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <TextInput
                  class="text-sm placeholder:text-xs w-full px-3 rounded-2xl h-[38px] bg-bg-light focus:outline-none border border-[#f5f5f]/0 focus:border-primary transition"
                  placeholder="اسم الوظيفة "
                  label="اسم الوظيفة "
                />
              </div>

              <div class="w-full">
                <TextInput
                  class="text-sm placeholder:text-xs w-full px-3 rounded-2xl h-[38px] bg-bg-light focus:outline-none border border-[#f5f5f]/0 focus:border-primary transition"
                  placeholder="اسم المتقدم "
                  label="اسم المتقدم "
                />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> نوع المقابلة </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="نوع المقابلة "
                 
                  key="select-1"
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
      <div class="flex flex-col md:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-2">عرض 84 مقابلة عمل</h1>
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
          <CompanyInterviewCard v-for="i in 12" />
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
