import { describe, it, expect, beforeEach, vi } from 'vitest'

const mockUser = {
  id: 1,
  nationalId: '1010101010',
  name: 'Test User',
  email: 'test@example.com',
  phone: '0500000000',
  type: 'individual' as const,
  gender: 'male',
  nationality: 'سعودي',
  createdAt: '2025-01-01',
}

let mockUserObj: any = null

function setupUseAuth() {
  vi.stubGlobal('useAuth', vi.fn(() => ({
    user: { value: mockUserObj },
    isAuthenticated: { value: !!mockUserObj },
    isLoading: { value: false },
    error: { value: null },
    userType: { value: mockUserObj?.type || null },
    isIndividual: { value: mockUserObj?.type === 'individual' },
    isEntity: { value: mockUserObj?.type === 'entity' },
    isAdmin: { value: mockUserObj?.type === 'admin' },
    setUser: vi.fn((u: any) => { mockUserObj = u }),
    clearUser: vi.fn(() => { mockUserObj = null }),
    init: vi.fn(),
    login: vi.fn(),
    register: vi.fn(),
    logout: vi.fn(),
  })))
}

const mockFetch = vi.fn()

beforeEach(() => {
  vi.clearAllMocks()
  mockUserObj = null
  vi.stubGlobal('$fetch', mockFetch)
  vi.stubGlobal('useRuntimeConfig', vi.fn(() => ({
    public: { apiBaseUrl: '/api' },
  })))
  vi.stubGlobal('navigateTo', vi.fn())
  vi.stubGlobal('import.meta', { server: false })
  setupUseAuth()
})

describe('useAuth composable', () => {
  describe('initial state', () => {
    it('starts with unauthenticated state', () => {
      const auth = (globalThis as any).useAuth()
      expect(auth.isAuthenticated.value).toBe(false)
      expect(auth.user.value).toBeNull()
    })
  })

  describe('getters', () => {
    it('userType returns the type', () => {
      mockUserObj = mockUser
      setupUseAuth()
      const auth = (globalThis as any).useAuth()
      expect(auth.userType.value).toBe('individual')
    })

    it('isIndividual returns true for individual type', () => {
      mockUserObj = mockUser
      setupUseAuth()
      const auth = (globalThis as any).useAuth()
      expect(auth.isIndividual.value).toBe(true)
      expect(auth.isEntity.value).toBe(false)
      expect(auth.isAdmin.value).toBe(false)
    })
  })
})
