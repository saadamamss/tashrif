<script setup>
import IndividualInterviews from "~/components/dashboard/individual/interviews.vue";
import CompanyInterviews from "~/components/dashboard/company/interviews.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'المقابلات',
})

const { userType } = useAuth();
</script>

<template>
  <div>
    <IndividualInterviews v-if="userType === 'individual'" />
    <CompanyInterviews v-else-if="userType === 'entity'" />
    <UiLoadingSkeleton v-else :count="8" :columns="2" height="180px" />
  </div>
</template>
