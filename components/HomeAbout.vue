<template>
  <section class="about-section bg-white py-12 py-20" id="about">
    <div class="w-full px-4 sm:px-3 xl:px-0">
      <div
        class="grid grid-cols-1 lg:grid-cols-2 gap-16 md:gap-12 items-center"
      >
        <div class="order-2 lg:order-1 max-w-[515px]">
          <span
            class="tag text-[#ECB42B] text-sm font-medium py-1 rounded-full mb-4 inline-block"
            >#منصة_تشريف</span
          >
          <h2
            class="text-2xl md:text-[32px] font-bold text-black mb-6 leading-[1.8] md:leading-[1.8] lg:leading-[1.8]"
          >
            نربط بين الأفراد المؤهلين والجهات المشغّلة عبر منصة رقمية ذكية
          </h2>
          <div
            class="richtext text-base text-[#696C68] leading-[1.8] mb-8 space-y-4"
          >
            <p>
              تعتمد على الموقع الجغرافي والمهارات المطلوبة لضمان تجربة توظيف
              سريعة وفعّالة.
            </p>
          </div>
          <div class="flex justify-start">
            <NuxtLink
              to="/jobs"
              class="btn-primary text-sm px-6 py-3 rounded-lg"
              >استعرض جميع الوظائف</NuxtLink
            >
          </div>
        </div>

        <div class="order-1 lg:order-2 flex justify-center">
          <div
            class="rounded-md px-6 max-w-[700px] w-full"
            style="
              box-shadow: 11.528px 11.528px 57.6402px 3.45841px
                rgba(39, 92, 141, 0.1);
            "
          >
            <div class="border-b-2 py-6">
              <h1 class="text-base font-bold">أنشئ ملفك الشخصي</h1>
            </div>
            <div class="flex flex-wrap items-start justify-center gap-6 py-8">
              <div class="w-24 sm:w-32 w-24 sm:h-32 rounded-full">
                <img
                  src="~/assets/images/avatar-1.png"
                  alt="فريق العمل"
                  class="w-full h-auto rounded-xl object-cover"
                  loading="lazy"
                />
              </div>
              <div class="flex-1 min-w-[250px]">
                <div
                  class="py-4 px-3"
                  style="
                    box-shadow: 8.85445px 13.2817px 44.2723px 2.65634px
                      rgba(69, 69, 69, 0.1);
                  "
                >
                  <div class="items-center flex justify-between mb-3">
                    <span class="text-sm">التعليم</span>
                    <span class="px-3 py-2 bg-[#8C8C8C]/10">
                      <IconsPlusCircle />
                    </span>
                  </div>
                  <div>
                    <svg
                      width="294"
                      height="63"
                      viewBox="0 0 294 63"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                      class="max-w-full"
                    >
                      <rect
                        opacity="0.2"
                        width="292.197"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 0.0447998)"
                        fill="#A8B1BD"
                      />
                      <rect
                        opacity="0.2"
                        width="292.197"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 23.955)"
                        fill="#A8B1BD"
                      />
                      <rect
                        opacity="0.2"
                        width="176.305"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 47.8573)"
                        fill="#A8B1BD"
                      />
                    </svg>
                  </div>
                </div>
                <div
                  class="border-b-2 py-4 px-3 items-center flex justify-between"
                >
                  <span class="text-sm">المهارات العملية</span>
                  <span class="px-3 py-2 bg-[#8C8C8C]/10">
                    <PlusCircle />
                  </span>
                </div>
                <div class="items-center py-4 px-3 flex justify-between">
                  <span class="text-sm">الخبرات المهنية</span>
                  <span class="px-3 py-2 bg-[#8C8C8C]/10">
                    <PlusCircle />
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="divider py-20"></div>

      <div
        class="grid grid-cols-1 lg:grid-cols-2 gap-16 md:gap-12 items-center"
      >
        <div class="order-2 lg:order-2 max-w-[515px]">
          <span
            class="tag text-[#ECB42B] text-sm font-medium py-1 rounded-full mb-4 inline-block"
            >#منصة_تشريف</span
          >
          <h2
            class="text-2xl md:text-[32px] font-bold text-black mb-6 leading-[1.8] md:leading-[1.8] lg:leading-[1.8]"
          >
            تمكين التوظيف الموسمي عبر تجربة رقمية سلسة
          </h2>
          <div
            class="richtext text-base text-[#696C68] leading-[1.8] mb-8 space-y-4"
          >
            <p>
              تضمن للباحثين عن العمل الوصول للفرص المناسبة، وللجهات المشغّلة
              الوصول إلى الكفاءات المؤهلة بسرعة وفعالية.
            </p>
          </div>
          <div class="flex justify-start">
            <NuxtLink
              to="/jobs"
              class="btn-primary text-sm px-6 py-3 rounded-lg"
              >استعرض جميع الوظائف</NuxtLink
            >
          </div>
        </div>

        <div class="order-1 lg:order-1 flex justify-center">
          <div
            class="rounded-md px-6 max-w-[700px] w-full"
            style="
              box-shadow: 11.528px 11.528px 57.6402px 3.45841px
                rgba(39, 92, 141, 0.1);
            "
          >
            <div class="border-b-2 py-6">
              <h1 class="text-base font-bold">أنشئ ملفك الشخصي</h1>
            </div>
            <div class="flex flex-wrap items-start justify-center gap-6 py-8">
              <div class="w-24 sm:w-32 w-24 sm:h-32 rounded-full">
                <img
                  src="~/assets/images/avatar-2.png"
                  alt="فريق العمل"
                  class="w-full h-auto rounded-xl object-cover"
                  loading="lazy"
                />
              </div>
              <div class="flex-1 min-w-[250px]">
                <div
                  class="py-4 px-3"
                  style="
                    box-shadow: 8.85445px 13.2817px 44.2723px 2.65634px
                      rgba(69, 69, 69, 0.1);
                  "
                >
                  <div class="items-center flex justify-between mb-3">
                    <span class="text-sm">التعليم</span>
                    <span class="px-3 py-2 bg-[#8C8C8C]/10">
                      <PlusCircle />
                    </span>
                  </div>
                  <div>
                    <svg
                      width="294"
                      height="63"
                      viewBox="0 0 294 63"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                      class="max-w-full"
                    >
                      <rect
                        opacity="0.2"
                        width="292.197"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 0.0447998)"
                        fill="#A8B1BD"
                      />
                      <rect
                        opacity="0.2"
                        width="292.197"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 23.955)"
                        fill="#A8B1BD"
                      />
                      <rect
                        opacity="0.2"
                        width="176.305"
                        height="14.1671"
                        rx="2"
                        transform="matrix(-1 0 0 1 293.029 47.8573)"
                        fill="#A8B1BD"
                      />
                    </svg>
                  </div>
                </div>
                <div
                  class="border-b-2 py-4 px-3 items-center flex justify-between"
                >
                  <span class="text-sm">المهارات العملية</span>
                  <span class="px-3 py-2 bg-[#8C8C8C]/10">
                    <PlusCircle />
                  </span>
                </div>
                <div class="items-center py-4 px-3 flex justify-between">
                  <span class="text-sm">الخبرات المهنية</span>
                  <span class="px-3 py-2 bg-[#8C8C8C]/10">
                    <IconsPlusCircle />
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup></script>

<style lang="scss" scoped>
.about-section {
  img {
    mix-blend-mode: darken;
  }
}
</style>
