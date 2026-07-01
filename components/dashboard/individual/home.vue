<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import PercentCircle from "~/components/PercentCircle.vue";
import JobCard from "~/components/JobCard.vue";
import JobRequestCard from "~/components/JobRequestCard.vue";
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import ChevronLeftIcon from "~/components/icons/ChevronLeftIcon.vue";

const applyJobDialog = ref(false);
const openApplyForm = () => {
  applyJobDialog.value = true;
  return;
};
</script>
<template>
  <div class="px-4 lg:px-0">
    <section
      class="bg-white profile-statistics rounded-xl shadow-sm overflow-hidden min-h-[495px]"
    >
      <div class="container-fluid">
        <div class="back-image h-[197px] overflow-hidden">
          <img
            src="~/assets/images/profile-back.png"
            class="w-full h-full object-cover"
            alt="back-profile"
          />
        </div>
        <div class="px-8 pb-8">
          <div
            class="flex flex-col sm:flex-row justify-between gap-6 items-center -mt-16"
          >
            <div class="text-center sm:text-right">
              <div
                class="border-4 border-[#f5f5f5] mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
              >
                <img
                  src="~/assets/images/avatar.png"
                  class="w-full h-full"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">عبدالله محمد الحربي</h1>
                <p class="text-base text-gray-500">مشرف تنظيم حشود</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center justify-center gap-2 bg-[#f5f5f5] px-4 py-3 rounded-full text-sm border border-[#ecb42b]/0 hover:border-[#ecb42b] transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
            </div>
          </div>

          <div
            class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6 mt-8"
          >
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  10
                </h1>
                <h3 class="text-sm text-[#667178]">الطلبات المكتملة</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>

            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  12
                </h1>
                <h3 class="text-sm text-[#667178]">مقابلة عمل</h3>
              </div>
              <span>
                <ClockIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  24
                </h1>
                <h3 class="text-sm text-[#667178]">تقدمت للوظائف</h3>
              </div>
              <span>
                <BriefcaseIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  12
                </h1>
                <h3 class="text-sm text-[#667178]">عروض العمل</h3>
              </div>
              <span>
                <EyeIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!--  -->
    <section class="complete-profile bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid p-8">
        <div
          class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
        >
          <div class="flex flex-col sm:flex-row gap-6 max-w-[760px]">
            <PercentCircle
              :percent="57"
              gradient-start="#ECB42B05"
              gradient-middle="#ECB42B01"
              gradient-end="#ECB42B"
              :size="94"
            />
            <div>
              <h1 class="text-base font-semibold mb-3">أستكمل ملفك الشخصي</h1>
              <p class="text-sm text-[#667178]">
                استكمالك لملفك الشخصي يساعدنا في ترشيح الوظائف الأنسب لك، ويزيد
                من فرص قبولك لدى الجهات. أضف معلوماتك الشخصية، مؤهلاتك، وخبراتك
                العملية لتظهر بشكل احترافي أمام أصحاب العمل.
              </p>
            </div>
          </div>
          <div class="">
            <nuxt-link to="#" class="btn-primary text-nowrap text-sm px-8">
              إستكمل ملفك الشخصى
            </nuxt-link>
          </div>
        </div>
      </div>
    </section>
    <!--  -->

    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 xs:p-6 sm:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  استكشف طلبات العمل الخاص بك
                </h1>
                <p class="text-sm text-[#667178]">
                  تابع حالة الوظائف التي تقدمت لها، وابقَ على اطلاع بآخر
                  التحديثات من الجهات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/job-requests"
                class="self-end flex items-center justify-center gap-2 bg-[#f5f5f5] px-4 py-3 rounded-full text-sm border border-[#ecb42b]/0 hover:border-[#ecb42b] transition"
              >
                مشاهدة الكل
                <ChevronLeftIcon width="16" height="16" color="#161614" />
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

          <div class="p-4 xs:p-6 sm:ps-8">
            <Swiper
              :modules="[]"
              :initial-slide="0"
              class="swiper-modal"
              :space-between="20"
              :breakpoints="{
                640: { slidesPerView: 2.1 },
                786: { slidesPerView: 2.3 },
                1024: { slidesPerView: 2.5 },
              }"
              direction="horizontal"
              dir="rtl"
            >
              <SwiperSlide v-for="(item, index) in 8" :key="index">
                <JobRequestCard />
              </SwiperSlide>
            </Swiper>
          </div>
        </div>
      </div>
    </section>
    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 xs:p-6 sm:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  استكشف الوظائف المضافة مؤخرًا
                </h1>
                <p class="text-sm text-[#667178]">
                  خدمات واستشارات تكنولوجيا المعلومات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/jobs-explore"
                class="self-end flex items-center justify-center gap-2 bg-[#f5f5f5] px-4 py-3 rounded-full text-sm border border-[#ecb42b]/0 hover:border-[#ecb42b] transition"
              >
                مشاهدة الكل
                <ChevronLeftIcon width="16" height="16" color="#161614" />
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

          <div class="p-4 xs:p-6 sm:ps-8">
            <Swiper
              :modules="[]"
              :initial-slide="0"
              class="swiper-modal"
              :space-between="20"
              :breakpoints="{
                640: { slidesPerView: 2.1 },
                786: { slidesPerView: 2.3 },
                1024: { slidesPerView: 2.5 },
              }"
              direction="horizontal"
              dir="rtl"
            >
              <SwiperSlide v-for="(item, index) in 8" :key="index">
                <JobCard @open-apply-form="openApplyForm" />
              </SwiperSlide>
            </Swiper>
          </div>
        </div>
      </div>
    </section>

    <!--  -->
    <ApplyJobDialog v-model="applyJobDialog" />
  </div>
</template>
