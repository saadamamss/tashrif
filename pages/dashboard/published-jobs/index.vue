<script setup>
import CustomSelect from "~/components/elements/custom-select.vue";
import InterviewCard from "~/components/interview-card.vue";
import JobOfferCard from "~/components/job-offer-card.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global", "auth-guard", "entity"],
});

const filterAreaExpands = ref(false);
const toggleFilterAria = () => {
  filterAreaExpands.value = !filterAreaExpands.value;
};

const handleSelection = () => {};
</script>
<template>
  <div class="px-4 lg:px-0">
    <div class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-6">
      <div class="flex gap-6 justify-between items-start">
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
            <svg
              width="14"
              height="8"
              viewBox="0 0 14 8"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M1.58332 7.08398L6.99999 1.25065L12.4167 7.08398"
                stroke="#929A9F"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
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
              <!-- Region -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> تاريخ النشر </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="تاريخ النشر"
                  @selected="handleSelection"
                  key="select-2"
                />
              </div>
              <!-- Job Type -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> تاريخ الإنتهاء </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="تاريخ الإنتهاء"
                  @selected="handleSelection"
                  key="select-1"
                />
              </div>

              <!-- Employer -->
              <div class="w-full">
                <label class="text-sm mb-2 block"> الحالة </label>
                <CustomSelect
                  :items="['Option 1', 'Option 2', 'Option 3']"
                  placeholder="الحالة"
                  @selected="handleSelection"
                  key="select-4"
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
      <div class="flex flex-col md:flex-row justify-between gap-6 items-start">
        <div>
          <h1 class="text-base font-semibold mb-3">عرض 84 وظيفة منشورة</h1>
          <p class="text-sm text-[#667178]">بناءً على ملفك الشخصي وتفضيلاتك</p>
        </div>
        <div class="self-end flex gap-3">
          <button class="text-sm btn-outline gap-2">
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M3.56061 5.6857C3.69102 5.53896 3.85651 5.30857 4.0088 5.09651L4.04439 5.04693C4.11131 4.9537 4.18003 4.85797 4.24999 4.76188V9C4.24999 9.41421 4.58578 9.75 4.99999 9.75C5.41421 9.75 5.74999 9.41421 5.74999 9V4.76188C5.81997 4.85798 5.88867 4.95369 5.9556 5.04693L5.99119 5.09651C6.14348 5.30857 6.30897 5.53896 6.43938 5.6857C6.71453 5.99532 7.18858 6.02327 7.4982 5.74812C7.80782 5.47297 7.83576 4.99892 7.56061 4.6893C7.49441 4.6148 7.38162 4.46113 7.20957 4.22155L7.1718 4.16893C7.01991 3.95729 6.8405 3.70729 6.6555 3.46849C6.45778 3.21329 6.23272 2.94317 6.00474 2.73079C5.89068 2.62454 5.75732 2.51509 5.6095 2.42857C5.46897 2.34632 5.25683 2.25 4.99999 2.25C4.74316 2.25 4.53102 2.34632 4.39049 2.42857C4.24267 2.51509 4.10931 2.62454 3.99525 2.73079C3.76726 2.94317 3.54221 3.21329 3.34449 3.46849C3.15949 3.70729 2.98007 3.9573 2.82818 4.16895L2.79042 4.22155C2.61837 4.46113 2.50558 4.6148 2.43938 4.6893C2.16423 4.99892 2.19217 5.47297 2.50179 5.74812C2.81141 6.02327 3.28546 5.99532 3.56061 5.6857Z"
                fill="#161616"
              />
              <path
                d="M11 5.25C10.5858 5.25 10.25 5.58579 10.25 6C10.25 6.41421 10.5858 6.75 11 6.75H21C21.4142 6.75 21.75 6.41421 21.75 6C21.75 5.58579 21.4142 5.25 21 5.25H11Z"
                fill="#161616"
              />
              <path
                d="M11 9.25001C10.5858 9.25 10.25 9.58579 10.25 10C10.25 10.4142 10.5858 10.75 11 10.75L18 10.7501C18.4142 10.7501 18.75 10.4143 18.75 10.0001C18.75 9.58585 18.4142 9.25006 18 9.25005L11 9.25001Z"
                fill="#161616"
              />
              <path
                d="M11 13.25C10.5858 13.25 10.25 13.5858 10.25 14C10.25 14.4142 10.5858 14.75 11 14.75H16C16.4142 14.75 16.75 14.4142 16.75 14C16.75 13.5858 16.4142 13.25 16 13.25H11Z"
                fill="#161616"
              />
              <path
                d="M10.25 18C10.25 17.5858 10.5858 17.25 11 17.25H14C14.4142 17.25 14.75 17.5858 14.75 18C14.75 18.4142 14.4142 18.75 14 18.75H11C10.5858 18.75 10.25 18.4142 10.25 18Z"
                fill="#161616"
              />
              <path
                d="M5.99117 18.9035C6.14346 18.6914 6.30895 18.461 6.43936 18.3143C6.71451 18.0047 7.18856 17.9767 7.49818 18.2519C7.8078 18.527 7.83575 19.0011 7.5606 19.3107C7.49439 19.3852 7.3816 19.5389 7.20955 19.7785L7.17179 19.8311C7.0199 20.0427 6.84048 20.2927 6.65548 20.5315C6.45777 20.7867 6.23271 21.0568 6.00472 21.2692C5.89067 21.3755 5.7573 21.4849 5.60948 21.5714C5.46895 21.6537 5.25681 21.75 4.99998 21.75C4.74314 21.75 4.531 21.6537 4.39047 21.5714C4.24265 21.4849 4.10929 21.3755 3.99523 21.2692C3.76725 21.0568 3.54219 20.7867 3.34448 20.5315C3.15948 20.2927 2.98006 20.0427 2.82817 19.8311L2.7904 19.7785C2.61835 19.5389 2.50556 19.3852 2.43936 19.3107C2.16421 19.0011 2.19215 18.527 2.50177 18.2519C2.81139 17.9767 3.28545 18.0047 3.5606 18.3143C3.691 18.461 3.8565 18.6914 4.00878 18.9035L4.04437 18.9531C4.11127 19.0463 4.18004 19.1421 4.24998 19.2381L4.24998 15C4.24998 14.5858 4.58576 14.25 4.99998 14.25C5.41419 14.25 5.74998 14.5858 5.74998 15L5.74998 19.2381C5.81984 19.1422 5.88842 19.0466 5.95524 18.9535L5.99117 18.9035Z"
                fill="#161616"
              />
            </svg>

            <span> بحلول تاريخ المقابلة </span>
          </button>
          <nuxt-link class="btn-primary text-sm" to="/dashboard/publish-job">
            نشر وظيفة جديدة
          </nuxt-link>
        </div>
      </div>
      <div class="jobs-container py-8">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <JobOfferCard
            v-for="i in 12"
            :key="i"
            class="border border-[#fff]/0 hover:border-[#ecb42b] transition"
            role="button"
            @click="$router.push('/dashboard/published-jobs/details')"
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
