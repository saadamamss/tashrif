import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

const TextInputStub = {
  name: 'TextInput',
  props: { name: String, label: String, id: String, placeholder: String, required: Boolean },
  template: '<input />',
}

const stubs = {
  Dialog: { template: '<div><slot /></div>' },
  CustomSelect: { name: 'CustomSelect', template: '<div />' },
  FileInput: { name: 'FileInput', template: '<input />' },
  TextInput: TextInputStub,
  PhoneInput: { name: 'PhoneInput', template: '<input />' },
  OTPDialog: { template: '<div />' },
  UploadIcon: { template: '<span />' },
  StepCheckIcon: { template: '<span />' },
  Form: { template: '<form><slot :errors="{}" /></form>' },
  Field: { template: '<div><slot /></div>' },
  ErrorMessage: { template: '<span />' },
}

describe('register/entity (phase 3.7 — National ID + CR)', () => {
  it('separates the National ID from the commercial registration', async () => {
    vi.stubGlobal('useApi', vi.fn(() => ({ post: vi.fn() })))
    vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
    vi.stubGlobal('useFormPersistence', vi.fn(() => ({ save: vi.fn(), restore: vi.fn(() => null), clear: vi.fn() })))
    vi.stubGlobal('useBeforeUnload', vi.fn(() => ({ enable: vi.fn(), disable: vi.fn() })))

    const Page = (await import('../../pages/register/entity.vue')).default
    const wrapper = mount(Page, { global: { stubs } })

    const props = wrapper.findAllComponents(TextInputStub).map(c => c.props())
    const nationalIdInputs = props.filter(p => p.name === 'nationalId')
    const commercialInputs = props.filter(p => p.name === 'commercialReg')

    expect(nationalIdInputs.length).toBeGreaterThan(0)
    expect(commercialInputs.length).toBeGreaterThan(0)
    // Locked-in mapping: a «رقم السجل التجاري» input must never be bound to nationalId.
    expect(nationalIdInputs.every(p => p.label === 'الرقم الوطني')).toBe(true)
    expect(commercialInputs.every(p => p.label === 'رقم السجل التجاري')).toBe(true)
  })
})

