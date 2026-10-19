import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

const getMock = vi.fn()

vi.stubGlobal('useApi', () => ({
  get: getMock,
}))

const users = [
  {
    id: 5,
    name: 'أحمد محمد',
    email: 'ahmed@example.com',
    type: 'individual',
    avatarUrl: null,
  },
  {
    id: 3,
    name: 'شركة الحج الرائدة',
    email: 'entity@example.com',
    type: 'entity',
    avatarUrl: null,
  },
]

const mountComponent = async () => {
  const UserSearchSelect = (await import('../../components/UserSearchSelect.vue')).default
  return mount(UserSearchSelect as any, {
    global: {
      stubs: {
        Search: { template: '<span />' },
      },
    },
  })
}

beforeEach(() => {
  vi.useFakeTimers()
  getMock.mockResolvedValue({ data: { items: users }, error: null })
})

afterEach(() => {
  vi.useRealTimers()
  vi.clearAllMocks()
})

describe('UserSearchSelect', () => {
  it('renders search results in the dropdown after typing', async () => {
    const wrapper = await mountComponent()

    await wrapper.find('input').setValue('أحمد')
    await vi.advanceTimersByTimeAsync(300) // debounce
    await flushPromises()

    expect(getMock).toHaveBeenCalledWith('/admin/users', expect.objectContaining({ search: 'أحمد', limit: 10 }))

    const dropdown = wrapper.find('.dropdown')
    expect(dropdown.exists()).toBe(true)
    expect(dropdown.text()).toContain('أحمد محمد')
    expect(dropdown.text()).toContain('ahmed@example.com')
    expect(dropdown.text()).toContain('شركة الحج الرائدة')
    // loading state must be gone
    expect(dropdown.text()).not.toContain('جارٍ البحث')
  })

  it('emits the selected user id via v-model when a result is clicked', async () => {
    const wrapper = await mountComponent()

    await wrapper.find('input').setValue('أحمد')
    await vi.advanceTimersByTimeAsync(300)
    await flushPromises()

    await wrapper.findAll('.dropdown button')[0].trigger('click')

    expect(wrapper.emitted('update:modelValue')![0]).toEqual([5])
    expect(wrapper.find('input').element.value).toBe('أحمد محمد')
    expect(wrapper.text()).toContain('تم الاختيار')
  })

  it('shows empty state when search returns no users', async () => {
    getMock.mockResolvedValue({ data: { items: [] }, error: null })
    const wrapper = await mountComponent()

    await wrapper.find('input').setValue('لا يوجد')
    await vi.advanceTimersByTimeAsync(300)
    await flushPromises()

    expect(wrapper.find('.dropdown').text()).toContain('لا توجد نتائج')
  })
})
