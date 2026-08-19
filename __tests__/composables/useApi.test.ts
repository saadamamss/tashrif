import { describe, it, expect, beforeEach, vi } from 'vitest'

const mockAxiosInstance = {
  get: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
  interceptors: {
    request: { use: vi.fn() },
    response: { use: vi.fn() },
  },
}

let mockUserObj: any = { id: 1, name: 'Test', type: 'individual' }

const authActions = {
  setUser: vi.fn((u: any) => { mockUserObj = u }),
  clearUser: vi.fn(() => { mockUserObj = null }),
}

vi.stubGlobal('useNuxtApp', vi.fn(() => ({
  $api: mockAxiosInstance,
})))

vi.stubGlobal('useAuth', vi.fn(() => ({
  user: { value: mockUserObj },
  isAuthenticated: { value: !!mockUserObj },
  isLoading: { value: false },
  error: { value: null },
  userType: { value: mockUserObj?.type || null },
  isIndividual: { value: mockUserObj?.type === 'individual' },
  isEntity: { value: mockUserObj?.type === 'entity' },
  isAdmin: { value: mockUserObj?.type === 'admin' },
  ...authActions,
})))

vi.stubGlobal('navigateTo', vi.fn())
vi.stubGlobal('import.meta', { server: false })

beforeEach(() => {
  vi.clearAllMocks()
})

describe('useApi', () => {
  describe('request basics', () => {
    it('calls api.get for GET requests', async () => {
      mockAxiosInstance.get.mockResolvedValue({ data: { id: 1, name: 'test' } })

      const { useApi } = await import('../../composables/useApi')
      const api = useApi()
      await api.get('/test')

      expect(mockAxiosInstance.get).toHaveBeenCalledWith('/test', { params: undefined })
    })

    it('calls api.post for POST requests with body', async () => {
      mockAxiosInstance.post.mockResolvedValue({ data: { ok: true } })

      const { useApi } = await import('../../composables/useApi')
      const api = useApi()
      await api.post('/test', { name: 'test' })

      expect(mockAxiosInstance.post).toHaveBeenCalledWith('/test', { name: 'test' })
    })
  })

  describe('401 handling', () => {
    it('401 handling is in axios interceptor, not useApi', async () => {
      mockAxiosInstance.get.mockRejectedValue({ response: { status: 401 } })

      const { useApi } = await import('../../composables/useApi')
      const api = useApi()
      const response = await api.get('/some/protected')

      expect(response.error).toBeTruthy()
    })
  })

  describe('response format', () => {
    it('returns data on success', async () => {
      mockAxiosInstance.get.mockResolvedValue({ data: { id: 1, name: 'test' } })

      const { useApi } = await import('../../composables/useApi')
      const api = useApi()
      const response = await api.get('/test')

      expect(response.data).toEqual({ id: 1, name: 'test' })
      expect(response.error).toBeNull()
      expect(response.pending).toBe(false)
    })

    it('returns error message on failure', async () => {
      mockAxiosInstance.get.mockRejectedValue({
        response: { data: { message: 'Bad request' } },
      })

      const { useApi } = await import('../../composables/useApi')
      const api = useApi()
      const response = await api.get('/test')

      expect(response.data).toBeNull()
      expect(response.error).toBe('Bad request')
    })
  })
})
