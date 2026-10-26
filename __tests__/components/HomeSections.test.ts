import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('HomeSection', () => {
  it('shows skeleton while loading and hides slot content', async () => {
    const HomeSection = (await import('../../components/dashboard/home/HomeSection.vue')).default
    const wrapper = mount(HomeSection as any, {
      props: { title: 'وظائفي المنشورة', subtitle: 'تابع', loading: true },
      slots: { default: '<div class="real-content">محتوى</div>' },
    })

    expect(wrapper.text()).toContain('وظائفي المنشورة')
    expect(wrapper.find('.real-content').exists()).toBe(false)
    expect(wrapper.findAll('.animate-pulse').length).toBeGreaterThan(0)
  })

  it('shows title and slot content once loaded', async () => {
    const HomeSection = (await import('../../components/dashboard/home/HomeSection.vue')).default
    const wrapper = mount(HomeSection as any, {
      props: { title: 'وظائفي المنشورة', subtitle: 'تابع', loading: false },
      slots: { default: '<div class="real-content">محتوى</div>' },
    })

    expect(wrapper.text()).toContain('وظائفي المنشورة')
    expect(wrapper.find('.real-content').exists()).toBe(true)
  })
})

describe('CompletionSection', () => {
  it('shows skeleton while loading and emits action once loaded', async () => {
    const CompletionSection = (
      await import('../../components/dashboard/home/CompletionSection.vue')
    ).default

    const loadingWrapper = mount(CompletionSection as any, {
      props: { percent: 40, loading: true },
      global: { stubs: { PercentCircle: { template: '<div />' } } },
    })
    expect(loadingWrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThan(0)
    expect(loadingWrapper.find('button').exists()).toBe(false)

    const loadedWrapper = mount(CompletionSection as any, {
      props: { percent: 40, loading: false },
      global: { stubs: { PercentCircle: { template: '<div />' } } },
    })
    await loadedWrapper.find('button').trigger('click')
    expect(loadedWrapper.emitted('action')).toBeTruthy()
  })
})
