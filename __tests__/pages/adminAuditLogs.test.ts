import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('AdminAuditLogsSkeleton', () => {
  it('renders table-shaped anatomy (header + 8 rows with badge and change)', async () => {
    const AdminAuditLogsSkeleton = (await import('../../components/AdminAuditLogsSkeleton.vue')).default
    const wrapper = mount(AdminAuditLogsSkeleton as any)

    expect(wrapper.find('.admin-audit-logs-skeleton').exists()).toBe(true)
    // Header (6) + 8 rows x 7 shapes = 60+ skeleton shapes
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(20)
    // Action badge pill per row = 8 pills
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBe(8)
    // No real content leaks
    expect(wrapper.text()).toBe('')
  })
})

describe('admin audit-logs first paint', () => {
  let resolveLogs: ((v: any) => void) | null = null

  const apiGet = vi.fn(() => {
    return new Promise((resolve) => {
      resolveLogs = resolve as (v: any) => void
    })
  })

  beforeEach(() => {
    resolveLogs = null
    apiGet.mockClear()
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
    vi.stubGlobal('usePagination', vi.fn(() => ({
      page: { value: 1 },
      perPage: { value: 12 },
      total: { value: 0 },
      totalPages: { value: 1 },
      goToPage: vi.fn(),
      onPerPageChange: vi.fn(),
    })))
  })

  const stubs = {
    CustomSelect: { template: '<div />' },
    UserSearchSelect: { template: '<div />' },
    Pagination: { template: '<div />' },
    ChevronUp: { template: '<span />' },
    ExpandArea: { props: ['expand'], template: '<div><slot /></div>' },
    UiErrorState: { template: '<div />' },
    UiEmptyState: { template: '<div class="empty-state">لا توجد سجلات</div>' },
  }

  it('shows table skeleton — never the empty state — while loading', async () => {
    const LogsPage = (await import('../../pages/dashboard/admin/audit-logs.vue')).default
    const wrapper = mount(LogsPage as any, { global: { stubs } })

    expect(wrapper.find('.admin-audit-logs-skeleton').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the logs once resolved', async () => {
    const LogsPage = (await import('../../pages/dashboard/admin/audit-logs.vue')).default
    const wrapper = mount(LogsPage as any, { global: { stubs } })

    resolveLogs!({ data: { items: [{ id: 1, userName: 'أحمد', action: 'users.user_deactivated', entityType: 'users', entityId: 9, oldValue: 'active', newValue: 'deactivated', ipAddress: '1.2.3.4', createdAt: '2026-10-08T00:00:00.000Z' }], total: 1 }, error: null })
    await flushPromises()

    expect(wrapper.find('.admin-audit-logs-skeleton').exists()).toBe(false)
    expect(wrapper.text()).toContain('أحمد')
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('toggles the filter area open/closed', async () => {
    const LogsPage = (await import('../../pages/dashboard/admin/audit-logs.vue')).default
    const wrapper = mount(LogsPage as any, { global: { stubs } })

    const toggle = wrapper.find('button[aria-label="تصفية"]')
    expect(toggle.exists()).toBe(true)
    expect(toggle.attributes('aria-expanded')).toBe('false')
    await toggle.trigger('click')
    expect(toggle.attributes('aria-expanded')).toBe('true')
  })
})
