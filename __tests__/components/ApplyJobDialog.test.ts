import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('ApplyJobDialog', () => {
  it('renders dialog with title', async () => {
    const ApplyJobDialog = (await import('../../components/ApplyJobDialog.vue')).default
    const wrapper = mount(ApplyJobDialog as any, {
      props: {
        jobId: 1,
        modelValue: true,
      },
      global: {
        stubs: {
          Dialog: { template: '<div><slot /></div>' },
          Form: { template: '<div><slot /></div>' },
        },
      },
    })

    expect(wrapper.text()).toContain('التقديم على وظيفة')
  })

  it('renders submit and cancel buttons', async () => {
    const ApplyJobDialog = (await import('../../components/ApplyJobDialog.vue')).default
    const wrapper = mount(ApplyJobDialog as any, {
      props: {
        jobId: 1,
        modelValue: true,
      },
      global: {
        stubs: {
          Dialog: { template: '<div><slot /></div>' },
          Form: { template: '<div><slot /></div>' },
        },
      },
    })

    expect(wrapper.text()).toContain('إلغاء')
    expect(wrapper.text()).toContain('التقديم')
  })

  it('has jobId prop (optional)', async () => {
    const ApplyJobDialog = (await import('../../components/ApplyJobDialog.vue')).default
    expect(ApplyJobDialog.props?.jobId).toBeDefined()
  })
})
