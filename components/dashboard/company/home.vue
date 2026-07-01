<script setup>
import "swiper/css";
import JobOfferCard from "~/components/JobOfferCard.vue";
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
                class="border-4 border-bg-light mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
              >
                <img
                  src="/images/partner-3.svg"
                  class="w-full h-full"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">شركة نسك لخدمات الحجاج</h1>
                <p class="text-base text-gray-500">لخدمات الحج والعمرة</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
            </div>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-3 gap-6 mt-8">
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  24
                </h1>
                <h3 class="text-sm text-muted">وظائفى المنشورة</h3>
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
                <h3 class="text-sm text-muted">مقابلات العمل</h3>
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
                  12
                </h1>
                <h3 class="text-sm text-muted">عروض العمل</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 sm:p-6 md:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  وظائفي المنشورة
                </h1>
                <p class="text-sm text-muted">
                  تابع حالة الوظائف التي نشرتها ، وابقَ على اطلاع بآخر التحديثات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/publish-job"
                class="self-end btn-primary text-sm"
              >
                نشر وظيفة جديدة
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

          <div class="px-4 sm:px-6 md:px-8">
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              <JobOfferCard
                v-for="i in 12"
                class="border border-[#fff]/0 hover:border-primary transition"
                role="button"
                @click="$router.push('/dashboard/published-jobs/details')"
              />
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
