<template>
  <section class="partners-section py-12 md:py-16" id="partners">
    <div class="px-4 sm:px-3 xl:px-0">
      <div class="mb-10 md:mb-14">
        <h2 class="text-2xl md:text-3xl font-bold text-gray-800 mb-4">
          <span class="text-icon-muted font-thin">اكتشف</span> الجهات المشغلة
        </h2>
        <p class="text-icon-muted max-w-2xl text-sm leading-[1.8]">
          تعرف على الجهات التي يمكنك العمل معها خلال موسم الحج والعمرة، وابدأ رحلتك المهنية مع كيانات موثوقة ومتميزة في خدمة ضيوف الرحمن.
        </p>
      </div>

      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-4 gap-6">
        <div v-for="item in partners" :key="item" class="partner-card bg-bg-light/55 p-6 rounded-xl flex items-center justify-center h-40">
          <img :src="`/images/${item}`" loading="lazy" />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
const partners = [
  "partner-2.svg",
  "partner-3.svg",
  "partner-4.svg",
  "partner-5.svg",
  "partner-8.svg",
  "partner-7.svg",
  "partner-1.svg",
  "partner-6.svg",
];
</script>

<style lang="scss" scoped>
.partners-section {
  img {
    mix-blend-mode: darken;
  }
}
</style>