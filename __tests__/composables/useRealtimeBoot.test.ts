import { describe, it, expect, beforeEach, vi } from 'vitest'

const fetchNotifications = vi.fn()
const clearAll = vi.fn()
const start = vi.fn()
const stop = vi.fn()
let authed = true
let connected = false

function setupGlobals() {
  vi.stubGlobal('useAuth', vi.fn(() => ({
    isAuthenticated: { value: authed },
  })))
  vi.stubGlobal('useNotifications', vi.fn(() => ({
    fetchNotifications,
    clearAll,
  })))
  vi.stubGlobal('useSignalr', vi.fn(() => ({
    start,
    stop,
    isConnected: { value: connected },
  })))
}

beforeEach(() => {
  vi.clearAllMocks()
  authed = true
  connected = false
  setupGlobals()
})

describe('useRealtimeBoot', () => {
  it('fetches notifications and starts SignalR when authenticated and disconnected', async () => {
    const { useRealtimeBoot } = await import('../../composables/useRealtimeBoot')
    useRealtimeBoot().boot()

    expect(fetchNotifications).toHaveBeenCalledOnce()
    expect(start).toHaveBeenCalledOnce()
  })

  it('skips SignalR start when already connected but still refreshes notifications', async () => {
    connected = true
    setupGlobals()
    const { useRealtimeBoot } = await import('../../composables/useRealtimeBoot')
    useRealtimeBoot().boot()

    expect(fetchNotifications).toHaveBeenCalledOnce()
    expect(start).not.toHaveBeenCalled()
  })

  it('does nothing when logged out', async () => {
    authed = false
    setupGlobals()
    const { useRealtimeBoot } = await import('../../composables/useRealtimeBoot')
    useRealtimeBoot().boot()

    expect(fetchNotifications).not.toHaveBeenCalled()
    expect(start).not.toHaveBeenCalled()
  })

  it('teardown stops SignalR and clears notifications', async () => {
    const { useRealtimeBoot } = await import('../../composables/useRealtimeBoot')
    useRealtimeBoot().teardown()

    expect(stop).toHaveBeenCalledOnce()
    expect(clearAll).toHaveBeenCalledOnce()
  })
})
