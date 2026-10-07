import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('UiSkeleton', () => {
  it('renders a rect skeleton by default with custom dimensions', async () => {
    const UiSkeleton = (await import('../../components/ui/UiSkeleton.vue')).default
    const wrapper = mount(UiSkeleton as any, {
      props: { width: '160px', height: '24px' },
    })

    const el = wrapper.find('[aria-hidden="true"]')
    expect(el.exists()).toBe(true)
    expect(el.classes()).toContain('rounded-xl')
    expect(el.classes()).toContain('animate-pulse')
    expect((el.element as HTMLElement).style.width).toBe('160px')
    expect((el.element as HTMLElement).style.height).toBe('24px')
  })

  it('renders a circle skeleton with rounded-full', async () => {
    const UiSkeleton = (await import('../../components/ui/UiSkeleton.vue')).default
    const wrapper = mount(UiSkeleton as any, {
      props: { variant: 'circle', width: '128px', height: '128px' },
    })

    expect(wrapper.find('[aria-hidden="true"]').classes()).toContain('rounded-full')
  })

  it('renders a text skeleton with rounded', async () => {
    const UiSkeleton = (await import('../../components/ui/UiSkeleton.vue')).default
    const wrapper = mount(UiSkeleton as any, {
      props: { variant: 'text', width: '112px', height: '16px' },
    })

    expect(wrapper.find('[aria-hidden="true"]').classes()).toContain('rounded')
  })

  it('respects an explicit rounded override', async () => {
    const UiSkeleton = (await import('../../components/ui/UiSkeleton.vue')).default
    const wrapper = mount(UiSkeleton as any, {
      props: { variant: 'rect', width: '192px', height: '44px', rounded: 'rounded-full' },
    })

    expect(wrapper.find('[aria-hidden="true"]').classes()).toContain('rounded-full')
  })
})
