import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('AdminUsersSkeleton', () => {
  it('renders table-shaped anatomy (header + 6 rows with avatar, badges, action)', async () => {
    const AdminUsersSkeleton = (await import('../../components/AdminUsersSkeleton.vue')).default
    const wrapper = mount(AdminUsersSkeleton as any)

    expect(wrapper.find('.admin-users-skeleton').exists()).toBe(true)
    // Header (5) + 6 rows x 8 shapes = 50+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(20)
    // Two pill badges per row (type + status) + action pill = 18 pills
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBeGreaterThanOrEqual(12)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('admin users first paint', () => {
  let resolveUsers: ((v: any) => void) | null = null

  const apiGet = vi.fn(() => {
    return new Promise((resolve) => {
      resolveUsers = resolve as (v: any) => void
    })
  })

  beforeEach(() => {
    resolveUsers = null
    apiGet.mockClear()
    vi.stubGlobal('useRoute', vi.fn(() => ({ query: {} })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet, put: vi.fn() })))
    vi.stubGlobal('usePagination', vi.fn(() => ({
      page: { value: 1 },
      perPage: { value: 10 },
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
    UiEmptyState: { template: '<div class="empty-state">لا يوجد مستخدمون</div>' },
  }

  it('shows table skeleton — never the empty state — while loading', async () => {
    const UsersPage = (await import('../../pages/dashboard/admin/users.vue')).default
    const wrapper = mount(UsersPage as any, { global: { stubs } })

    expect(wrapper.find('.admin-users-skeleton').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the users once resolved', async () => {
    const UsersPage = (await import('../../pages/dashboard/admin/users.vue')).default
    const wrapper = mount(UsersPage as any, { global: { stubs } })

    resolveUsers!({ data: { items: [{ id: 1, name: 'أحمد', email: 'a@x.com', type: 'individual', nationalId: '1010101010', isDeleted: false }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.find('.admin-users-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('أحمد')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('toggles the filter area open/closed', async () => {
    const UsersPage = (await import('../../pages/dashboard/admin/users.vue')).default
    const wrapper = mount(UsersPage as any, { global: { stubs } })

    const toggle = wrapper.find('button[aria-label="تصفية"]')
    expect(toggle.exists()).toBe(true)
    expect(toggle.attributes('aria-expanded')).toBe('false')
    await toggle.trigger('click')
    expect(toggle.attributes('aria-expanded')).toBe('true')
  })
})
