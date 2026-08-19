<script setup>
import Header from "~/components/dashboard/Header.vue";
import SideNav from "~/components/dashboard/SideNav.vue";
useHead({
  titleTemplate: (title) => {
    if (!title) return 'لوحة التحكم | منصة تشريف'
    return `${title} | لوحة التحكم | منصة تشريف`
  },
  meta: [
    { name: "robots", content: "noindex, nofollow" },
  ],
  bodyAttrs: {
    class: "dashboard-layout",
  },
});

const route = useRoute();
const sideOpen = ref(false);
const toggleSideNav = () => {
  sideOpen.value = !sideOpen.value;
};

const paths = ["dashboard-publish-job"];
const isHidden = computed(() => {
  return paths.includes(route.name);
});
</script>

<template>
  <div>
    <a href="#dashboard-content" class="skip-link">تخطى إلى المحتوى الرئيسي</a>
    <Header @toggleSide="toggleSideNav" />
    <SideNav
      :open-side-nav="sideOpen"
      @toggleSide="toggleSideNav"
      :class="{ hide_side: isHidden }"
    />
    <main id="dashboard-content" class="max-wrapper pt-[90px] sm:pt-[90px] md:pt-[110px]" tabindex="-1">
      <div class="main-content" :class="{ hide_side: isHidden }">
        <slot />
      </div>
    </main>
    <Toast />
  </div>
</template>
<style scoped lang="scss">
.main-content {
  width: 100%;
  min-height: 100vh;
  margin-right: auto;
  max-width: calc(100% - 275px);
  transition: max-width 0.3s ease;
  &.hide_side {
    max-width: 100%;
  }
  @media (max-width: 991px) {
    max-width: 100%;
  }
}
</style>
