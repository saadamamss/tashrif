import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

const contract = {
  id: 7,
  fileName: 'عقد-العمل.pdf',
  fileSize: 2048,
  fileUrl: '/uploads/contracts/c7.pdf',
  endDate: '2026-11-01T20:59:59.999Z',
  status: 'sent',
}

const mountDialog = async (props: any = {}, post: any = vi.fn(async () => ({ error: null }))) => {
  vi.stubGlobal('useApi', () => ({ get: vi.fn(), post }))
  const SignContract = (await import('../../components/SignContract.vue')).default
  return mount(SignContract as any, {
    props: { modelValue: true, ...props },
    global: {
      stubs: {
        Dialog: { template: '<div><slot /></div>' },
        Pdf: { template: '<span />' },
        TextInput: {
          props: ['modelValue'],
          template: '<input :value="modelValue" @input="$emit(\'update:modelValue\', $event.target.value)" />',
        },
      },
    },
  })
}

beforeEach(() => {
  vi.clearAllMocks()
  vi.stubGlobal('useApi', () => ({ get: vi.fn(), post: vi.fn() }))
  vi.stubGlobal('useToast', () => ({ show: vi.fn() }))
  vi.stubGlobal('useRuntimeConfig', () => ({ public: { apiBaseUrl: 'http://localhost:5001/api' } }))
})

describe('SignContract contract wiring', () => {
  it('renders the passed contract file name, size and deadline', async () => {
    const wrapper = await mountDialog({ contractId: 7, contract })
    expect(wrapper.text()).toContain('عقد-العمل.pdf')
    expect(wrapper.text()).toContain('2.0 Kb')
    expect(wrapper.text()).toContain('ينتهي التوقيع')
  })

  it('shows the empty preview message when no contract is passed', async () => {
    const wrapper = await mountDialog()
    expect(wrapper.text()).toContain('لا يمكن عرض ملف العقد')
  })

  it('emits signed after a successful submit', async () => {
    const post = vi.fn(async () => ({ error: null }))
    const wrapper = await mountDialog({ contractId: 7, contract }, post)
    await wrapper.find('input[type="checkbox"]').setChecked(true)
    await wrapper.find('input[type="text"], input:not([type])').setValue('أوافق على كل بنود العقد')
    const submit = wrapper.findAll('button').find((b) => b.text().includes('توقيع العقد'))
    await submit!.trigger('click')
    await flushPromises()
    expect(post).toHaveBeenCalledWith('/contracts/7/sign')
    expect(wrapper.emitted('signed')).toHaveLength(1)
  })

  it('does not sign without a contract id', async () => {
    const post = vi.fn(async () => ({ error: null }))
    const wrapper = await mountDialog({ contract }, post)
    await wrapper.find('input[type="checkbox"]').setChecked(true)
    await wrapper.find('input[type="text"], input:not([type])').setValue('أوافق على كل بنود العقد')
    const submit = wrapper.findAll('button').find((b) => b.text().includes('توقيع العقد'))
    await submit!.trigger('click')
    await flushPromises()
    expect(post).not.toHaveBeenCalled()
    expect(wrapper.emitted('signed')).toBeUndefined()
  })
})
