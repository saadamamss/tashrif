import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

let notifState: { list: any[]; unread: number; loading: boolean } = {
  list: [],
  unread: 0,
  loading: false,
}

function setupGlobals() {
  vi.stubGlobal('useAuth', vi.fn(() => ({
    user: { value: { id: 1, avatarUrl: null } },
    logout: vi.fn(),
  })))
  vi.stubGlobal('useNotifications', vi.fn(() => ({
    notifications: notifState.list,
    unreadCount: notifState.unread,
    loading: notifState.loading,
    loadingMore: false,
    hasMore: false,
    loadMore: vi.fn(),
    addNotification: vi.fn(),
    markAsRead: vi.fn(),
  })))
  vi.stubGlobal('useSignalr', vi.fn(() => ({
    on: vi.fn(),
  })))
}

beforeEach(() => {
  vi.clearAllMocks()
  notifState = { list: [], unread: 0, loading: false }
  setupGlobals()
})

describe('dashboard Header bell', () => {
  it('shows a pulse placeholder while notifications load instead of the badge', async () => {
    notifState.loading = true
    setupGlobals()
    const Header = (await import('../../components/dashboard/Header.vue')).default
    const wrapper = mount(Header as any, {
      global: { stubs: { DropDown: { template: '<div><slot name="trigger" /><slot name="list" /></div>' } } },
    })

    expect(wrapper.find('[aria-hidden="true"].animate-pulse').exists()).toBe(true)
    expect(wrapper.text()).not.toContain('+9')
  })

  it('shows the unread badge once loaded and subscribes to live events', async () => {
    notifState = {
      list: [{ id: 1, title: 'عقد جديد', isRead: false, createdAt: new Date().toISOString() }],
      unread: 2,
      loading: false,
    }
    setupGlobals()
    const signalrOn = vi.fn()
    vi.stubGlobal('useSignalr', vi.fn(() => ({ on: signalrOn })))
    const Header = (await import('../../components/dashboard/Header.vue')).default
    const wrapper = mount(Header as any, {
      global: { stubs: { DropDown: { template: '<div><slot name="trigger" /><slot name="list" /></div>' } } },
    })

    expect(wrapper.find('[aria-hidden="true"].animate-pulse').exists()).toBe(false)
    expect(wrapper.text()).toContain('2')
    expect(signalrOn).toHaveBeenCalledWith('NewApplication', expect.any(Function))
    expect(signalrOn).toHaveBeenCalledWith('StatusChanged', expect.any(Function))
  })
})
