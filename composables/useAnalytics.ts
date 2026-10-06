import type { EntityAnalytics } from '~/types/analytics'

export function useAnalytics() {
  const analytics = ref<EntityAnalytics | null>(null)
  const loading = ref(true)
  const error = ref<string | null>(null)

  async function fetchAnalytics() {
    loading.value = true
    error.value = null
    try {
      const res = await useApi().get<EntityAnalytics>('/analytics/entity')
      if (res.error) {
        error.value = res.error
        return
      }
      // Typed get — no `as` cast, compiler checks totalJobs vs total_jobs
      analytics.value = res.data
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : 'حدث خطأ أثناء تحميل البيانات'
      error.value = msg
    } finally {
      loading.value = false
    }
  }

  return { analytics, loading, error, fetchAnalytics, refresh: fetchAnalytics }
}
