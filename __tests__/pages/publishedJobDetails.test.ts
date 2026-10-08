import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('ApplicantCardSkeleton', () => {
  it('renders card-shaped anatomy (avatar row, badge, 5 detail rows)', async () => {
    const ApplicantCardSkeleton = (await import('../../components/ApplicantCardSkeleton.vue')).default
    const wrapper = mount(ApplicantCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.text()).toBe('')
  })
})

describe('published-job details applicants tab first paint', () => {
  let resolvers: Array<(v: any) => void> = []

  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolvers.push(resolve)
      })
  )

  const resolveAll = (apps: any[]) => {
    // loadApplicants fires job + applications requests together
    resolvers.forEach((resolve, i) =>
      resolve(i === 0 ? { data: { id: 1, title: 'مشرف حجاج' }, error: null } : { data: { items: apps }, error: null })
    )
  }

  beforeEach(() => {
    resolvers = []
    apiGet.mockClear()
    vi.stubGlobal('useRoute', vi.fn(() => ({ query: { id: 1 }, params: {} })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  })

  const stubs = {
    Breadcrumbs: { template: '<div />' },
    CustomTabs: { template: '<div><slot name="applicants" /></div>' },
    ArrowTabs: { template: '<div class="arrow-tabs"><slot name="filter" /><slot name="new" /></div>' },
    CustomSelect: { template: '<div />' },
    Search: { template: '<span />' },
    GridIcon: { template: '<span />' },
    ListIcon: { template: '<span />' },
    FilterDrawer: { template: '<div />' },
    SendInterview: { template: '<div />' },
    SendContract: { template: '<div />' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state" />' },
    ApplicantCard: { props: ['applicant'], template: '<div class="applicant-card" />' },
    JobShortlist: { template: '<div />' },
    JobInterviews: { template: '<div />' },
  }

  it('shows pipeline tabs with skeletons in the cards area while loading', async () => {
    const DetailsPage = (await import('../../pages/dashboard/published-jobs/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    // Pipeline UI stays visible; skeletons live in the applicant-grid area only
    expect(wrapper.find('.arrow-tabs').exists()).toBe(true)
    expect(wrapper.findAllComponents({ name: 'ApplicantCardSkeleton' }).length).toBe(6)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
    expect(wrapper.find('.applicant-card').exists()).toBe(false)
  })

  it('swaps skeletons for applicant cards once loaded', async () => {
    const DetailsPage = (await import('../../pages/dashboard/published-jobs/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    resolveAll([{ id: 1, status: 'new' }])
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'ApplicantCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.applicant-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the tab empty state when no applicants resolve', async () => {
    const DetailsPage = (await import('../../pages/dashboard/published-jobs/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    resolveAll([])
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'ApplicantCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.empty-state').exists()).toBe(true)
  })
})
