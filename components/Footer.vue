<script setup>
import Logo from "./icons/logo.vue";
const { currentSection, goToSection } = useScrollSpy();
</script>

<template>
  <footer class="pt-10">
    <div class="border rounded-t-[40px] bg-white border-b-0 py-10">
      <div class="flex items-center justify-center space-x-4 space-x-reverse">
        <span>
          <Logo />
        </span>
      </div>

      <div class="flex justify-center py-12 px-8">
        <div
          class="nav-menu-horizontal flex flex-wrap items-center justify-center gap-6"
        >
          <a
            href="#home"
            :class="{ active: currentSection == 'home' }"
            @click.stop.prevent="goToSection"
            class="text-sm text-[#25343E] transition-colors"
            >الرئيسية</a
          >
          <a
            href="#about"
            @click.stop.prevent="goToSection"
            :class="{ active: currentSection == 'about' }"
            class="text-sm text-[#25343E] transition-colors"
            >عن المنصة</a
          >
          <nuxt-link
            to="/jobs"
            :class="{ active: currentSection == 'jobs' }"
            class="text-sm text-[#25343E] transition-colors"
            >الوظائف</nuxt-link
          >
          <a
            href="#partners"
            @click.stop.prevent="goToSection"
            :class="{ active: currentSection == 'partners' }"
            class="text-sm text-[#25343E] transition-colors"
            >الأخبار</a
          >
          <a
            href="#contact"
            @click.stop.prevent="goToSection"
            :class="{ active: currentSection == 'contact' }"
            class="text-sm text-[#25343E] transition-colors"
            >تواصل معنا</a
          >
        </div>
      </div>

      <div class="text-center">
        <p class="text-sm text-gray-400">جميع الحقوق محفوظة لتشريف © 2025</p>
      </div>
    </div>
  </footer>
</template>

<style lang="scss" scoped>
@use "~/assets/scss/components/header";
</style>
