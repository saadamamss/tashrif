import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('AnalyticsSkeleton', () => {
  it('mirrors the analytics layout (stats, funnel, top-jobs, timeline, demographics)', async () => {
    const AnalyticsSkeleton = (await import('../../components/analytics/AnalyticsSkeleton.vue')).default
    const wrapper = mount(AnalyticsSkeleton as any)

    // White section cards like the real page (not gray generic boxes)
    expect(wrapper.findAll('.bg-white.rounded-xl').length).toBeGreaterThanOrEqual(5)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThan(20)
    expect(wrapper.text()).toBe('')
  })
})

describe('analytics page first paint', () => {
  let resolveAnalytics: ((v: any) => void) | null = null

  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolveAnalytics = resolve
      })
  )

  beforeEach(() => {
    resolveAnalytics = null
    apiGet.mockClear()
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  })

  const stubs = {
    DemographicsChart: { template: '<div />' },
    FunnelChart: { template: '<div />' },
    StatCard: { props: ['label', 'value'], template: '<div class="stat-card">{{ label }}</div>' },
    TimelineChart: { template: '<div />' },
    UiErrorState: { template: '<div />' },
  }

  it('shows the layout skeleton while loading, stats once resolved', async () => {
    const AnalyticsPage = (await import('../../pages/dashboard/analytics.vue')).default
    const wrapper = mount(AnalyticsPage as any, { global: { stubs } })

    expect(wrapper.findAllComponents({ name: 'AnalyticsSkeleton' }).length).toBe(1)
    expect(wrapper.find('.stat-card').exists()).toBe(false)

    resolveAnalytics!({
      data: {
        totalJobs: 3,
        activeJobs: 2,
        totalApplications: 5,
        conversionRate: 20,
        averageTimeToHireDays: 4,
        applicationsByStatus: {},
        topJobs: [],
        applicationsOverTime: [],
        applicantDemographics: { byGender: {}, byNationality: {} },
      },
      error: null,
    })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'AnalyticsSkeleton' }).length).toBe(0)
    expect(wrapper.text()).toContain('إجمالي الوظائف')
  })
})
