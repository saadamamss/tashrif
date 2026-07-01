<script setup>
import CustomSelect from "~/components/elements/CustomSelect.vue";
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
      <div class="flex flex-row gap-6 justify-between items-start gap-6">
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
      <div class="flex flex-col sm:flex-row justify-between items-start gap-6">
        <div>
          <h1 class="text-base font-semibold mb-3">عرض 84 عقد عمل</h1>
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
          <div
            v-for="i in 12"
            class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6"
          >
            <div class="pb-4 border-b-2">
              <div class="flex items-center gap-2">
                <span
                  class="company-logo border-2 rounded-md overflow-hidden py-1 px-2"
                >
                  <img
                    src="/images/partner-3.svg"
                    class="w-10 h-6 object-cover"
                  />
                </span>
                <span class="company-name font-medium text-sm text-[#161614]">
                  شركة نسك لخدمات الحجاج
                </span>
              </div>
            </div>
            <div class="details py-6 flex flex-col gap-4">
              <div class="flex gap-2 items-center">
                <span>
                  <CalenderIcon width="18" height="18" color="#696C68" />
                </span>
                <span class="text-[#696C68] text-xs">
                  5 – 13 ذو الحجة 1446هـ
                </span>
              </div>
              <div class="flex gap-2 items-center">
                <span>
                  <Location />
                </span>
                <span class="text-[#696C68] text-xs">
                  حي العزيزية، مكة المكرمة
                </span>
              </div>
            </div>

            <div class="p-4 rounded-xl bg-[#F8F9F9] mb-2">
              <div class="flex justify-between gap-4 items-center">
                <div class="flex flex-wrap items-center gap-2">
                  <span class="block p-2 bg-white rounded-xl">
                    <Pdf />
                  </span>
                  <div>
                    <span class="block text-slate-900 text-sm mb-1">
                      pdf السيرة الذاتية
                    </span>
                    <span class="text-xs block text-slate-400"> 1.2Mb </span>
                  </div>
                </div>
                <div>
                  <button class="block shadow-sm p-2 bg-white rounded-lg">
                    <Download />
                  </button>
                </div>
              </div>
            </div>
            <p class="text-xs text-[#667178] mb-6">
              يجب توقيع العقد قبل تاريخ 15 ذو القعدة 1446هـ لتأكيد انضمامك
              رسميًا.
            </p>

            <div class="flex gap-4">
              <button
                @click="signContractOpen = true"
                class="flex-1 text-center btn-primary text-sm"
              >
                توقيع العقد الإلكترونى
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!--  -->
    <SignContract v-model="signContractOpen" />
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
