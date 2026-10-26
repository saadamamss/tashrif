import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobRequestDetailsSkeleton', () => {
  it('renders details-shaped anatomy (main card, tabs card, 3 sidebar cards)', async () => {
    const JobRequestDetailsSkeleton = (await import('../../components/JobRequestDetailsSkeleton.vue')).default
    const wrapper = mount(JobRequestDetailsSkeleton as any)

    expect(wrapper.find('.job-request-details-skeleton').exists()).toBe(true)
    // Same 7-col grid as the real details layout for a 1:1 swap
    expect(wrapper.find('.grid.grid-cols-7').exists()).toBe(true)
    // Main + tabs + status + history + job-details = 20+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(20)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('dashboard job-request details first paint', () => {
  let resolvers: Array<(v: any) => void> = []

  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolvers.push(resolve as (v: any) => void)
      })
  )

  beforeEach(() => {
    resolvers = []
    apiGet.mockClear()
    vi.stubGlobal('useRoute', vi.fn(() => ({ query: { id: 1 }, params: {} })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet, put: vi.fn() })))
  })

  const stubs = {
    Breadcrumbs: { template: '<div />' },
    CustomTabs: { template: '<div><slot /></div>' },
    StatusTimeline: { template: '<div />' },
    SignContract: { template: '<div />' },
    AddToCalendar: { template: '<div />' },
  }

  function resolveAll(app: any) {
    // loadData fires 4 parallel gets: app, interviews, contracts, history
    resolvers[0]({ data: app, error: null })
    resolvers[1]({ data: { items: [] }, error: null })
    resolvers[2]({ data: { items: [] }, error: null })
    resolvers[3]({ data: [], error: null })
  }

  it('shows the skeleton while loading', async () => {
    const DetailsPage = (await import('../../pages/dashboard/job-requests/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    expect(wrapper.find('.job-request-details-skeleton').exists()).toBe(true)
    expect(wrapper.text()).not.toContain('مشرف حجاج')
  })

  it('shows the request once resolved', async () => {
    const DetailsPage = (await import('../../pages/dashboard/job-requests/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    resolveAll({ id: 1, status: 'new', job: { title: 'مشرف حجاج' } })
    await flushPromises()

    expect(wrapper.find('.job-request-details-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('مشرف حجاج')
  })
})
