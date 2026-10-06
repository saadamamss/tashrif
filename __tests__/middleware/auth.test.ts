import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mockNavigateTo } from '../mocks/nuxt'

let mockIsAuthenticated = false

vi.stubGlobal('useAuth', vi.fn(() => ({
  isAuthenticated: { value: mockIsAuthenticated },
  userType: { value: null },
  user: { value: null },
})))

beforeEach(() => {
  vi.clearAllMocks()
  mockIsAuthenticated = false
})

describe('auth middleware', () => {
  it('redirects to / when not authenticated and requiresAuth', async () => {
    mockIsAuthenticated = false

    const middleware = (await import('../../middleware/auth')).default
    const to = {
      path: '/dashboard',
      fullPath: '/dashboard',
      query: {},
      meta: { requiresAuth: true },
      matched: [{ meta: { requiresAuth: true } }],
    }
    await middleware(to)

    expect(mockNavigateTo).toHaveBeenCalledWith({ path: '/', query: { redirect: '/dashboard' } })
  })

  it('allows access when authenticated and requiresAuth', async () => {
    mockIsAuthenticated = true

    const middleware = (await import('../../middleware/auth')).default
    const to = {
      path: '/dashboard',
      fullPath: '/dashboard',
      query: {},
      meta: { requiresAuth: true },
      matched: [{ meta: { requiresAuth: true } }],
    }
    const result = await middleware(to)

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })

  it('redirects to /dashboard when authenticated and guest', async () => {
    mockIsAuthenticated = true

    const middleware = (await import('../../middleware/auth')).default
    const to = {
      path: '/login',
      fullPath: '/login',
      query: {},
      meta: { guest: true },
      matched: [{ meta: { guest: true } }],
    }
    await middleware(to)

    expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
  })

  it('allows unauthenticated access to guest routes', async () => {
    mockIsAuthenticated = false

    const middleware = (await import('../../middleware/auth')).default
    const to = {
      path: '/login',
      fullPath: '/login',
      query: {},
      meta: { guest: true },
      matched: [{ meta: { guest: true } }],
    }
    const result = await middleware(to)

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })

  it('allows public routes without meta', async () => {
    mockIsAuthenticated = false

    const middleware = (await import('../../middleware/auth')).default
    const to = {
      path: '/',
      fullPath: '/',
      query: {},
      meta: {},
      matched: [{ meta: {} }],
    }
    const result = await middleware(to)

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })
})
