import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

const FakeIcon = { template: '<span class="fake-icon" />' }

describe('ProfileInfoSection', () => {
  it('shows skeleton rows while loading and hides real values', async () => {
    const ProfileInfoSection = (
      await import('../../components/dashboard/profile/ProfileInfoSection.vue')
    ).default
    const wrapper = mount(ProfileInfoSection as any, {
      props: {
        title: 'البيانات الشخصية',
        items: [{ key: 'الجنسية', value: 'سعودي', icon: FakeIcon }],
        loading: true,
      },
    })

    expect(wrapper.text()).toContain('البيانات الشخصية')
    expect(wrapper.text()).not.toContain('سعودي')
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThan(0)
  })

  it('shows title, items and header action once loaded', async () => {
    const ProfileInfoSection = (
      await import('../../components/dashboard/profile/ProfileInfoSection.vue')
    ).default
    const wrapper = mount(ProfileInfoSection as any, {
      props: {
        title: 'بيانات الإتصال',
        items: [{ key: 'البريد الإلكترونى', value: 'a@b.co', icon: FakeIcon }],
        loading: false,
      },
      slots: { 'header-action': '<a class="change-pw">تغيير كلمة المرور</a>' },
    })

    expect(wrapper.text()).toContain('بيانات الإتصال')
    expect(wrapper.text()).toContain('a@b.co')
    expect(wrapper.find('.change-pw').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBe(0)
  })
})

describe('ProfileListSection', () => {
  it('shows skeleton while loading and hides slot content', async () => {
    const ProfileListSection = (
      await import('../../components/dashboard/profile/ProfileListSection.vue')
    ).default
    const wrapper = mount(ProfileListSection as any, {
      props: { title: 'المؤهلات', loading: true },
      slots: { default: '<div class="real-row">مؤهل</div>' },
    })

    expect(wrapper.text()).toContain('المؤهلات')
    expect(wrapper.find('.real-row').exists()).toBe(false)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThan(0)
  })

  it('shows title, action and slot content once loaded', async () => {
    const ProfileListSection = (
      await import('../../components/dashboard/profile/ProfileListSection.vue')
    ).default
    const wrapper = mount(ProfileListSection as any, {
      props: { title: 'الخبرات', loading: false },
      slots: {
        default: '<div class="real-row">خبرة</div>',
        'header-action': '<button class="add-btn">إضافة خبرة</button>',
      },
    })

    expect(wrapper.text()).toContain('الخبرات')
    expect(wrapper.find('.real-row').exists()).toBe(true)
    expect(wrapper.find('.add-btn').exists()).toBe(true)
  })
})
