<script setup>
import HomeHero from "~/components/HomeHero.vue";
import HomeAbout from "~/components/HomeAbout.vue";
import HomeJobs from "~/components/HomeJobs.vue";
import HomePartners from "~/components/HomePartners.vue";
import HomeContact from "~/components/HomeContact.vue";
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";

useScrollSpy();

definePageMeta({
  middleware: ["auth-global"],
  auth: false,
});

const authStore = useAuthStore();
const { showModal } = useLoginModal();

const applyJobDialog = ref(false);
const openApplyForm = () => {
  if (authStore.isAuthenticated) {
    applyJobDialog.value = true;
    return;
  }
  showModal();
};
</script>

<template>
  <div class="home-page">
    <div class="max-wrapper mx-auto">
      <HomeHero />
      <HomeAbout />
    </div>
    
    <HomeJobs @apply="openApplyForm" />
    
    <div class="max-wrapper mx-auto">
      <HomePartners />
      <HomeContact />
    </div>

    <ApplyJobDialog v-model="applyJobDialog" v-if="authStore.isAuthenticated" />
  </div>
</template>

<style lang="scss" scoped>
.divider {
  height: 1px;
}
</style>