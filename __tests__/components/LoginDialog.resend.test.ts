import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

/**
 * Plan §4 / spec-02 — LoginDialog resend flow after a 403 verification-gate
 * login failure (D6/D8/B-1):
 *
 *   - a requiresVerification:true login result opens the resend block
 *   - handleResend → auth.resendVerification(nationalId) — the composable
 *     call is the pin; the exact { nationalId } payload is asserted in
 *     useAuthEmailVerification.test.ts
 *   - resend success → generic success toast + block collapses
 *   - resend failure → error toast (Arabic), block stays
 */

const toastShow = vi.fn()
const loginMock = vi.fn()
const resendMock = vi.fn()

const mountDialog = async () => {
  const LoginDialog = (await import('../../components/LoginDialog.vue')).default
  return mount(LoginDialog, {
    global: {
      stubs: {
        Transition: { template: '<div><slot /></div>' },
        // Mirrors vee-validate <Form @submit="handleSubmit"> — emitting the
        // submit event drives the component's handler.
        Form: {
          template: '<form @submit.prevent="$emit(\'submit\')"><slot :errors="{}" /></form>',
        },
        Field: { template: '<div><slot/></div>' },
        ErrorMessage: { template: '<span />' },
        TextInput: { template: '<input />' },
        NuxtLink: true,
      },
    },
  })
}

beforeEach(() => {
  vi.clearAllMocks()
  loginMock.mockReset()
  resendMock.mockReset()
  toastShow.mockReset()

  vi.stubGlobal('useLoginModal', () => ({
    showModal: vi.fn(),
    closeModal: vi.fn(),
    isLoginModalShow: true,
  }))
  vi.stubGlobal('useAuth', () => ({
    login: loginMock,
    resendVerification: resendMock,
    isLoading: { value: false },
    isAuthenticated: { value: false },
    error: { value: null },
  }))
  vi.stubGlobal('useToast', () => ({ show: toastShow }))
})

describe('LoginDialog — email verification resend flow', () => {
  it('does not show the resend block on a normal login failure', async () => {
    loginMock.mockResolvedValueOnce({ success: false, requiresVerification: false })
    const wrapper = await mountDialog()

    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.text()).not.toContain('لم يتم تفعيل بريدك الإلكتروني بعد')
    expect(toastShow).toHaveBeenCalledWith('فشل تسجيل الدخول', 'error')
  })

  it('a 403 requiresVerification login opens the resend block', async () => {
    loginMock.mockResolvedValueOnce({ success: false, requiresVerification: true })
    const wrapper = await mountDialog()

    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.text()).toContain('لم يتم تفعيل بريدك الإلكتروني بعد')
    expect(wrapper.text()).toContain('إعادة إرسال رابط التفعيل')
  })

  it('resend button calls resendVerification and shows the generic success toast', async () => {
    loginMock.mockResolvedValueOnce({ success: false, requiresVerification: true })
    resendMock.mockResolvedValueOnce({ success: true })
    const wrapper = await mountDialog()

    await wrapper.find('form').trigger('submit')
    await flushPromises()
    expect(resendMock).not.toHaveBeenCalled()

    const resendBtn = wrapper
      .findAll('button')
      .find(b => b.text().includes('إعادة إرسال رابط التفعيل'))
    expect(resendBtn).toBeTruthy()
    await resendBtn!.trigger('click')
    await flushPromises()

    expect(resendMock).toHaveBeenCalledTimes(1)
    expect(toastShow).toHaveBeenCalledWith(
      'تم إعادة إرسال رابط التفعيل إلى بريدك الإلكتروني',
      'success',
    )
    // success collapses the block back
    expect(wrapper.text()).not.toContain('لم يتم تفعيل بريدك الإلكتروني بعد')
  })

  it('resend failure shows an error toast and keeps the block open', async () => {
    loginMock.mockResolvedValueOnce({ success: false, requiresVerification: true })
    resendMock.mockResolvedValueOnce({ success: false })
    const wrapper = await mountDialog()

    await wrapper.find('form').trigger('submit')
    await flushPromises()

    const resendBtn = wrapper
      .findAll('button')
      .find(b => b.text().includes('إعادة إرسال رابط التفعيل'))
    await resendBtn!.trigger('click')
    await flushPromises()

    expect(toastShow).toHaveBeenCalledWith('فشل إعادة الإرسال', 'error')
    expect(wrapper.text()).toContain('إعادة إرسال رابط التفعيل')
  })
})
