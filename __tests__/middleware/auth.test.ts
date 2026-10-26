import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mockNavigateTo } from '../mocks/nuxt'

let mockIsAuthenticated = false
let mockUser: any = null

vi.stubGlobal('useAuth', vi.fn(() => ({
  isAuthenticated: { value: mockIsAuthenticated },
  userType: { value: mockUser?.type || null },
  user: { value: mockUser },
})))

beforeEach(() => {
  vi.clearAllMocks()
  mockIsAuthenticated = false
  mockUser = null
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

// F4 (spec-03) — the email-verification gate: an authenticated-but-unverified
// user must be bounced to /verify-email from requiresAuth pages (strict
// `=== false` check — legacy users without the flag are never bounced).
describe('auth middleware — verified gate (F4)', () => {
  const makeTo = (overrides: any = {}) => ({
    path: '/dashboard',
    fullPath: '/dashboard',
    query: {},
    meta: { requiresAuth: true },
    matched: [{ meta: { requiresAuth: true } }],
    ...overrides,
  })

  it('redirects unverified user from a requiresAuth page to /verify-email', async () => {
    mockIsAuthenticated = true
    mockUser = { id: 1, isEmailVerified: false }

    const middleware = (await import('../../middleware/auth')).default
    await middleware(makeTo())

    expect(mockNavigateTo).toHaveBeenCalledWith('/verify-email')
  })

  it('does not bounce the user on /verify-email itself (no loop)', async () => {
    mockIsAuthenticated = true
    mockUser = { id: 1, isEmailVerified: false }

    const middleware = (await import('../../middleware/auth')).default
    const result = await middleware(makeTo({ path: '/verify-email', fullPath: '/verify-email' }))

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })

  it('lets a verified user through', async () => {
    mockIsAuthenticated = true
    mockUser = { id: 1, isEmailVerified: true }

    const middleware = (await import('../../middleware/auth')).default
    const result = await middleware(makeTo())

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })

  it('never bounces when the flag is missing (legacy users, strict === false)', async () => {
    mockIsAuthenticated = true
    mockUser = { id: 1 } // no isEmailVerified at all

    const middleware = (await import('../../middleware/auth')).default
    const result = await middleware(makeTo())

    expect(mockNavigateTo).not.toHaveBeenCalled()
    expect(result).toBeUndefined()
  })

  it('does not fire on guest routes (existing guest redirect wins)', async () => {
    mockIsAuthenticated = true
    mockUser = { id: 1, isEmailVerified: false }

    const middleware = (await import('../../middleware/auth')).default
    await middleware({
      path: '/login',
      fullPath: '/login',
      query: {},
      meta: { guest: true },
      matched: [{ meta: { guest: true } }],
    })

    expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
  })
})
