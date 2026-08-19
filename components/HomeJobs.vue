<template>
  <section class="jobs-section py-12 md:py-16 bg-bg-light" id="jobs">
    <div class="px-0 section-content">
      <div class="mb-8 flex gap-6 sm:gap-4 flex-col sm:flex-row justify-between items-start max-w-[1300px] mx-auto px-4 sm:px-3 xl:px-0">
        <div>
          <h2 class="text-2xl md:text-3xl font-bold text-gray-800 mb-4">
            <span class="text-icon-muted font-thin">اكتشف</span> الوظائف الموسمية
          </h2>
          <p class="text-icon-muted max-w-2xl text-sm leading-[1.8]">
            استعرض مئات الفرص المتاحة في موسم الحج والعمرة، واختر الوظيفة التي تناسب مهاراتك وجدولك الزمني.
          </p>
        </div>
        <nuxt-link to="/jobs" class="btn-outline text-sm text-center">استعرض جميع الوظائف</nuxt-link>
      </div>

      <div class="jobs-slider">
        <swiper
          :modules="modules"
          :navigation="{ prevEl: '#prevButton', nextEl: '#nextButton' }"
          class="!px-4 sm:!px-3 xl:!px-[100px]"
          :scrollbar="{ el: '#swiper-scrollbar', draggable: true, snapOnRelease: true }"
          :slidesPerView="'auto'"
          :space-between="20"
          dir="rtl"
        >
          <swiper-slide v-for="job in jobs" :key="job.id" class="py-10 max-w-[400px]">
            <JobCard :job="job" style="border-radius: 40px; border: none; max-width: 400px" @open-apply-form="(id) => $emit('apply', id)" />
          </swiper-slide>
        </swiper>
      </div>

      <div class="flex flex-col md:flex-row items-center gap-10 max-w-[1300px] mx-auto  px-4 sm:px-3 xl:px-0">
        <div id="swiper-scrollbar" class="w-full h-1 bg-white rounded-2"></div>
        <div class="flex gap-2 py-2 justify-end">
          <button id="prevButton" class="bg-white rounded-md py-3 px-4" aria-label="السابق">
            <ChevronLeftIcon width="24" height="16" color="#BEC2C5" />
          </button>
          <button id="nextButton" class="bg-white rounded-md py-3 px-4" aria-label="التالي">
            <ChevronRightIcon width="24" height="16" color="#BEC2C5" />
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import { Navigation, Scrollbar } from "swiper/modules";
import "swiper/css";
import "swiper/css/navigation";
import "swiper/css/scrollbar";
import JobCard from "./JobCard.vue";
import ChevronLeftIcon from "./icons/ChevronLeftIcon.vue";
import ChevronRightIcon from "./icons/ChevronRightIcon.vue";

import { ref, onMounted } from "vue";

const modules = [Navigation, Scrollbar];
defineEmits(['apply']);

const jobs = ref([])
onMounted(async () => {
  try {
    const { data, error } = await useApi().get('/jobs?limit=8')
    if (error) {
      useToast().show(error, "error")
      return
    }
    if (data) jobs.value = data.items || data
  } catch (e) { console.error(e) }
})
</script>