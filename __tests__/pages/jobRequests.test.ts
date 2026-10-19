import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobRequestCardSkeleton', () => {
  it('renders card-shaped anatomy (badge, title, desc, logo row, 2 date columns)', async () => {
    const JobRequestCardSkeleton = (await import('../../components/JobRequestCardSkeleton.vue')).default
    const wrapper = mount(JobRequestCardSkeleton as any)

    // White card shell like JobRequestCard (not the gray generic box)
    expect(wrapper.find('.bg-white.rounded-3xl').exists()).toBe(true)
    // Badge + title + desc(2) + logo(2) + 2x(label icon + label + value) = 11 shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('dashboard job-requests first paint', () => {
  let resolveApps: ((v: any) => void) | null = null

  const apiGet = vi.fn((url: string) => {
    return new Promise((resolve) => {
      resolveApps = resolve
    })
  })

  beforeEach(() => {
    resolveApps = null
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
    UiEmptyState: { template: '<div class="empty-state">لا توجد طلبات</div>' },
    JobRequestCard: { props: ['application'], template: '<div class="request-card" />' },
  }

  it('shows card skeletons — never the empty state — while loading', async () => {
    const JobRequestsPage = (await import('../../pages/dashboard/job-requests/index.vue')).default
    const wrapper = mount(JobRequestsPage as any, { global: { stubs } })

    // Requests in flight: skeleton grid visible, empty state must NOT flash
    expect(wrapper.findAllComponents({ name: 'JobRequestCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
    expect(wrapper.find('.request-card').exists()).toBe(false)
  })

  it('swaps skeletons for real cards once applications resolve', async () => {
    const JobRequestsPage = (await import('../../pages/dashboard/job-requests/index.vue')).default
    const wrapper = mount(JobRequestsPage as any, { global: { stubs } })

    resolveApps!({ data: { items: [{ id: 1 }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'JobRequestCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.request-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })
})
