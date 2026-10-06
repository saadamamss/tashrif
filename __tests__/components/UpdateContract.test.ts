import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

const mountDialog = async (contract: any = {}) => {
  const UpdateContract = (await import('../../components/UpdateContract.vue')).default
  return mount(UpdateContract as any, {
    props: {
      contract: { id: 7, notes: 'ملاحظات قديمة', endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString(), fileName: 'old.pdf', ...contract },
      modelValue: true,
    },
    global: {
      stubs: {
        Dialog: { template: '<div><slot /></div>' },
        FileInput: { template: '<div />' },
      },
    },
  })
}

beforeEach(() => {
  vi.clearAllMocks()
  vi.stubGlobal('useApi', () => ({ put: vi.fn(async () => ({ error: null })) }))
  vi.stubGlobal('useToast', () => ({ show: vi.fn() }))
})

describe('UpdateContract', () => {
  it('prefills notes from the contract', async () => {
    const wrapper = await mountDialog()
    expect(wrapper.html()).toContain('ملاحظات قديمة')
  })

  it('shows the keep-current-file hint', async () => {
    const wrapper = await mountDialog()
    expect(wrapper.html()).toContain('old.pdf')
  })
})
