<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import JobCard from "~/components/JobCard.vue";
import JobRequestCard from "~/components/JobRequestCard.vue";
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import EditIndividualProfile from "~/components/EditIndividualProfile.vue";
import ChevronLeftIcon from "~/components/icons/ChevronLeftIcon.vue";
import FileIcon from "~/components/icons/FileIcon.vue";
import ClockIcon from "~/components/icons/ClockIcon.vue";
import BriefcaseIcon from "~/components/icons/BriefcaseIcon.vue";
import EyeIcon from "~/components/icons/EyeIcon.vue";
import ProfileHeroSection from "~/components/dashboard/home/ProfileHeroSection.vue";
import CompletionSection from "~/components/dashboard/home/CompletionSection.vue";
import HomeSection from "~/components/dashboard/home/HomeSection.vue";
import { buildImageUrl } from "~/services/help";

const applyJobDialog = ref(false);
const editProfile = ref(false);
const selectedJobId = ref(null);
const loading = ref(true);
const userData = ref({ name: '', jobTitle: '', avatarUrl: '' })
const stats = ref({ totalApplications: 0, pendingApps: 0, interviews: 0, signedContracts: 0 })
const profileCompletion = ref(0)
const applications = ref([])
const jobs = ref([])
const route = useRoute()

const avatarSrc = computed(() => buildImageUrl(userData.value.avatarUrl, '/images/avatar.png'));

const statsItems = computed(() => [
  { value: stats.value.pendingApps, label: 'الطلبات المكتملة', icon: FileIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
  { value: stats.value.interviews, label: 'مقابلة عمل', icon: ClockIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.totalApplications, label: 'تقدمت للوظائف', icon: BriefcaseIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.signedContracts, label: 'عروض العمل', icon: EyeIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
])

const findJob = (jobId) => jobs.value.find(j => j.id === jobId)

const openApplyForm = (jobId) => {
  selectedJobId.value = jobId;
  applyJobDialog.value = true;
};

const markJobApplied = (jobId) => {
  const job = findJob(jobId);
  if (job) job.isApplied = true;
};

const loadProfile = async () => {
  const [profileRes, statsRes] = await Promise.allSettled([
    useApi().get('/individuals/profile'),
    useApi().get('/stats/individual'),
  ])
  if (profileRes.status === 'fulfilled' && profileRes.value.data) {
    userData.value = profileRes.value.data
    profileCompletion.value = profileRes.value.data.profileCompletionPct || 0
  }
  if (statsRes.status === 'fulfilled' && statsRes.value.data) stats.value = statsRes.value.data
}

onMounted(async () => {
  loading.value = true
  try {
    const [appsRes, allJobsRes] = await Promise.allSettled([
      useApi().get('/applications?limit=8'),
      useApi().get('/jobs?limit=8'),
      loadProfile(),
    ])
    if (appsRes.status === 'fulfilled' && appsRes.value.data) applications.value = appsRes.value.data.items || appsRes.value.data
    if (allJobsRes.status === 'fulfilled' && allJobsRes.value.data) jobs.value = allJobsRes.value.data.items || allJobsRes.value.data
    for (const res of [appsRes, allJobsRes]) {
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
      :avatar-src="avatarSrc"
      :name="userData.name"
      :subtitle="userData.jobTitle"
      :stats-items="statsItems"
      :columns="4"
      :loading="loading"
    />

    <CompletionSection :percent="profileCompletion" :loading="loading" @action="editProfile = true" />

    <HomeSection
      title="استكشف طلبات العمل الخاص بك"
      subtitle="تابع حالة الوظائف التي تقدمت لها، وابقَ على اطلاع بآخر التحديثات من الجهات"
      :loading="loading"
      :skeleton-count="3"
      skeleton-height="280px"
    >
      <template #action>
        <nuxt-link
          to="/dashboard/job-requests"
          class="self-end flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
        >
          مشاهدة الكل
          <ChevronLeftIcon width="16" height="16" color="#161614" />
        </nuxt-link>
      </template>
      <div class="p-4 xs:p-6 sm:ps-8">
        <Swiper
          :modules="[]"
          :initial-slide="0"
          class="swiper-modal"
          :space-between="20"
          :breakpoints="{
            640: { slidesPerView: 2.1 },
            786: { slidesPerView: 2.3 },
            1024: { slidesPerView: 2.5 },
          }"
          direction="horizontal"
          dir="rtl"
        >
          <SwiperSlide v-for="app in applications" :key="app.id">
            <JobRequestCard :application="app" :job="findJob(app.jobId)" />
          </SwiperSlide>
        </Swiper>
      </div>
    </HomeSection>

    <HomeSection
      title="استكشف الوظائف المضافة مؤخرًا"
      subtitle="خدمات واستشارات تكنولوجيا المعلومات"
      :loading="loading"
      :skeleton-count="3"
      skeleton-height="280px"
    >
      <template #action>
        <nuxt-link
          to="/dashboard/jobs-explore"
          class="self-end flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
        >
          مشاهدة الكل
          <ChevronLeftIcon width="16" height="16" color="#161614" />
        </nuxt-link>
      </template>
      <div class="p-4 xs:p-6 sm:ps-8">
        <Swiper
          :modules="[]"
          :initial-slide="0"
          class="swiper-modal"
          :space-between="20"
          :breakpoints="{
            640: { slidesPerView: 2.1 },
            786: { slidesPerView: 2.3 },
            1024: { slidesPerView: 2.5 },
          }"
          direction="horizontal"
          dir="rtl"
        >
          <SwiperSlide v-for="job in jobs" :key="job.id">
            <JobCard :job="job" @open-apply-form="openApplyForm" />
          </SwiperSlide>
        </Swiper>
      </div>
    </HomeSection>

    <!--  -->
    <ApplyJobDialog v-model="applyJobDialog" :job-id="selectedJobId" @applied="markJobApplied" />
    <EditIndividualProfile v-model="editProfile" @saved="loadProfile" />
  </div>
</template>
