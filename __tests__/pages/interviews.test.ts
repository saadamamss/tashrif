import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('InterviewCardSkeleton', () => {
  it('renders card-shaped anatomy (badge, logo row, 3 detail rows, action row)', async () => {
    const InterviewCardSkeleton = (await import('../../components/InterviewCardSkeleton.vue')).default
    const wrapper = mount(InterviewCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBe(1)
    expect(wrapper.text()).toBe('')
  })
})

describe('CompanyInterviewCardSkeleton', () => {
  it('renders card-shaped anatomy (badge, avatar row, 3 detail rows, action row)', async () => {
    const CompanyInterviewCardSkeleton = (
      await import('../../components/CompanyInterviewCardSkeleton.vue')
    ).default
    const wrapper = mount(CompanyInterviewCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.text()).toBe('')
  })
})

function stubApi() {
  let resolveInterviews: ((v: any) => void) | null = null
  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolveInterviews = resolve
      })
  )
  vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
  vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  return () => resolveInterviews
}

const baseStubs = {
  CustomSelect: { template: '<div />' },
  ExpandArea: { template: '<div><slot /></div>' },
  ChevronUp: { template: '<span />' },
  SortBars: { template: '<span />' },
  UiEmptyState: { template: '<div class="empty-state">لا توجد مقابلات</div>' },
}

describe('individual interviews first paint', () => {
  let getResolver: () => ((v: any) => void) | null

  beforeEach(() => {
    getResolver = stubApi()
  })

  it('shows card skeletons while loading, cards after resolve, empty state when none', async () => {
    const IndividualInterviews = (
      await import('../../components/dashboard/individual/interviews.vue')
    ).default
    const wrapper = mount(IndividualInterviews as any, {
      global: {
        stubs: {
          ...baseStubs,
          InterviewCard: { props: ['interview'], template: '<div class="interview-card" />' },
        },
      },
    })

    expect(wrapper.findAllComponents({ name: 'InterviewCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)

    getResolver()!({ data: { items: [{ id: 1 }] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'InterviewCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.interview-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the empty state when the list resolves empty', async () => {
    const IndividualInterviews = (
      await import('../../components/dashboard/individual/interviews.vue')
    ).default
    const wrapper = mount(IndividualInterviews as any, {
      global: {
        stubs: {
          ...baseStubs,
          InterviewCard: { props: ['interview'], template: '<div class="interview-card" />' },
        },
      },
    })

    getResolver()!({ data: { items: [] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'InterviewCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.empty-state').exists()).toBe(true)
  })
})

describe('company interviews first paint', () => {
  let getResolver: () => ((v: any) => void) | null

  beforeEach(() => {
    getResolver = stubApi()
  })

  it('shows card skeletons while loading, cards after resolve, empty state when none', async () => {
    const CompanyInterviews = (
      await import('../../components/dashboard/company/interviews.vue')
    ).default
    const wrapper = mount(CompanyInterviews as any, {
      global: {
        stubs: {
          ...baseStubs,
          CompanyInterviewCard: { props: ['interview'], template: '<div class="company-interview-card" />' },
        },
      },
    })

    expect(wrapper.findAllComponents({ name: 'CompanyInterviewCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)

    getResolver()!({ data: { items: [{ id: 1 }] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'CompanyInterviewCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.company-interview-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the empty state when the list resolves empty', async () => {
    const CompanyInterviews = (
      await import('../../components/dashboard/company/interviews.vue')
    ).default
    const wrapper = mount(CompanyInterviews as any, {
      global: {
        stubs: {
          ...baseStubs,
          CompanyInterviewCard: { props: ['interview'], template: '<div class="company-interview-card" />' },
        },
      },
    })

    getResolver()!({ data: { items: [] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'CompanyInterviewCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.empty-state').exists()).toBe(true)
  })
})
