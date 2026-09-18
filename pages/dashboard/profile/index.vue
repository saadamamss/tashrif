<script setup>
import { defineAsyncComponent, computed } from "vue";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "user-type"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'الملف الشخصي',
})

const { userType } = useAuth();

const currentHomeComponent = computed(() => {
  if (!userType.value) return null;

  return defineAsyncComponent(() => {
    // Admin has no individual/entity profile endpoints — show a read-only
    // profile card instead of loading the entity profile (which 403s).
    if (userType.value === "admin") {
      return import("~/components/dashboard/admin/profile.vue");
    }
    return userType.value === "individual"
      ? import("~/components/dashboard/individual/profile.vue")
      : import("~/components/dashboard/company/profile.vue");
  });
});
</script>

<template>
  <div>
    <Suspense>
      <component :is="currentHomeComponent" v-if="currentHomeComponent" />
      <template #fallback>
        <UiLoadingSkeleton :count="3" height="300px" />
      </template>
    </Suspense>
  </div>
</template>
