<script setup lang="ts">
import DemographicsChart from '~/components/analytics/DemographicsChart.vue'
import FunnelChart from '~/components/analytics/FunnelChart.vue'
import StatCard from '~/components/analytics/StatCard.vue'
import TimelineChart from '~/components/analytics/TimelineChart.vue'
import { useAnalytics } from '~/composables/useAnalytics'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth', 'entity'],
  meta: { requiresAuth: true },
})

useHead({ title: 'الإحصائيات' })

const { analytics, loading, error, fetchAnalytics } = useAnalytics()

onMounted(fetchAnalytics)
</script>

<template>
  <div class="px-4 lg:px-0 space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">الإحصائيات</h1>
      <button v-if="error" class="text-sm text-primary underline" @click="fetchAnalytics">إعادة المحاولة</button>
    </div>

    <UiLoadingSkeleton v-if="loading" :count="5" :columns="3" height="110px" />
    <UiErrorState v-else-if="error" :message="error" @retry="fetchAnalytics" />

    <template v-else-if="analytics">
      <div class="grid grid-cols-2 lg:grid-cols-5 gap-4">
        <StatCard label="إجمالي الوظائف" :value="analytics.totalJobs" />
        <StatCard label="الوظائف النشطة" :value="analytics.activeJobs" />
        <StatCard label="إجمالي المتقدمين" :value="analytics.totalApplications" />
        <StatCard label="معدل التوظيف" :value="`${analytics.conversionRate}%`" accent />
        <StatCard
          label="متوسط مدة التوظيف"
          :value="analytics.averageTimeToHireDays !== null ? `${analytics.averageTimeToHireDays} يوم` : '—'"
        />
      </div>

      <FunnelChart :by-status="analytics.applicationsByStatus" />

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="bg-white rounded-xl border p-5 sm:p-6">
          <h2 class="text-base font-semibold text-dark mb-4">أكثر الوظائف طلباً</h2>
          <div v-if="!analytics.topJobs.length" class="text-center text-sm text-muted py-6">لا توجد وظائف بعد</div>
          <div v-else class="space-y-3">
            <div
              v-for="job in analytics.topJobs"
              :key="job.jobId"
              class="flex items-center justify-between rounded-lg border bg-bg-light/50 px-4 py-3"
            >
              <div class="min-w-0 flex-1">
                <p class="text-sm font-medium text-dark truncate">{{ job.title }}</p>
                <p class="text-xs text-muted mt-0.5">{{ job.applicationCount }} متقدم · {{ job.hiredCount }} توظيف</p>
              </div>
              <NuxtLink :to="`/dashboard/published-jobs/details?id=${job.jobId}`" class="text-xs text-primary font-medium shrink-0 ms-3 hover:underline">عرض</NuxtLink>
            </div>
          </div>
        </div>
        <TimelineChart :days="analytics.applicationsOverTime" />
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <DemographicsChart title="حسب الجنس" kind="gender" :data="analytics.applicantDemographics.byGender" />
        <DemographicsChart title="حسب الجنسية" kind="nationality" :data="analytics.applicantDemographics.byNationality" />
      </div>
    </template>
  </div>
</template>
