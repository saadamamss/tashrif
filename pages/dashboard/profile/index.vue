<script setup>
import IndividualProfile from "~/components/dashboard/individual/profile.vue";
import CompanyProfile from "~/components/dashboard/company/profile.vue";
import AdminProfile from "~/components/dashboard/admin/profile.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'الملف الشخصي',
})

const { userType } = useAuth();
</script>

<template>
  <div>
    <IndividualProfile v-if="userType === 'individual'" />
    <CompanyProfile v-else-if="userType === 'entity'" />
    <AdminProfile v-else-if="userType === 'admin'" />
    <UiLoadingSkeleton v-else :count="3" height="300px" />
  </div>
</template>
