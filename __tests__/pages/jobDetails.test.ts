import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobDetailsSkeleton', () => {
  it('renders details-shaped anatomy (main card, tabs card, sidebar rows, button)', async () => {
    const JobDetailsSkeleton = (await import('../../components/JobDetailsSkeleton.vue')).default
    const wrapper = mount(JobDetailsSkeleton as any)

    expect(wrapper.find('.job-details-skeleton').exists()).toBe(true)
    // Same 7-col grid as the real details layout for a 1:1 swap
    expect(wrapper.find('.grid.grid-cols-7').exists()).toBe(true)
    // Title + desc + logo + tabs + sidebar rows + button = 15+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(15)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('dashboard job details first paint', () => {
  let resolveJob: ((v: any) => void) | null = null

  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolveJob = resolve
      })
  )

  beforeEach(() => {
    resolveJob = null
    apiGet.mockClear()
    vi.stubGlobal('useRoute', vi.fn(() => ({ query: { id: 1 }, params: {} })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  })

  const stubs = {
    Breadcrumbs: { template: '<div />' },
    CustomTabs: { template: '<div><slot /></div>' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state">الوظيفة غير موجودة</div>' },
    ApplyJobDialog: { template: '<div />' },
  }

  it('shows the skeleton — never the not-found state — while loading', async () => {
    const DetailsPage = (await import('../../pages/dashboard/jobs-explore/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    expect(wrapper.find('.job-details-skeleton').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the job once resolved', async () => {
    const DetailsPage = (await import('../../pages/dashboard/jobs-explore/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    resolveJob!({ data: { id: 1, title: 'مشرف حجاج' }, error: null })
    await flushPromises()

    expect(wrapper.find('.job-details-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })
})
