import { describe, it, expect, beforeEach, vi } from 'vitest'
import { ref } from 'vue'

/**
 * Plan §4 / spec-02 — pins the REAL useAuth composable (not a stubbed copy)
 * for the email-verification contract:
 *
 *   - login: 403 → { success:false, requiresVerification:true } and NO user set (D6/D2)
 *   - login: 401/other → requiresVerification:false (credentials error, not the gate)
 *   - register: multipart 201 success → sessionless { success:true, requiresVerification:true },
 *     user NEVER set (D3 — no auto-login, no cookies)
 *   - verifyEmail: GET /auth/verify-email?token=… → setUser (D7 auto-login)
 *   - resendVerification: POSTs EXACTLY { nationalId } — never email (D8/B-1)
 */

const mockUser = {
  id: 1,
  nationalId: '1010101010',
  name: 'Test User',
  email: 'test@example.com',
  phone: '0500000000',
  type: 'individual' as const,
  isEmailVerified: true,
}

// Per-key singleton map mirrors Nuxt's useState semantics closely enough:
// the same key returns the same ref for every useAuth() in the same test.
const state = new Map<string, any>()

const apiMock = {
  get: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
}

beforeEach(() => {
  state.clear()
  apiMock.get.mockReset()
  apiMock.post.mockReset()
  apiMock.put.mockReset()

  vi.stubGlobal('useState', (key: string, init?: () => any) => {
    if (!state.has(key)) state.set(key, ref(init ? init() : undefined))
    return state.get(key)!
  })
  vi.stubGlobal('useNuxtApp', () => ({ $api: apiMock }))
  vi.stubGlobal('navigateTo', vi.fn())
  vi.stubGlobal('useRuntimeConfig', () => ({ public: { apiBaseUrl: '/api' } }))
})

const loadUseAuth = async () => {
  const { useAuth } = await import('../../composables/useAuth')
  return useAuth()
}

const httpError = (status: number, message: string) => ({
  response: { status, data: { message } },
})

describe('useAuth — email verification contract (real composable)', () => {
  describe('login gate (D6)', () => {
    it('403 → requiresVerification:true, error set, NO user set', async () => {
      apiMock.post.mockRejectedValueOnce(
        httpError(403, 'يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول'),
      )
      const auth = await loadUseAuth()

      const result = await auth.login({ nationalId: '1010101010', password: 'pw' })

      expect(result.success).toBe(false)
      expect(result.requiresVerification).toBe(true)
      expect(auth.user.value).toBeNull()
      expect(auth.error.value).toBe('يجب تفعيل بريدك الإلكتروني قبل تسجيل الدخول')
    })

    it('400 (wrong credentials) → requiresVerification:false', async () => {
      apiMock.post.mockRejectedValueOnce(httpError(400, 'بيانات الدخول غير صحيحة'))
      const auth = await loadUseAuth()

      const result = await auth.login({ nationalId: '1010101010', password: 'bad' })

      expect(result.success).toBe(false)
      expect(result.requiresVerification).toBe(false)
      expect(auth.error.value).toBe('بيانات الدخول غير صحيحة')
    })

    it('verified user → success + user set', async () => {
      apiMock.post.mockResolvedValueOnce({ data: mockUser })
      const auth = await loadUseAuth()

      const result = await auth.login({ nationalId: '1010101010', password: 'pw' })

      expect(result.success).toBe(true)
      expect(result.requiresVerification).toBe(false)
      expect(auth.user.value).toEqual(mockUser)
    })

    it('network error (no response) → fallback Arabic error, requiresVerification:false', async () => {
      apiMock.post.mockRejectedValueOnce(new Error('Network Error'))
      const auth = await loadUseAuth()

      const result = await auth.login({ nationalId: '1', password: 'pw' })

      expect(result.requiresVerification).toBe(false)
      expect(auth.error.value).toBe('Network Error')
    })
  })

  describe('register — sessionless (D3)', () => {
    it('201 → requiresVerification:true and user NEVER set', async () => {
      apiMock.post.mockResolvedValueOnce({ data: { message: 'تم إنشاء الحساب بنجاح' } })
      const auth = await loadUseAuth()

      const result = await auth.register({ nationalId: '1010101010' })

      expect(result.success).toBe(true)
      expect(result.requiresVerification).toBe(true)
      expect(auth.user.value).toBeNull()
      expect(apiMock.post).toHaveBeenCalledWith('/auth/register', expect.anything())
    })

    it('failure (e.g. 409 duplicate) → success:false + Arabic message', async () => {
      apiMock.post.mockRejectedValueOnce(httpError(409, 'رقم الهوية مسجل مسبقاً'))
      const auth = await loadUseAuth()

      const result = await auth.register({ nationalId: '1010101010' })

      expect(result.success).toBe(false)
      expect(result.requiresVerification).toBe(false)
      expect(auth.error.value).toBe('رقم الهوية مسجل مسبقاً')
    })
  })

  describe('verifyEmail (D7)', () => {
    it('success → GET /auth/verify-email?token=… and user set', async () => {
      apiMock.get.mockResolvedValueOnce({ data: mockUser })
      const auth = await loadUseAuth()

      const result = await auth.verifyEmail('raw-token-123')

      expect(apiMock.get).toHaveBeenCalledWith('/auth/verify-email', {
        params: { token: 'raw-token-123' },
      })
      expect(result.success).toBe(true)
      expect(auth.user.value).toEqual(mockUser)
    })

    it('failure → success:false + error message from response', async () => {
      apiMock.get.mockRejectedValueOnce(httpError(400, 'انتهت صلاحية رابط التحقق'))
      const auth = await loadUseAuth()

      const result = await auth.verifyEmail('expired-token')

      expect(result.success).toBe(false)
      expect(auth.error.value).toBe('انتهت صلاحية رابط التحقق')
      expect(auth.user.value).toBeNull()
    })
  })

  describe('resendVerification (D8/B-1)', () => {
    it('posts EXACTLY { nationalId } — never email — and returns generic success', async () => {
      apiMock.post.mockResolvedValueOnce({
        data: { message: 'إذا كان الحساب موجوداً…' },
      })
      const auth = await loadUseAuth()

      const result = await auth.resendVerification('1010101010')

      // The B-1 fix: payload key must be nationalId. No email anywhere.
      expect(apiMock.post).toHaveBeenCalledWith(
        '/auth/resend-verification',
        { nationalId: '1010101010' },
      )
      expect(result.success).toBe(true)
    })

    it('failure → success:false + error set', async () => {
      apiMock.post.mockRejectedValueOnce(httpError(400, 'يرجى إدخال رقم الهوية مطلوب'))
      const auth = await loadUseAuth()

      const result = await auth.resendVerification('')

      expect(result.success).toBe(false)
      expect(auth.error.value).toBe('يرجى إدخال رقم الهوية مطلوب')
    })
  })

  describe('isEmailVerified getter', () => {
    it('false when unauthenticated, reflects flag when authenticated', async () => {
      const auth = await loadUseAuth()
      expect(auth.isEmailVerified.value).toBe(false)

      apiMock.post.mockResolvedValueOnce({
        data: { ...mockUser, isEmailVerified: false },
      })
      await auth.login({ nationalId: '1010101010', password: 'pw' })
      expect(auth.isEmailVerified.value).toBe(false)

      apiMock.get.mockResolvedValueOnce({ data: mockUser })
      await auth.verifyEmail('token')
      expect(auth.isEmailVerified.value).toBe(true)
    })
  })
})
