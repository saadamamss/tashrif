import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

const mountDialog = async (modelValue = true) => {
  const ApplyJobDialog = (await import('../../components/ApplyJobDialog.vue')).default
  return mount(ApplyJobDialog as any, {
    props: {
      jobId: 1,
      modelValue,
    },
    global: {
      stubs: {
        Dialog: { template: '<div><slot /></div>' },
        Form: { template: '<div><slot /></div>' },
        NuxtLink: { template: '<a><slot /></a>' },
      },
    },
  })
}

beforeEach(() => {
  vi.clearAllMocks()
  vi.stubGlobal('useApi', () => ({ get: vi.fn(), post: vi.fn() }))
  vi.stubGlobal('useToast', () => ({ show: vi.fn() }))
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

  it('renders the required CV picker section', async () => {
    const wrapper = await mountDialog()
    expect(wrapper.text()).toContain('السيرة الذاتية')
    expect(wrapper.text()).toContain('اختر السيرة الذاتية')
  })

  it('loads CVs into the picker when opened', async () => {
    const get = vi.fn(async (url: string) => {
      if (url === '/cvs') return { data: { items: [{ id: 5, fileName: 'cv.pdf' }] } }
      return { data: { items: [{ id: 1, type: 'بكالوريوس' }] } }
    })
    vi.stubGlobal('useApi', () => ({ get, post: vi.fn() }))

    const wrapper = await mountDialog(false)
    await wrapper.setProps({ modelValue: true })
    await flushPromises()

    const selects = wrapper.findAllComponents({ name: 'CustomSelect' })
    const cvSelect = selects.find((s) => s.props('placeholder') === 'اختر السيرة الذاتية')
    expect(cvSelect?.props('items')).toEqual([{ value: 5, label: 'cv.pdf' }])
  })

  it('shows empty state + disables submit when the user has no CVs', async () => {
    const get = vi.fn(async () => ({ data: { items: [] } }))
    vi.stubGlobal('useApi', () => ({ get, post: vi.fn() }))

    const wrapper = await mountDialog(false)
    await wrapper.setProps({ modelValue: true })
    await flushPromises()

    expect(wrapper.text()).toContain('لا توجد سيرة ذاتية')
    expect(wrapper.find('button[type="submit"]').attributes('disabled')).toBeDefined()
  })
})
