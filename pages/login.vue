<script setup>
import LoginDialog from "~/components/LoginDialog.vue";

useHead({
  title: "تسجيل الدخول",
});

definePageMeta({
  layout: "login-layout",
  middleware: ["auth"],
  meta: { guest: true },
});
const { showModal, closeModal, isLoginModalShow } = useLoginModal();

onMounted(() => {
  showModal();
});

onBeforeUnmount(() => {
  closeModal();
});
</script>

<template>
  <div>
    <LoginDialog />
  </div>
</template>
