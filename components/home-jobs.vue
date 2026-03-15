<template>
  <section class="jobs-section py-12 md:py-16 bg-[#F5F5F5]" id="jobs">
    <div class="px-0 section-content">
      <div class="mb-8 flex gap-6 sm:gap-4 flex-col sm:flex-row justify-between items-start max-w-[1300px] mx-auto px-4 sm:px-3 xl:px-0">
        <div>
          <h2 class="text-2xl md:text-3xl font-bold text-gray-800 mb-4">
            <span class="text-[#696C68] font-thin">اكتشف</span> الوظائف الموسمية
          </h2>
          <p class="text-[#696C68] max-w-2xl text-sm leading-[1.8]">
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
          <swiper-slide v-for="i in 8" :key="i" class="py-10 max-w-[400px]">
            <JobCard style="border-radius: 40px; border: none; max-width: 400px" @open-apply-form="$emit('apply')" />
          </swiper-slide>
        </swiper>
      </div>

      <div class="flex flex-col md:flex-row items-center gap-10 max-w-[1300px] mx-auto  px-4 sm:px-3 xl:px-0">
        <div id="swiper-scrollbar" class="w-full h-1 bg-white rounded-2"></div>
        <div class="flex gap-2 py-2 justify-end">
          <button id="prevButton" class="bg-white rounded-md py-3 px-4">
            <svg width="24" height="16" viewBox="0 0 24 16" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path opacity="0.4" fill-rule="evenodd" clip-rule="evenodd" d="M23.9995 7.99959C23.9995 8.73597 23.4025 9.33293 22.6661 9.33293L1.33277 9.33293C0.59639 9.33293 -0.000564607 8.73597 -0.000564575 7.99959C-0.000564543 7.26321 0.59639 6.66626 1.33277 6.66626L22.6661 6.66626C23.4025 6.66626 23.9995 7.26321 23.9995 7.99959Z" fill="#161616" />
              <path d="M21.2951 7.99976C21.1707 7.7934 20.9099 7.42257 20.6248 7.09984C20.0571 6.45739 19.2748 5.72355 18.4581 5.01449C17.6481 4.31119 16.8341 3.65795 16.2203 3.17892C15.9141 2.93996 15.3857 2.54077 15.2081 2.4067C14.6153 1.97 14.4887 1.1354 14.9253 0.542502C15.362 -0.0504288 16.1967 -0.177098 16.7896 0.259579L16.7945 0.263316C16.9871 0.408718 17.5452 0.830302 17.8609 1.07665C18.4971 1.57311 19.3498 2.25713 20.2064 3.00088C21.0564 3.73888 21.9408 4.56189 22.6231 5.33413C22.9629 5.71873 23.2808 6.12215 23.521 6.52056C23.7391 6.88236 23.9989 7.40882 23.9989 7.99971C23.9989 8.5906 23.7391 9.11715 23.521 9.47895C23.2808 9.87737 22.9629 10.2808 22.6231 10.6654C21.9408 11.4376 21.0564 12.2606 20.2064 12.9986C19.3498 13.7424 18.4971 14.4264 17.8609 14.9229C17.5452 15.1692 16.9874 15.5905 16.7949 15.7359L16.7896 15.7399C16.1967 16.1766 15.362 16.0499 14.9253 15.457C14.4887 14.8641 14.6153 14.0295 15.2081 13.5928C15.3857 13.4587 15.9141 13.0596 16.2203 12.8206C16.8341 12.3416 17.6481 11.6883 18.4581 10.985C19.2748 10.276 20.0571 9.54212 20.6248 8.89967C20.9099 8.57694 21.1707 8.20611 21.2951 7.99976Z" fill="#161614" />
            </svg>
          </button>
          <button id="nextButton" class="bg-white rounded-md py-3 px-4">
            <svg width="24" height="16" viewBox="0 0 24 16" fill="none" xmlns="http://www.w3.org/2000/svg" transform="rotate(180)">
              <path opacity="0.4" fill-rule="evenodd" clip-rule="evenodd" d="M23.9995 7.99959C23.9995 8.73597 23.4025 9.33293 22.6661 9.33293L1.33277 9.33293C0.59639 9.33293 -0.000564607 8.73597 -0.000564575 7.99959C-0.000564543 7.26321 0.59639 6.66626 1.33277 6.66626L22.6661 6.66626C23.4025 6.66626 23.9995 7.26321 23.9995 7.99959Z" fill="#161616" />
              <path d="M21.2951 7.99976C21.1707 7.7934 20.9099 7.42257 20.6248 7.09984C20.0571 6.45739 19.2748 5.72355 18.4581 5.01449C17.6481 4.31119 16.8341 3.65795 16.2203 3.17892C15.9141 2.93996 15.3857 2.54077 15.2081 2.4067C14.6153 1.97 14.4887 1.1354 14.9253 0.542502C15.362 -0.0504288 16.1967 -0.177098 16.7896 0.259579L16.7945 0.263316C16.9871 0.408718 17.5452 0.830302 17.8609 1.07665C18.4971 1.57311 19.3498 2.25713 20.2064 3.00088C21.0564 3.73888 21.9408 4.56189 22.6231 5.33413C22.9629 5.71873 23.2808 6.12215 23.521 6.52056C23.7391 6.88236 23.9989 7.40882 23.9989 7.99971C23.9989 8.5906 23.7391 9.11715 23.521 9.47895C23.2808 9.87737 22.9629 10.2808 22.6231 10.6654C21.9408 11.4376 21.0564 12.2606 20.2064 12.9986C19.3498 13.7424 18.4971 14.4264 17.8609 14.9229C17.5452 15.1692 16.9874 15.5905 16.7949 15.7359L16.7896 15.7399C16.1967 16.1766 15.362 16.0499 14.9253 15.457C14.4887 14.8641 14.6153 14.0295 15.2081 13.5928C15.3857 13.4587 15.9141 13.0596 16.2203 12.8206C16.8341 12.3416 17.6481 11.6883 18.4581 10.985C19.2748 10.276 20.0571 9.54212 20.6248 8.89967C20.9099 8.57694 21.1707 8.20611 21.2951 7.99976Z" fill="#161614" />
            </svg>
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
import JobCard from "./job-card.vue";

const modules = [Navigation, Scrollbar];
defineEmits(['apply']);
</script>