<script setup>
import HomeHero from "~/components/HomeHero.vue";
import HomeAbout from "~/components/HomeAbout.vue";
import HomeJobs from "~/components/HomeJobs.vue";
import HomePartners from "~/components/HomePartners.vue";
import HomeContact from "~/components/HomeContact.vue";
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";

useHead({
  title: 'منصة تشريف',
  meta: [
    { name: "description", content: "منصة توظيف موسمي لخدمة ضيوف الرحمن في موسم الحج والعمرة. اكتشف الوظائف الموسمية المتاحة وتقدم الآن." },
    { property: "og:image", content: "/og-image.svg" },
  ],
})

useScrollSpy();

definePageMeta({
  middleware: [],
});

const { isAuthenticated } = useAuth();
const { showModal } = useLoginModal();

const applyJobDialog = ref(false);
const selectedJobId = ref(null);
const openApplyForm = (jobId) => {
  if (isAuthenticated.value) {
    selectedJobId.value = jobId;
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

    <ApplyJobDialog v-model="applyJobDialog" :job-id="selectedJobId" v-if="isAuthenticated" />
  </div>
</template>

<style lang="scss" scoped>
.divider {
  height: 1px;
}
</style>