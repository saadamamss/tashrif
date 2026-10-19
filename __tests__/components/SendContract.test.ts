import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { validate } from 'vee-validate'

const toDateInput = (d: Date) => d.toISOString().slice(0, 10)
const daysFromNow = (n: number) => toDateInput(new Date(Date.now() + n * 24 * 60 * 60 * 1000))

const mountDialog = async () => {
  const SendContract = (await import('../../components/SendContract.vue')).default
  return mount(SendContract as any, {
    props: {
      applicants: [],
      modelValue: true,
    },
    global: {
      stubs: {
        Dialog: { template: '<div><slot /></div>' },
        ApplicantCard: { template: '<div />' },
        FileInput: { template: '<div />' },
      },
    },
  })
}

beforeEach(() => {
  vi.clearAllMocks()
  vi.stubGlobal('useApi', () => ({ get: vi.fn(), post: vi.fn() }))
  vi.stubGlobal('useToast', () => ({ show: vi.fn() }))
})

describe('SendContract endDate validation', () => {
  it('wires the validateEndDate rule to the expiry field', async () => {
    const wrapper = await mountDialog()
    const inputs = wrapper.findAllComponents({ name: 'TextInput' })
    const endDate = inputs.find((c) => c.props('name') === 'endDate')
    expect(endDate?.props('rules')).toBe('validateEndDate')
  })

  it('rejects a past date', async () => {
    await mountDialog() // registers the rule
    const result = await validate(daysFromNow(-1), 'validateEndDate')
    expect(result.valid).toBe(false)
    expect(result.errors[0]).toContain('24 ساعة')
  })

  it('rejects today (end of today is within 24h)', async () => {
    await mountDialog()
    const result = await validate(daysFromNow(0), 'validateEndDate')
    expect(result.valid).toBe(false)
  })

  it('accepts a date 2 days out', async () => {
    await mountDialog()
    const result = await validate(daysFromNow(2), 'validateEndDate')
    expect(result.valid).toBe(true)
  })

  it('requires a date', async () => {
    await mountDialog()
    const result = await validate('', 'validateEndDate')
    expect(result.valid).toBe(false)
  })
})
