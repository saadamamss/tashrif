import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

const stubs = {
  Dialog: { template: '<div><slot /></div>' },
  CustomSelect: { template: '<div />' },
  FileInput: { template: '<input />' },
  TextInput: { template: '<input />' },
  PhoneInput: { template: '<input />' },
  OTPDialog: { template: '<div />' },
  Form: { template: '<form><slot :errors="{}" /></form>' },
  Field: { template: '<div><slot /></div>' },
  ErrorMessage: { template: '<span />' },
}

describe('register/individual (phase 3.6 — CV upload removed)', () => {
  it('does not render a CV field, but keeps the required ID field', async () => {
    vi.stubGlobal('useApi', vi.fn(() => ({ post: vi.fn() })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))

    const Page = (await import('../../pages/register/individual.vue')).default
    const wrapper = mount(Page, { global: { stubs } })

    expect(wrapper.text()).not.toContain('السيرة الذاتية')
    expect(wrapper.text()).toContain('صورة الهوية الوطنية')
  })

  it('still renders the core registration fields', async () => {
    vi.stubGlobal('useApi', vi.fn(() => ({ post: vi.fn() })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))

    const Page = (await import('../../pages/register/individual.vue')).default
    const wrapper = mount(Page, { global: { stubs } })

    expect(wrapper.text()).toContain('إنشاء حساب جديد')
    expect(wrapper.text()).toContain('رقم الجوال')
    expect(wrapper.text()).toContain('صورة الهوية الوطنية')
  })
})
