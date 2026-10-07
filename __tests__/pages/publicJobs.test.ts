import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

vi.mock('vue-router', () => ({
  useRoute: () => ({ query: {} }),
  useRouter: () => ({ push: vi.fn() }),
}))

const apiGet = vi.fn(async (url: string) => {
  if (url === '/jobs/filter-options') {
    return {
      data: { workTypes: ['full-time'], locations: ['makkah'], genders: ['male'], entities: [] },
      error: null,
    }
  }
  return { data: { items: [{ id: 1, title: 'مشرف حجاج' }], total: 1 }, error: null }
})

beforeEach(() => {
  vi.clearAllMocks()
  vi.stubGlobal('useAuth', vi.fn(() => ({ isAuthenticated: { value: false } })))
  vi.stubGlobal('useLoginModal', vi.fn(() => ({ showModal: vi.fn(), closeModal: vi.fn() })))
  vi.stubGlobal('useScrollSpy', vi.fn())
  vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn(), toasts: [] })))
  vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  vi.stubGlobal('usePagination', vi.fn(() => ({
    page: { value: 1 },
    perPage: { value: 9 },
    total: { value: 0 },
    totalPages: { value: 1 },
    goToPage: vi.fn(),
    onPerPageChange: vi.fn(),
  })))
  // SSR-first path: loaders RETURN envelopes, one apply step writes refs —
  // the payload transfers return values only, never ref side effects.
  vi.stubGlobal('useAsyncData', vi.fn(async (_key: string, fn: any) => {
    const data = { value: await fn() }
    return {
      data,
      refresh: vi.fn(async () => { data.value = await fn() }),
    }
  }))
})

describe('public jobs page (SSR-first)', () => {
  it('renders the seeded list instead of skeletons on first paint', async () => {
    const JobsPage = (await import('../../pages/jobs.vue')).default
    // Nuxt renders pages inside <Suspense> — mirror that boundary here.
    // jobs.vue relies on Nuxt auto-imports for these — stub them here.
    const wrapper = mount(
      { template: '<Suspense><JobsPage /></Suspense>', components: { JobsPage } } as any,
      {
        global: {
          stubs: {
            CustomSelect: { template: '<div />' },
            Pagination: { template: '<div />' },
            SortBars: { template: '<span />' },
            UiLoadingSkeleton: { template: '<div />' },
            UiErrorState: { template: '<div />' },
            UiEmptyState: { template: '<div />' },
            ApplyJobDialog: { template: '<div />' },
            JobCard: { props: ['job'], template: '<div class="job-card">{{ job.title }}</div>' },
          },
        },
      }
    )
    await flushPromises()

    expect(apiGet).toHaveBeenCalledWith('/jobs/filter-options')
    expect(apiGet).toHaveBeenCalledWith('/jobs', expect.anything())
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.text()).toContain('عرض 1 نتيجة وظيفة')
  })
})
