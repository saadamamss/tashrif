import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { mockUseRoute, mockUseRouter } from '../mocks/nuxt'

/**
 * Plan §4 / spec-02 — the F2 public verification page (/verify-email):
 *
 *   - missing ?token → Arabic invalid-link error, no API call
 *   - verifyEmail success → "تم تفعيل بريدك الإلكتروني!" + redirect to
 *     /dashboard after 1.2s (D7 auto-login, user-approved)
 *   - verifyEmail failure → shows the backend's Arabic message + login link
 *   - route contract: the page lives at pages/verify-email.vue (no subfolder)
 *     — the mail link A1/D13 targets /verify-email?token=…
 */

const verifyMock = vi.fn()
const routerPush = vi.fn()

beforeEach(() => {
  vi.clearAllMocks()
  verifyMock.mockReset()
  routerPush.mockReset()

  vi.stubGlobal('useAuth', () => ({
    verifyEmail: verifyMock,
    error: { value: null },
    user: { value: null },
    isAuthenticated: { value: false },
  }))
  mockUseRoute.mockReturnValue({ query: {} })
  mockUseRouter.mockReturnValue({ push: routerPush, replace: vi.fn() })
  vi.useFakeTimers()
})

afterEach(() => {
  vi.useRealTimers()
})

const mountPage = async (queryToken?: string) => {
  mockUseRoute.mockReturnValue({ query: queryToken ? { token: queryToken } : {} })
  const VerifyEmail = (await import('../../pages/verify-email.vue')).default
  const wrapper = mount(VerifyEmail, {
    global: {
      stubs: {
        // auto-imported icon component — stub with a marker so the page's
        // mail-icon isn't the thing under test (see known-issues.md)
        EmailIcon: { template: '<span data-test="email-icon" />' },
        Transition: { template: '<div><slot /></div>' },
      },
    },
  })
  await flushPromises()
  return wrapper
}

describe('pages/verify-email', () => {
  it('missing ?token → invalid-link error, no API call', async () => {
    const wrapper = await mountPage()

    expect(verifyMock).not.toHaveBeenCalled()
    expect(wrapper.text()).toContain('فشل التفعيل')
    expect(wrapper.text()).toContain('رابط التفعيل غير صالح')
    expect(wrapper.text()).toContain('الذهاب إلى تسجيل الدخول')
  })

  it('success → verified state + router push to /dashboard after 1200ms (D7)', async () => {
    verifyMock.mockResolvedValueOnce({ success: true })
    const wrapper = await mountPage('raw-token')

    expect(verifyMock).toHaveBeenCalledWith('raw-token')
    expect(wrapper.text()).toContain('تم تفعيل بريدك الإلكتروني!')
    expect(wrapper.text()).toContain('تحويلك إلى لوحة التحكم')

    routerPush.mockClear()
    vi.advanceTimersByTime(1300)
    expect(routerPush).toHaveBeenCalledWith('/dashboard')
  })

  it('failure → shows the backend Arabic message + login link', async () => {
    verifyMock.mockResolvedValueOnce({ success: false })
    vi.stubGlobal(
      'useAuth',
      vi.fn(() => ({
        verifyEmail: verifyMock,
        error: { value: 'انتهت صلاحية رابط التحقق' },
        user: { value: null },
        isAuthenticated: { value: false },
      })),
    )
    const wrapper = await mountPage('expired-token')

    expect(wrapper.text()).toContain('فشل التفعيل')
    expect(wrapper.text()).toContain('انتهت صلاحية رابط التحقق')
  })

  it('login link navigates to /login via the router', async () => {
    verifyMock.mockRejectedValueOnce(new Error('boom'))
    const wrapper = await mountPage('token')

    // verifyEmail never throws (it catches), but if it ever did the page
    // must still land on the error state, not crash
    expect(wrapper.text()).toContain('فشل التفعيل')

    const link = wrapper.findAll('a').find(a => a.text().includes('الذهاب إلى تسجيل الدخول'))!
    await link.trigger('click')
    expect(routerPush).toHaveBeenCalledWith('/login')
  })
})
