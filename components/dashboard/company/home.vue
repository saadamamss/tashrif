<script setup>
import "swiper/css";
import JobOfferCard from "~/components/JobOfferCard.vue";
import { buildImageUrl } from "~/services/help";

const loading = ref(false);
const companyName = ref('')
const companyField = ref('')
const logoUrl = ref('')
const stats = ref({ totalJobs: 0, activeJobs: 0, totalApplicants: 0 })
const jobs = ref([])

const logoSrc = computed(() => buildImageUrl(logoUrl.value, '/images/partner-3.svg'));

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
                  :src="logoSrc"
                  class="w-full h-full object-cover"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">{{ companyName }}</h1>
                <p class="text-base text-gray-500">{{ companyField }}</p>
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

          <div class="grid grid-cols-1 sm:grid-cols-3 gap-6 mt-8">
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.totalJobs }}
                </h1>
                <h3 class="text-sm text-muted">وظائفى المنشورة</h3>
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
                  {{ stats.activeJobs }}
                </h1>
                <h3 class="text-sm text-muted">الوظائف النشطة</h3>
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
                  {{ stats.totalApplicants }}
                </h1>
                <h3 class="text-sm text-muted">إجمالي المتقدمين</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>
          </div>

          <NuxtLink
            to="/dashboard/analytics"
            class="mt-6 flex items-center justify-center gap-2 bg-primary text-white px-6 py-3 rounded-full text-sm font-medium hover:bg-primary/90 transition"
          >
            عرض الإحصائيات التفصيلية
          </NuxtLink>
        </div>
      </div>
    </section>

    <section class="new-jobs bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid">
        <div class="pb-8">
          <div class="p-4 sm:p-6 md:p-8">
            <div
              class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
            >
              <div class="">
                <h1 class="text-base md:text-lg font-semibold mb-3">
                  وظائفي المنشورة
                </h1>
                <p class="text-sm text-muted">
                  تابع حالة الوظائف التي نشرتها ، وابقَ على اطلاع بآخر التحديثات
                </p>
              </div>

              <nuxt-link
                to="/dashboard/publish-job"
                class="self-end btn-primary text-sm"
              >
                نشر وظيفة جديدة
              </nuxt-link>
            </div>

            <div class="py-4">
              <hr />
            </div>
          </div>

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
        </div>
      </div>
    </section>
  </div>
</template>
