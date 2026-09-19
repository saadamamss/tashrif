<script setup lang="ts">
import BarChart from '~/components/analytics/BarChart.vue'
import type { EntityAnalytics } from '~/types/analytics'
import { statusLabels } from '~/services/statusLabels'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth', 'entity'],
  meta: { requiresAuth: true },
})

useHead({ title: 'الإحصائيات' })

const loading = ref(true)
const error = ref<string | null>(null)
const analytics = ref<EntityAnalytics | null>(null)

const statusOrder = ['new', 'shortlisted', 'interview', 'contract_sent', 'accepted', 'refused', 'withdrawn'] as const

const statusColors: Record<string, string> = {
  new: '#ECB42B',
  shortlisted: '#3B82F6',
  interview: '#8B5CF6',
  contract_sent: '#F59E0B',
  accepted: '#10B981',
  refused: '#EF4444',
  withdrawn: '#9CA3AF',
}

const funnelData = computed(() => {
  if (!analytics.value) return []
  return statusOrder.map((key) => ({
    label: statusLabels[key] || key,
    value: analytics.value!.applicationsByStatus[key] ?? 0,
    color: statusColors[key],
  }))
})

const overTimeMax = computed(() => {
  if (!analytics.value?.applicationsOverTime.length) return 1
  return Math.max(...analytics.value.applicationsOverTime.map((d) => d.count), 1)
})

const totalDemographics = computed(() => {
  if (!analytics.value) return 0
  return Object.values(analytics.value.applicantDemographics.byGender).reduce((a, b) => a + b, 0)
})

const genderLabels: Record<string, string> = {
  male: 'ذكر',
  female: 'أنثى',
  'غير محدد': 'غير محدد',
}

function genderLabel(key: string): string {
  return genderLabels[key] || key
}

function genderPct(count: number): string {
  const total = totalDemographics.value
  if (!total) return '0%'
  return `${Math.round((count / total) * 100)}%`
}

async function fetchAnalytics() {
  loading.value = true
  error.value = null
  try {
    const res = await useApi().get<EntityAnalytics>('/analytics/entity')
    if (res.error) {
      error.value = res.error
      return
    }
    analytics.value = res.data as EntityAnalytics
  } catch (e: unknown) {
    const msg = e instanceof Error ? e.message : 'حدث خطأ أثناء تحميل البيانات'
    error.value = msg
  } finally {
    loading.value = false
  }
}

onMounted(fetchAnalytics)
</script>

