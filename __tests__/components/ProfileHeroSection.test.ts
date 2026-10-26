import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

const stubs = {
  NuxtLink: { template: '<a><slot /></a>' },
  EditSmall: { template: '<span />' },
}

describe('ProfileHeroSection', () => {
  it('shows skeleton shapes while loading and hides real data', async () => {
    const ProfileHeroSection = (
      await import('../../components/dashboard/home/ProfileHeroSection.vue')
    ).default
    const wrapper = mount(ProfileHeroSection as any, {
      props: {
        avatarSrc: `https://ui-avatars.com/api/?name=م+ح`,
        name: 'أحمد',
        subtitle: 'مشرف حجاج',
        statsItems: [{ value: 5, label: 'وظائفى المنشورة' }],
        columns: 3,
        loading: true,
      },
      global: { stubs },
    })

    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThan(0)
    expect(wrapper.text()).not.toContain('أحمد')
  })

  it('shows avatar, name and stats once loaded', async () => {
    const ProfileHeroSection = (
      await import('../../components/dashboard/home/ProfileHeroSection.vue')
    ).default
    const FakeIcon = { template: '<span class="fake-icon" />' }
    const wrapper = mount(ProfileHeroSection as any, {
      props: {
        avatarSrc: `https://ui-avatars.com/api/?name=م+ح`,
        name: 'أحمد',
        subtitle: 'مشرف حجاج',
        statsItems: [{ value: 5, label: 'وظائفى المنشورة', icon: FakeIcon }],
        columns: 3,
        loading: false,
      },
      global: { stubs },
    })

    expect(wrapper.findAll('[aria-hidden="true"]').length).toBe(0)
    expect(wrapper.text()).toContain('أحمد')
    expect(wrapper.text()).toContain('مشرف حجاج')
    expect(wrapper.text()).toContain('وظائفى المنشورة')
    expect(wrapper.find('img[alt="user avatar"]').attributes('src')).toBe("https://ui-avatars.com/api/?name=م+ح")
  })
})
