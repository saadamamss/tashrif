<script setup>
import "swiper/css";
import JobOfferCard from "~/components/JobOfferCard.vue";
import BriefcaseIcon from "~/components/icons/BriefcaseIcon.vue";
import ClockIcon from "~/components/icons/ClockIcon.vue";
import FileIcon from "~/components/icons/FileIcon.vue";
import ProfileHeroSection from "~/components/dashboard/home/ProfileHeroSection.vue";
import HomeSection from "~/components/dashboard/home/HomeSection.vue";
import { buildImageUrl } from "~/services/help";

const loading = ref(true);
const companyName = ref('')
const companyField = ref('')
const logoUrl = ref('')
const stats = ref({ totalJobs: 0, activeJobs: 0, totalApplicants: 0 })
const jobs = ref([])

const logoSrc = computed(() => buildImageUrl(logoUrl.value, '/images/partner-3.svg'));

const statsItems = computed(() => [
  { value: stats.value.totalJobs, label: 'وظائفى المنشورة', icon: BriefcaseIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.activeJobs, label: 'الوظائف النشطة', icon: ClockIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.totalApplicants, label: 'إجمالي المتقدمين', icon: FileIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
])

onMounted(async () => {
  loading.value = true
  try {
    const [profileRes, statsRes, jobsRes] = await Promise.allSettled([
      useApi().get('/entities/profile'),
      useApi().get('/stats/entity'),
      useApi().get('/jobs/mine?limit=12'),
    ])
    if (profileRes.status === 'fulfilled' && profileRes.value.data) {
      companyName.value = profileRes.value.data.name
      companyField.value = profileRes.value.data.companyField || ''
      logoUrl.value = profileRes.value.data.logoUrl || ''
    }
    if (statsRes.status === 'fulfilled' && statsRes.value.data) stats.value = statsRes.value.data
    if (jobsRes.status === 'fulfilled' && jobsRes.value.data) jobs.value = jobsRes.value.data.items || jobsRes.value.data
    for (const res of [profileRes, statsRes, jobsRes]) {
      if (res.status === 'fulfilled' && res.value?.error) {
        useToast().show(res.value.error, "error")
      }
    }
  } finally {
    loading.value = false
  }
})
</script>
<template>
  <div class="px-4 lg:px-0">
    <ProfileHeroSection
      :avatar-src="logoSrc"
      :name="companyName"
      :subtitle="companyField"
      :stats-items="statsItems"
      :columns="3"
      :loading="loading"
    >
      <template #footer>
        <NuxtLink
          to="/dashboard/analytics"
          class="mt-6 flex items-center justify-center gap-2 bg-primary text-white px-6 py-3 rounded-full text-sm font-medium hover:bg-primary/90 transition"
        >
          عرض الإحصائيات التفصيلية
        </NuxtLink>
      </template>
    </ProfileHeroSection>

    <HomeSection
      title="وظائفي المنشورة"
      subtitle="تابع حالة الوظائف التي نشرتها ، وابقَ على اطلاع بآخر التحديثات"
      :loading="loading"
      :skeleton-count="3"
      skeleton-height="200px"
    >
      <template #action>
        <nuxt-link to="/dashboard/publish-job" class="self-end btn-primary text-sm">
          نشر وظيفة جديدة
        </nuxt-link>
      </template>
      <div class="px-4 sm:px-6 md:px-8">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <JobOfferCard
            v-for="job in jobs"
            :key="job.id"
            :job="job"
            class="border border-[#fff]/0 hover:border-primary transition"
            role="button"
            @click="$router.push(`/dashboard/published-jobs/details?id=${job.id}`)"
          />
        </div>
      </div>
    </HomeSection>
  </div>
</template>
