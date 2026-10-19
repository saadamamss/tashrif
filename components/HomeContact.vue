<template>
  <section class="contact-section" id="contact">
    <div class="section-content flex items-center">
      <div class="text-center text-white max-w-[650px] mx-auto px-6">
        <h1 class="text-2xl md:text-3xl lg:text-4xl font-bold mb-6">موسـم جديد.. فرصة جديدة</h1>
        <p class="text-base mb-10 leading-[1.8]">
          سجّل الآن وكن جزءًا من فرق العمل الموسمية التي تخدم ملايين الحجاج والمعتمرين. وظائف في مكة، المدينة، المشاعر المقدسة – برواتب مجزية وتجربة إنسانية لا تُنسى.
        </p>
        <div class="flex justify-center">
          <nuxt-link to="#" class="block btn-primary text-black text-sm hover:text-white hover:bg-white/10">قدم علي الوظيفة</nuxt-link>
        </div>
      </div>
    </div>
  </section>
</template>

<style lang="scss" scoped>
.contact-section {
  .section-content {
    min-height: 455px;
    background: linear-gradient(179.24deg, rgba(0, 0, 0, 0.7) 12.77%, rgba(0, 0, 0, 0) 139.66%), url("~/assets/images/contact-section.png");
    background-repeat: no-repeat;
    background-size: cover;
    background-position: center;
    border-radius: 40px;
  }
}
</style>