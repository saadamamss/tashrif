<script setup>
import IndividualHome from "~/components/dashboard/individual/home.vue";
import CompanyHome from "~/components/dashboard/company/home.vue";
import AdminHome from "~/components/dashboard/admin/home.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'الرئيسية',
})

const { userType } = useAuth();
</script>

<template>
  <div>
    <IndividualHome v-if="userType === 'individual'" />
    <CompanyHome v-else-if="userType === 'entity'" />
    <AdminHome v-else-if="userType === 'admin'" />
    <UiLoadingSkeleton v-else :count="8" :columns="2" height="180px" />
  </div>
</template>
