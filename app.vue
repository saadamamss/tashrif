<script setup>
useHead({
  title: "منصة تشريف",
});
</script>
<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>
<style lang="scss">
.skip-link {
  position: absolute;
  top: -100%;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  padding: 8px 16px;
  background: theme('colors.primary.DEFAULT');
  color: #fff;
  border-radius: 0 0 8px 8px;
  font-size: 14px;
  text-decoration: none;
  &:focus {
    top: 0;
  }
}

.layout-enter-active,
.layout-leave-active {
  transition: opacity 300ms ease, filter 300ms ease;

}
.layout-enter-from,
.layout-leave-to {
  opacity: 0;
  filter: blur(0.5rem);
}
/*  */
.page-enter-active,
.page-leave-active {
  transition: opacity 0.4s ease, filter 0.4s ease;
}
.page-enter-from,
.page-leave-to {
  opacity: 0;
  filter: blur(0.5rem);
}
</style>
