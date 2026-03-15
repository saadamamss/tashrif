<script setup>
import Footer from "~/components/Footer.vue";
import Header from "~/components/header.vue";
import LoginDialog from "~/components/LoginDialog.vue";
import RegisterDialog from "~/components/RegisterDialog.vue";
const { showModal, closeModal } = useLoginModal();
const isRegisterOpen = ref(false);
const isLoginOpen = ref(false);

const openRegister = () => {
  isRegisterOpen.value = true;
};
const openLogin = () => {
  showModal();
};

const closeRegister = () => {
  isRegisterOpen.value = false;
};

const handleConfirm = () => {
  alert("تم التأكيد!");
  closeModal();
};
</script>
<template>
  <div>
    <Header @open-register="openRegister" @open-login="openLogin" />
    <main class="pt-[82px] sm:pt-[94px] md:pt-[112px]">
      <slot />
    </main>
    <Footer />

    <!--  -->
    <RegisterDialog v-model="isRegisterOpen" />
    <LoginDialog />
  </div>
</template>
