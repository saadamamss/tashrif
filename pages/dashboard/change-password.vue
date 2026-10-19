<template>
  <div class="px-4 lg:px-0 h-full w-full">
    <Breadcrumbs :items="breadcrumbs" />

    <div class="mt-5 bg-white rounded-2xl p-8  shadow-xl">
      <ChangePassword />

    </div>
  </div>
</template>

<script setup lang="ts">
import Breadcrumbs from '~/components/elements/Breadcrumbs.vue';

definePageMeta({
  layout: "dashboard",
  middleware: ["auth"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'المتقدمون للوظيفة',
})

const breadcrumbs = computed(() => [
  {
    label: "الملف الشخصى",
    to: "/dashboard/profile",
    active: true,
  },
  {
    label: 'تغيير كلمة السر',
    active: false,
  },
]);
</script>
