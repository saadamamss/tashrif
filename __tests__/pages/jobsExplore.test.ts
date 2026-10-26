import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobCardSkeleton', () => {
  it('renders card-shaped anatomy (title, desc, logo row, 3 detail rows, 2 buttons)', async () => {
    const JobCardSkeleton = (await import('../../components/JobCardSkeleton.vue')).default
    const wrapper = mount(JobCardSkeleton as any)

    // White card shell like JobCard (not the gray generic box)
    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    // Title + desc + logo + 3 detail rows + 2 buttons = 10+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    // Two pill button placeholders
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBe(2)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('dashboard jobs-explore first paint', () => {
  let resolveJobs: ((v: any) => void) | null = null

  const apiGet = vi.fn((url: string) => {
    if (url === '/jobs/filter-options') {
      return Promise.resolve({
        data: { workTypes: ['full-time'], locations: ['makkah'], genders: ['male'], entities: [] },
        error: null,
      })
    }
    return new Promise((resolve) => {
      resolveJobs = resolve
    })
  })

  beforeEach(() => {
    resolveJobs = null
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
    UiEmptyState: { template: '<div class="empty-state">لا توجد وظائف</div>' },
    ApplyJobDialog: { template: '<div />' },
    JobCard: { props: ['job'], template: '<div class="job-card">{{ job.title }}</div>' },
  }

  it('shows card skeletons — never the empty state — while loading', async () => {
    const JobsExplorePage = (await import('../../pages/dashboard/jobs-explore/index.vue')).default
    const wrapper = mount(JobsExplorePage as any, { global: { stubs } })

    // Requests in flight: skeleton grid visible, empty state must NOT flash
    expect(wrapper.findAllComponents({ name: 'JobCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
    expect(wrapper.find('.job-card').exists()).toBe(false)
  })

  it('swaps skeletons for real cards once jobs resolve', async () => {
    const JobsExplorePage = (await import('../../pages/dashboard/jobs-explore/index.vue')).default
    const wrapper = mount(JobsExplorePage as any, { global: { stubs } })

    resolveJobs!({ data: { items: [{ id: 1, title: 'مشرف حجاج' }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'JobCardSkeleton' }).length).toBe(0)
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })
})
