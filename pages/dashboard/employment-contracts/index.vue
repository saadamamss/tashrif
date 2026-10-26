<script setup>
import IndividualContracts from "~/components/dashboard/individual/contracts.vue";
import CompanyContracts from "~/components/dashboard/company/contracts.vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'عقود التوظيف',
})

const { userType } = useAuth();
</script>

<template>
  <div>
    <IndividualContracts v-if="userType === 'individual'" />
    <CompanyContracts v-else-if="userType === 'entity'" />
    <UiLoadingSkeleton v-else :count="8" :columns="2" height="180px" />
  </div>
</template>
