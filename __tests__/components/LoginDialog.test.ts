import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('LoginDialog', () => {
  it('renders login form when modal is open', async () => {
    vi.stubGlobal('useLoginModal', vi.fn(() => ({
      showModal: vi.fn(),
      closeModal: vi.fn(),
      isLoginModalShow: true,
    })))

    vi.stubGlobal('useAuth', vi.fn(() => ({
      login: vi.fn(),
      isLoading: { value: false },
      isAuthenticated: { value: false },
      error: { value: null },
    })))

    const LoginDialog = (await import('../../components/LoginDialog.vue')).default
    const wrapper = mount(LoginDialog, {
      global: {
        stubs: {
          Transition: { template: '<div><slot /></div>' },
          Form: { template: '<div><slot :errors="{}" /></div>' },
          Field: { template: '<div><slot/></div>' },
          ErrorMessage: { template: '<span />' },
          TextInput: { template: '<input />' },
        },
      },
    })

    expect(wrapper.text()).toContain('تسجيل الدخول')
  })

  it('renders national ID input field', async () => {
    vi.stubGlobal('useLoginModal', vi.fn(() => ({
      showModal: vi.fn(),
      closeModal: vi.fn(),
      isLoginModalShow: true,
    })))

    vi.stubGlobal('useAuth', vi.fn(() => ({
      login: vi.fn(),
      isLoading: { value: false },
      isAuthenticated: { value: false },
      error: { value: null },
    })))

    const LoginDialog = (await import('../../components/LoginDialog.vue')).default
    const wrapper = mount(LoginDialog, {
      global: {
        stubs: {
          Transition: { template: '<div><slot /></div>' },
          Form: { template: '<div><slot :errors="{}" /></div>' },
          Field: { template: '<div><slot v-bind="field"/></div>' },
          ErrorMessage: { template: '<span />' },
          TextInput: { template: '<input />' },
        },
      },
    })

    expect(wrapper.text()).toContain('رقم الهوية')
  })
})
