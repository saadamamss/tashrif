<template>
  <section class="hero-section mb-[100px]" id="home">
    <div class="section-content">
      <div class="box flex flex-col justify-between px-4 lg:px-0">
        <div class="max-w-[740px] mx-auto mb-24">
          <div class="flex flex-col items-center gap-6 text-content text-center text-white">
            <h1 class="text-3xl md:text-4x1 lg:text-[50px] font-bold leading-[1.6] md:leading-[1.44]">
              منصتك الذكية للتوظيف الموسمي في الحج والعمرة
            </h1>
            <p class="text-sm md:text-lg md:leading-[1.5rem]">
              نربط الباحثين عن فرص العمل الموسمي بالجهات المشغّلة لخدمة ضيوف الرحمن باحترافية وجودة عالية. من خلال تقنيات ذكية وفلترة متقدمة، نسهّل عملية التوظيف من البداية حتى الانضمام.
            </p>
            <button class="play-video-btn flex gap-2 bg-[#000]/50 border border-[#fff]/0 rounded-full text-sm py-3 px-5 hover:bg-[#000]/30 hover:border-[#ecb42b]/50 transition duration-300">
              <span>تشغيل الفيديو</span>
              <svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path opacity="0.4" d="M12.5 1.25C6.56294 1.25 1.75 6.06294 1.75 12C1.75 17.9371 6.56294 22.75 12.5 22.75C18.4371 22.75 23.25 17.9371 23.25 12C23.25 6.06294 18.4371 1.25 12.5 1.25Z" fill="white" />
                <path d="M14.0886 9.03698L14.007 8.98648C13.3503 8.58022 12.7993 8.23934 12.3422 8.02763C11.8786 7.81293 11.35 7.65091 10.7988 7.82101C10.4345 7.93342 10.1108 8.14441 9.85518 8.42523C9.48329 8.8338 9.36071 9.36777 9.30525 9.90146C9.24997 10.4333 9.24999 11.1204 9.25 11.9571V12.0431C9.24999 12.8798 9.24997 13.567 9.30525 14.0988C9.36071 14.6325 9.48329 15.1665 9.85518 15.575C10.1108 15.8559 10.4345 16.0668 10.7988 16.1793C11.35 16.3494 11.8786 16.1873 12.3422 15.9726C12.7993 15.7609 13.3503 15.4201 14.007 15.0138L14.0886 14.9633C14.7703 14.5416 15.3387 14.19 15.7505 13.8625C16.1745 13.5252 16.5479 13.1266 16.6821 12.5711C16.7726 12.1966 16.7726 11.8037 16.6821 11.4292C16.5479 10.8736 16.1745 10.475 15.7505 10.1378C15.3387 9.81024 14.7703 9.45867 14.0886 9.03698Z" fill="white" />
              </svg>
            </button>
          </div>
        </div>

        <div class="job-filter w-full max-w-[1180px] mx-auto bg-white p-4 md:p-6 lg:p-10 rounded-xl lg:rounded-3xl">
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 w-full">
            <div class="w-full">
              <label class="text-sm mb-2 block">نوع الوظيفة <span class="text-red-500">*</span></label>
              <CustomSelect :items="['Option 1', 'Option 2', 'Option 3']" placeholder="اختر" />
            </div>
            <div class="w-full">
              <label class="text-sm mb-2 block">المنطقة <span class="text-red-500">*</span></label>
              <CustomSelect :items="['Option 1', 'Option 2', 'Option 3']" placeholder="اختر" />
            </div>
            <div class="w-full">
              <label class="text-sm mb-2 block">الجنس <span class="text-red-500">*</span></label>
              <CustomSelect :items="['Option 1', 'Option 2', 'Option 3']" placeholder="اختر" />
            </div>
            <div class="w-full">
              <label class="text-sm mb-2 block">الجهة الموظفة <span class="text-red-500">*</span></label>
              <CustomSelect :items="['Option 1', 'Option 2', 'Option 3']" placeholder="اختر" />
            </div>
            <div class="w-full mt-4 lg:mt-0 sm:col-span-2 lg:col-span-1 flex items-end justify-center">
              <button class="btn-primary text-sm h-[42px]">البحث عن وظيفة</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import CustomSelect from "./elements/custom-select.vue";
</script>

<style lang="scss" scoped>
.hero-section {
  .section-content {
    min-height: 610px;
    background: linear-gradient(179.24deg, rgba(0, 0, 0, 0.7) 12.77%, rgba(0, 0, 0, 0) 139.66%), url("~/assets/images/hero-section.png");
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
  .play-video-btn {
    &:active {
      border-color: transparent;
      box-shadow: 0 0 0 3px rgb(236, 180, 43, 0.4);
    }
  }
}
</style>