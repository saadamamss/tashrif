import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobOfferCardSkeleton', () => {
  it('renders card-shaped anatomy (badge, title, desc, 2x2 detail grid)', async () => {
    const JobOfferCardSkeleton = (await import('../../components/JobOfferCardSkeleton.vue')).default
    const wrapper = mount(JobOfferCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-3xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.text()).toBe('')
  })
})

describe('dashboard published-jobs first paint', () => {
  let resolveJobs: ((v: any) => void) | null = null
  let resolveOptions: ((v: any) => void) | null = null

  const apiGet = vi.fn((url: string) => {
    if (url === '/jobs/filter-options') {
      return new Promise((resolve) => {
        resolveOptions = resolve
      })
    }
    return new Promise((resolve) => {
      resolveJobs = resolve
    })
  })

  beforeEach(() => {
    resolveJobs = null
    resolveOptions = null
    apiGet.mockClear()
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
    vi.stubGlobal('usePagination', vi.fn(() => ({
      page: { value: 1 },
      perPage: { value: 9 },
      total: { value: 0 },
      totalPages: { value: 1 },
      goToPage: vi.fn(),
      onPerPageChange: vi.fn(),
    })))
  })

  const stubs = {
    CustomSelect: { template: '<div />' },
    Pagination: { template: '<div />' },
    ChevronUp: { template: '<span />' },
    SortBars: { template: '<span />' },
    ExpandArea: { template: '<div><slot /></div>' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state">لا توجد وظائف منشورة</div>' },
    JobOfferCard: { props: ['job'], template: '<div class="job-offer-card">{{ job.title }}</div>' },
  }

  it('shows card skeletons — never the empty state — while loading', async () => {
    const PublishedJobsPage = (await import('../../pages/dashboard/published-jobs/index.vue')).default
    const wrapper = mount(PublishedJobsPage as any, { global: { stubs } })

    expect(wrapper.findAllComponents({ name: 'JobOfferCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
    expect(wrapper.find('.job-offer-card').exists()).toBe(false)
  })

  it('swaps skeletons for real cards once jobs resolve', async () => {
    const PublishedJobsPage = (await import('../../pages/dashboard/published-jobs/index.vue')).default
    const wrapper = mount(PublishedJobsPage as any, { global: { stubs } })

    resolveOptions!({
      data: { workTypes: ['full-time'], statuses: ['active'] },
      error: null,
    })
    resolveJobs!({ data: { items: [{ id: 1, title: 'مشرف حجاج' }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'JobOfferCardSkeleton' }).length).toBe(0)
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })
})
