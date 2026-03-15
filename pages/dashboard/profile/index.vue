<script setup>
import { defineAsyncComponent, computed } from "vue";
import { useUserStore } from "~/stores/userStore";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global","auth-guard", "user-type"],
});

const userStore = useUserStore();

// Computed property for better reactivity
const currentHomeComponent = computed(() => {
  if (!userStore.userType) return null; // Handle loading state

  return defineAsyncComponent(() =>
    userStore.userType === "individual"
      ? import("~/components/dashboard/individual/profile.vue")
      : import("~/components/dashboard/company/profile.vue")
  );
});
</script>

<template>
  <div>
    <Suspense>
      <component :is="currentHomeComponent" v-if="currentHomeComponent" />
    </Suspense>
  </div>
</template>
