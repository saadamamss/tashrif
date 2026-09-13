<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 p-4">
    <ChangePassword />
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  middleware: ["auth"],
});
</script>
