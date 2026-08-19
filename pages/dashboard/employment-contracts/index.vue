<script setup>
import { defineAsyncComponent, computed } from "vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'عقود التوظيف',
})

const { userType } = useAuth();

const currentHomeComponent = computed(() => {
  if (!userType.value) return null;

  return defineAsyncComponent(() =>
    userType.value === "individual"
      ? import("~/components/dashboard/individual/contracts.vue")
      : import("~/components/dashboard/company/contracts.vue")
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
