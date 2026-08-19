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
            <button class="play-video-btn flex gap-2 bg-[#000]/50 border border-[#fff]/0 rounded-full text-sm py-3 px-5 hover:bg-[#000]/30 hover:border-primary/50 transition duration-300">
              <span>تشغيل الفيديو</span>
              <Play />
            </button>
          </div>
        </div>

        <div class="job-filter w-full max-w-[1180px] mx-auto bg-white p-4 md:p-6 lg:p-10 rounded-xl lg:rounded-3xl">
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 w-full">
            <div class="w-full">
              <label class="text-sm mb-2 block">نوع الوظيفة <span class="text-red-500">*</span></label>
              <CustomSelect :items="workTypes" placeholder="اختر" />
            </div>
            <div class="w-full">
              <label class="text-sm mb-2 block">المنطقة <span class="text-red-500">*</span></label>
              <CustomSelect :items="locations" placeholder="اختر" />
            </div>
            <div class="w-full">
              <label class="text-sm mb-2 block">الجنس <span class="text-red-500">*</span></label>
              <CustomSelect :items="genders" placeholder="اختر" />
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
import CustomSelect from "./elements/CustomSelect.vue";
import { ref, onMounted } from "vue";

const workTypes = ref([])
const locations = ref([])
const genders = ref([])

onMounted(async () => {
  try {
    const { data, error } = await useApi().get('/jobs/filter-options')
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data) {
      workTypes.value = data.workTypes || ['ميداني', 'مكتبي', 'عن بعد']
      locations.value = data.locations || ['مكة المكرمة', 'المدينة المنورة', 'منى', 'عرفات', 'مزدلفة']
      genders.value = data.genders || ['الكل', 'رجال', 'نساء']
    }
  } catch (e) { console.error(e) }
})
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