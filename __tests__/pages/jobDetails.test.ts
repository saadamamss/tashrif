import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
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
    UiLoadingSkeleton: { template: '<div class="details-skeleton animate-pulse" />' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state">الوظيفة غير موجودة</div>' },
    ApplyJobDialog: { template: '<div />' },
  }

  it('shows the skeleton — never the not-found state — while loading', async () => {
    const DetailsPage = (await import('../../pages/dashboard/jobs-explore/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    expect(wrapper.find('.details-skeleton').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the job once resolved', async () => {
    const DetailsPage = (await import('../../pages/dashboard/jobs-explore/details.vue')).default
    const wrapper = mount(DetailsPage as any, { global: { stubs } })

    resolveJob!({ data: { id: 1, title: 'مشرف حجاج' }, error: null })
    await flushPromises()

    expect(wrapper.find('.details-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })
})
