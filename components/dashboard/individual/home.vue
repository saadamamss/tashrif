<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import PercentCircle from "~/components/PercentCircle.vue";
import JobCard from "~/components/JobCard.vue";
import JobRequestCard from "~/components/JobRequestCard.vue";
import ApplyJobDialog from "~/components/ApplyJobDialog.vue";
import EditIndividualProfile from "~/components/EditIndividualProfile.vue";
import ChevronLeftIcon from "~/components/icons/ChevronLeftIcon.vue";
import { buildImageUrl } from "~/services/help";

const applyJobDialog = ref(false);
const editProfile = ref(false);
const selectedJobId = ref(null);
const loading = ref(false);
const userData = ref({ name: '', jobTitle: '', avatarUrl: '' })
const stats = ref({ totalApplications: 0, pendingApps: 0, interviews: 0, signedContracts: 0 })
const profileCompletion = ref(0)
const applications = ref([])
const jobs = ref([])
const route = useRoute()

const avatarSrc = computed(() => buildImageUrl(userData.value.avatarUrl, '/images/avatar.png'));

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
    ])
    await loadProfile()
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
    <section
      class="bg-white profile-statistics rounded-xl shadow-sm overflow-hidden min-h-[495px]"
    >
      <div class="container-fluid">
        <div class="back-image h-[197px] overflow-hidden">
          <img
            src="~/assets/images/profile-back.png"
            class="w-full h-full object-cover"
            alt="back-profile"
          />
        </div>
        <div class="px-8 pb-8">
          <div
            class="flex flex-col sm:flex-row justify-between gap-6 items-center -mt-16"
          >
            <div class="text-center sm:text-right">
              <div
                class="border-4 border-bg-light mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
              >
                <img
                  :src="avatarSrc"
                  class="w-full h-full object-cover"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">{{ userData.name }}</h1>
                <p class="text-base text-gray-500">{{ userData.jobTitle }}</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
            </div>
          </div>

          <div
            class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6 mt-8"
          >
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.pendingApps }}
                </h1>
                <h3 class="text-sm text-muted">الطلبات المكتملة</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>

            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.interviews }}
                </h1>
                <h3 class="text-sm text-muted">مقابلة عمل</h3>
              </div>
              <span>
                <ClockIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.totalApplications }}
                </h1>
                <h3 class="text-sm text-muted">تقدمت للوظائف</h3>
              </div>
              <span>
                <BriefcaseIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.signedContracts }}
                </h1>
                <h3 class="text-sm text-muted">عروض العمل</h3>
              </div>
              <span>
                <EyeIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!--  -->
    <section class="complete-profile bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid p-8">
        <div
          class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
        >
          <div class="flex flex-col sm:flex-row gap-6 max-w-[760px]">
            <PercentCircle
              :percent="profileCompletion"
              gradient-start="#ECB42B05"
              gradient-middle="#ECB42B01"
              gradient-end="#ECB42B"
              :size="94"
            />
            <div>
              <h1 class="text-base font-semibold mb-3">أستكمل ملفك الشخصي</h1>
              <p class="text-sm text-muted">
                استكمالك لملفك الشخصي يساعدنا في ترشيح الوظائف الأنسب لك، ويزيد
                من فرص قبولك لدى الجهات. أضف معلوماتك الشخصية، مؤهلاتك، وخبراتك
                العملية لتظهر بشكل احترافي أمام أصحاب العمل.
              </p>
            </div>
          </div>
          <div class="">
            <button
              @click="editProfile = true"
              class="btn-primary text-nowrap text-sm px-8"
            >
              إستكمل ملفك الشخصى
            </button>
          </div>
        </div>
      </div>
    </section>
    <!--  -->

    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 xs:p-6 sm:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  استكشف طلبات العمل الخاص بك
                </h1>
                <p class="text-sm text-muted">
                  تابع حالة الوظائف التي تقدمت لها، وابقَ على اطلاع بآخر
                  التحديثات من الجهات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/job-requests"
                class="self-end flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                مشاهدة الكل
                <ChevronLeftIcon width="16" height="16" color="#161614" />
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

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
        </div>
      </div>
    </section>
    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 xs:p-6 sm:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  استكشف الوظائف المضافة مؤخرًا
                </h1>
                <p class="text-sm text-muted">
                  خدمات واستشارات تكنولوجيا المعلومات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/jobs-explore"
                class="self-end flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                مشاهدة الكل
                <ChevronLeftIcon width="16" height="16" color="#161614" />
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

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
        </div>
      </div>
    </section>

    <!--  -->
    <ApplyJobDialog v-model="applyJobDialog" :job-id="selectedJobId" @applied="markJobApplied" />
    <EditIndividualProfile v-model="editProfile" @saved="loadProfile" />
  </div>
</template>