<template>
  <div class="px-4 lg:px-0 space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">الإحصائيات</h1>
      <button
        v-if="error"
        class="text-sm text-primary underline"
        @click="fetchAnalytics"
      >
        إعادة المحاولة
      </button>
    </div>

    <!-- Loading -->
    <UiLoadingSkeleton v-if="loading" :count="5" :columns="3" height="110px" />

    <!-- Error -->
    <UiErrorState v-else-if="error" :message="error" @retry="fetchAnalytics" />

    <!-- Content -->
    <template v-else-if="analytics">
      <!-- 1. Summary cards -->
      <div class="grid grid-cols-2 lg:grid-cols-5 gap-4">
        <div class="bg-white rounded-xl border p-5 text-center">
          <p class="text-3xl font-extrabold text-dark">{{ analytics.totalJobs }}</p>
          <p class="text-xs text-muted mt-1">إجمالي الوظائف</p>
        </div>
        <div class="bg-white rounded-xl border p-5 text-center">
          <p class="text-3xl font-extrabold text-dark">{{ analytics.activeJobs }}</p>
          <p class="text-xs text-muted mt-1">الوظائف النشطة</p>
        </div>
        <div class="bg-white rounded-xl border p-5 text-center">
          <p class="text-3xl font-extrabold text-dark">{{ analytics.totalApplications }}</p>
          <p class="text-xs text-muted mt-1">إجمالي المتقدمين</p>
        </div>
        <div class="bg-white rounded-xl border p-5 text-center">
          <p class="text-3xl font-extrabold text-primary">{{ analytics.conversionRate }}%</p>
          <p class="text-xs text-muted mt-1">معدل التوظيف</p>
        </div>
        <div class="bg-white rounded-xl border p-5 text-center">
          <p class="text-3xl font-extrabold text-dark">
            {{ analytics.averageTimeToHireDays !== null ? `${analytics.averageTimeToHireDays} يوم` : '—' }}
          </p>
          <p class="text-xs text-muted mt-1">متوسط مدة التوظيف</p>
        </div>
      </div>

      <!-- 2. Hiring funnel -->
      <div class="bg-white rounded-xl border p-5 sm:p-6">
        <h2 class="text-base font-semibold text-dark mb-4">مسار التوظيف</h2>
        <BarChart :data="funnelData" />
      </div>

      <!-- 3. Top jobs + 4. Over time (side by side on large screens) -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Top jobs -->
        <div class="bg-white rounded-xl border p-5 sm:p-6">
          <h2 class="text-base font-semibold text-dark mb-4">أكثر الوظائف طلباً</h2>
          <div v-if="!analytics.topJobs.length" class="text-center text-sm text-muted py-6">
            لا توجد وظائف بعد
          </div>
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
              <NuxtLink
                :to="`/dashboard/published-jobs/details?id=${job.jobId}`"
                class="text-xs text-primary font-medium shrink-0 ms-3 hover:underline"
              >
                عرض
              </NuxtLink>
            </div>
          </div>
        </div>

        <!-- Applications over time -->
        <div class="bg-white rounded-xl border p-5 sm:p-6">
          <h2 class="text-base font-semibold text-dark mb-4">الطلبات خلال 30 يوماً</h2>
          <div v-if="!analytics.applicationsOverTime.length" class="text-center text-sm text-muted py-6">
            لا توجد بيانات
          </div>
          <div v-else class="flex items-end gap-[2px] h-32" role="img" aria-label="الطلبات خلال 30 يوماً">
            <div
              v-for="day in analytics.applicationsOverTime"
              :key="day.date"
              class="flex-1 rounded-t transition-all"
              :class="day.count > 0 ? 'bg-primary' : 'bg-bg-light'"
              :style="{ height: `${overTimeMax > 0 ? Math.max((day.count / overTimeMax) * 100, day.count > 0 ? 8 : 4) : 4}%` }"
              :title="`${day.date}: ${day.count}`"
            />
          </div>
          <div class="flex justify-between text-[10px] text-muted mt-1">
            <span>{{ analytics.applicationsOverTime[0]?.date }}</span>
            <span>{{ analytics.applicationsOverTime[analytics.applicationsOverTime.length - 1]?.date }}</span>
          </div>
        </div>
      </div>

      <!-- 5. Demographics -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- By gender -->
        <div class="bg-white rounded-xl border p-5 sm:p-6">
          <h2 class="text-base font-semibold text-dark mb-4">حسب الجنس</h2>
          <div v-if="!Object.keys(analytics.applicantDemographics.byGender).length" class="text-center text-sm text-muted py-6">
            لا توجد بيانات
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="(count, gender) in analytics.applicantDemographics.byGender"
              :key="String(gender)"
              class="flex items-center gap-3"
            >
              <span class="text-xs text-muted w-20 shrink-0 text-right">{{ genderLabel(String(gender)) }}</span>
              <div class="flex-1 h-5 bg-bg-light rounded-full overflow-hidden">
                <div
                  class="h-full rounded-full flex items-center justify-end pe-2 transition-all"
                  :style="{ width: genderPct(count as number), backgroundColor: gender === 'male' ? '#3B82F6' : gender === 'female' ? '#EC4899' : '#9CA3AF' }"
                >
                  <span v-if="(count as number) > 0" class="text-[11px] font-bold text-white">{{ count }}</span>
                </div>
              </div>
              <span class="text-xs font-semibold text-dark w-12 text-left">{{ genderPct(count as number) }}</span>
            </div>
          </div>
        </div>

        <!-- By nationality -->
        <div class="bg-white rounded-xl border p-5 sm:p-6">
          <h2 class="text-base font-semibold text-dark mb-4">حسب الجنسية</h2>
          <div v-if="!Object.keys(analytics.applicantDemographics.byNationality).length" class="text-center text-sm text-muted py-6">
            لا توجد بيانات
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="(count, nat) in analytics.applicantDemographics.byNationality"
              :key="String(nat)"
              class="flex items-center gap-3"
            >
              <span class="text-xs text-muted w-20 shrink-0 text-right truncate" :title="String(nat)">{{ nat }}</span>
              <div class="flex-1 h-5 bg-bg-light rounded-full overflow-hidden">
                <div
                  class="h-full bg-primary rounded-full flex items-center justify-end pe-2 transition-all"
                  :style="{ width: genderPct(count as number) }"
                >
                  <span v-if="(count as number) > 0" class="text-[11px] font-bold text-white">{{ count }}</span>
                </div>
              </div>
              <span class="text-xs font-semibold text-dark w-12 text-left">{{ genderPct(count as number) }}</span>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
