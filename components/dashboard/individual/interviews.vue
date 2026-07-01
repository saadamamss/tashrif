<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
import InterviewCard from "~/components/InterviewCard.vue";

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};
//
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-8 mb-6">
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

      <!--  -->
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
          <h1 class="text-base font-semibold mb-3">عرض 84 مقابلة عمل</h1>
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
          <InterviewCard v-for="i in 12" />
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
