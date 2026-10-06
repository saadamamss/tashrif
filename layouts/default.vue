<script setup>
import Footer from "~/components/Footer.vue";
import Header from "~/components/Header.vue";
import LoginDialog from "~/components/LoginDialog.vue";
import RegisterDialog from "~/components/RegisterDialog.vue";

useHead({
  titleTemplate: (title) => title ? `${title} | منصة تشريف` : 'منصة تشريف',
  meta: [
    { name: "description", content: "منصة توظيف موسمي لخدمة ضيوف الرحمن في موسم الحج والعمرة" },
    { property: "og:title", content: "منصة تشريف | وظائف موسمية للحج والعمرة" },
    { property: "og:description", content: "منصة توظيف موسمي لخدمة ضيوف الرحمن في موسم الحج والعمرة" },
    { property: "og:type", content: "website" },
    { property: "og:locale", content: "ar_SA" },
    { property: "og:image", content: "/og-image.svg" },
    { name: "twitter:card", content: "summary_large_image" },
  ],
})

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
    <a href="#main-content" class="skip-link">تخطى إلى المحتوى الرئيسي</a>
    <Header @open-register="openRegister" @open-login="openLogin" />
    <main id="main-content" class="pt-[82px] sm:pt-[94px] md:pt-[112px]" tabindex="-1">
      <slot />
    </main>
    <Footer />

    <!--  -->
    <RegisterDialog v-model="isRegisterOpen" />
    <LoginDialog />
    <Toast />
  </div>
</template>
