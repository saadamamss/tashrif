<script setup>
import HomeHero from "~/components/home-hero.vue";
import HomeAbout from "~/components/home-about.vue";
import HomeJobs from "~/components/home-jobs.vue";
import HomePartners from "~/components/home-partners.vue";
import HomeContact from "~/components/home-contact.vue";
import ApplyJobDialog from "~/components/apply-job-dialog.vue";

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