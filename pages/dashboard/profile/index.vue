<script setup>
import { defineAsyncComponent, computed } from "vue";
import { useAuthStore } from "~/stores/authStore";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global","auth-guard", "user-type"],
});

const authStore = useAuthStore();

const currentHomeComponent = computed(() => {
  if (!authStore.userType) return null;

  return defineAsyncComponent(() =>
    authStore.userType === "individual"
      ? import("~/components/dashboard/individual/profile.vue")
      : import("~/components/dashboard/company/profile.vue")
  );
});
</script>

<template>
  <div>
    <Suspense>
      <component :is="currentHomeComponent" v-if="currentHomeComponent" />
      <template #fallback>
        <LoadingSkeleton :rows="3" height="300px" />
      </template>
    </Suspense>
  </div>
</template>
