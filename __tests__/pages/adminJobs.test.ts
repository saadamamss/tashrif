import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('AdminJobsSkeleton', () => {
  it('renders table-shaped anatomy (header + 9 rows with title, badges, action)', async () => {
    const AdminJobsSkeleton = (await import('../../components/AdminJobsSkeleton.vue')).default
    const wrapper = mount(AdminJobsSkeleton as any)

    expect(wrapper.find('.admin-jobs-skeleton').exists()).toBe(true)
    // Header (6) + 9 rows x 7 shapes = 60+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(20)
    // Status badge + action pill per row = 18 pills
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBeGreaterThanOrEqual(12)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('admin jobs first paint', () => {
  let resolveJobs: ((v: any) => void) | null = null

  const apiGet = vi.fn(() => {
    return new Promise((resolve) => {
      resolveJobs = resolve as (v: any) => void
    })
  })

  beforeEach(() => {
    resolveJobs = null
    apiGet.mockClear()
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet, put: vi.fn() })))
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
    ConfirmDialog: { template: '<div />' },
    ChevronUp: { template: '<span />' },
    ExpandArea: { props: ['expand'], template: '<div><slot /></div>' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state">لا توجد وظائف</div>' },
  }

  it('shows table skeleton — never the empty state — while loading', async () => {
    const JobsPage = (await import('../../pages/dashboard/admin/jobs.vue')).default
    const wrapper = mount(JobsPage as any, { global: { stubs } })

    expect(wrapper.find('.admin-jobs-skeleton').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the jobs once resolved', async () => {
    const JobsPage = (await import('../../pages/dashboard/admin/jobs.vue')).default
    const wrapper = mount(JobsPage as any, { global: { stubs } })

    resolveJobs!({ data: { items: [{ id: 1, title: 'مشرف حجاج', description: 'وصف', entityName: 'شركة', location: 'makkah', status: 'active', applicantCount: 3 }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.find('.admin-jobs-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('toggles the filter area open/closed', async () => {
    const JobsPage = (await import('../../pages/dashboard/admin/jobs.vue')).default
    const wrapper = mount(JobsPage as any, { global: { stubs } })

    const toggle = wrapper.find('button[aria-label="تصفية"]')
    expect(toggle.exists()).toBe(true)
    expect(toggle.attributes('aria-expanded')).toBe('false')
    await toggle.trigger('click')
    expect(toggle.attributes('aria-expanded')).toBe('true')
  })
})
