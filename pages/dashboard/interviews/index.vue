<script setup>
import { defineAsyncComponent, computed } from "vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'المقابلات',
})

const { userType } = useAuth();

const currentHomeComponent = computed(() => {
  if (!userType.value) return null;

  return defineAsyncComponent(() =>
    userType.value === "individual"
      ? import("~/components/dashboard/individual/interviews.vue")
      : import("~/components/dashboard/company/interviews.vue")
  );
});
</script>

<template>
  <div>
    <Suspense>
      <component :is="currentHomeComponent" v-if="currentHomeComponent" />
      <template #fallback>
        <UiLoadingSkeleton :count="8" :columns="2" height="180px" />
      </template>
    </Suspense>
  </div>
</template>
