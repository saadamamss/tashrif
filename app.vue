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
