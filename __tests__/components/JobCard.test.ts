import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('JobCard', () => {
  it('renders job title and description when provided', async () => {
    const JobCard = (await import('../../components/JobCard.vue')).default
    const wrapper = mount(JobCard as any, {
      props: {
        job: {
          id: 1,
          title: 'مشرف حجاج',
          description: 'الإشراف على مجموعة من الحجاج',
          entityName: 'شركة الحج الرائدة',
          location: 'مكة المكرمة',
          hours: 'دوام كامل',
          duration: '30 يوماً',
        },
      },
      global: {
        mocks: {
          $route: { path: '/', query: {} },
        },
        stubs: {
          NuxtLink: { template: '<a><slot /></a>' },
          Location: { template: '<span />' },
          CalenderIcon: { template: '<span />' },
        },
      },
    })

    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.text()).toContain('الإشراف على مجموعة من الحجاج')
    expect(wrapper.text()).toContain('شركة الحج الرائدة')
    expect(wrapper.text()).toContain('دوام كامل')
  })

  it('emits openApplyForm with job id when apply button is clicked', async () => {
    const JobCard = (await import('../../components/JobCard.vue')).default
    const wrapper = mount(JobCard as any, {
      props: {
        job: {
          id: 42,
          title: 'مشرف حجاج',
        },
      },
      global: {
        mocks: {
          $route: { path: '/', query: {} },
        },
        stubs: {
          NuxtLink: { template: '<a><slot /></a>' },
          Location: { template: '<span />' },
          CalenderIcon: { template: '<span />' },
        },
      },
    })

    const applyBtn = wrapper.findAll('button').find(b => b.text().includes('قدم الآن'))
    await applyBtn!.trigger('click')

    expect(wrapper.emitted('openApplyForm')).toBeTruthy()
    expect(wrapper.emitted('openApplyForm')![0]).toEqual([42])
  })

  it('renders both action buttons', async () => {
    const JobCard = (await import('../../components/JobCard.vue')).default
    const wrapper = mount(JobCard as any, {
      props: {
        job: {
          id: 1,
          title: 'مشرف حجاج',
        },
      },
      global: {
        mocks: {
          $route: { path: '/', query: {} },
        },
        stubs: {
          NuxtLink: { template: '<a><slot /></a>' },
          Location: { template: '<span />' },
          CalenderIcon: { template: '<span />' },
        },
      },
    })

    expect(wrapper.text()).toContain('عرض التفاصيل')
    expect(wrapper.text()).toContain('قدم الآن')
  })
})
