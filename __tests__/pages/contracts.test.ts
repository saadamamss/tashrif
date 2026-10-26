import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'

beforeEach(() => {
  vi.clearAllMocks()
})

describe('ContractCardSkeleton', () => {
  it('renders card-shaped anatomy (logo row, detail rows, file box, action button)', async () => {
    const ContractCardSkeleton = (await import('../../components/ContractCardSkeleton.vue')).default
    const wrapper = mount(ContractCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.findAll('.rounded-full[aria-hidden="true"]').length).toBe(1)
    expect(wrapper.text()).toBe('')
  })
})

describe('CompanyContractCardSkeleton', () => {
  it('renders card-shaped anatomy (badge, avatar row, detail grid, file box)', async () => {
    const CompanyContractCardSkeleton = (
      await import('../../components/CompanyContractCardSkeleton.vue')
    ).default
    const wrapper = mount(CompanyContractCardSkeleton as any)

    expect(wrapper.find('.bg-white.rounded-2xl').exists()).toBe(true)
    expect(wrapper.findAll('[aria-hidden="true"]').length).toBeGreaterThanOrEqual(10)
    expect(wrapper.text()).toBe('')
  })
})

function stubApi() {
  let resolveContracts: ((v: any) => void) | null = null
  const apiGet = vi.fn(
    () =>
      new Promise((resolve) => {
        resolveContracts = resolve
      })
  )
  vi.stubGlobal('useToast', vi.fn(() => ({ show: vi.fn() })))
  vi.stubGlobal('useApi', vi.fn(() => ({ get: apiGet })))
  return () => resolveContracts
}

const baseStubs = {
  CustomSelect: { template: '<div />' },
  ExpandArea: { template: '<div><slot /></div>' },
  ChevronUp: { template: '<span />' },
  SortBars: { template: '<span />' },
  UiEmptyState: { template: '<div class="empty-state">لا توجد عقود</div>' },
  SignContract: { template: '<div />' },
  UpdateContract: { template: '<div />' },
}

describe('individual contracts first paint', () => {
  let getResolver: () => ((v: any) => void) | null

  beforeEach(() => {
    getResolver = stubApi()
  })

  it('shows card skeletons while loading, cards after resolve, empty state when none', async () => {
    const IndividualContracts = (
      await import('../../components/dashboard/individual/contracts.vue')
    ).default
    const wrapper = mount(IndividualContracts as any, {
      global: {
        stubs: {
          ...baseStubs,
          ContractCard: { props: ['contract'], template: '<div class="contract-card" />' },
        },
      },
    })

    expect(wrapper.findAllComponents({ name: 'ContractCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)

    getResolver()!({ data: { items: [{ id: 1 }] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'ContractCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.contract-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the empty state when the list resolves empty', async () => {
    const IndividualContracts = (
      await import('../../components/dashboard/individual/contracts.vue')
    ).default
    const wrapper = mount(IndividualContracts as any, {
      global: {
        stubs: {
          ...baseStubs,
          ContractCard: { props: ['contract'], template: '<div class="contract-card" />' },
        },
      },
    })

    getResolver()!({ data: { items: [] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'ContractCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.empty-state').exists()).toBe(true)
  })
})

describe('company contracts first paint', () => {
  let getResolver: () => ((v: any) => void) | null

  beforeEach(() => {
    getResolver = stubApi()
  })

  it('shows card skeletons while loading, cards after resolve, empty state when none', async () => {
    const CompanyContracts = (
      await import('../../components/dashboard/company/contracts.vue')
    ).default
    const wrapper = mount(CompanyContracts as any, {
      global: {
        stubs: {
          ...baseStubs,
          CompanyContractCard: { props: ['contract'], template: '<div class="company-contract-card" />' },
        },
      },
    })

    expect(wrapper.findAllComponents({ name: 'CompanyContractCardSkeleton' }).length).toBeGreaterThan(0)
    expect(wrapper.find('.empty-state').exists()).toBe(false)

    getResolver()!({ data: { items: [{ id: 1 }] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'CompanyContractCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.company-contract-card').exists()).toBe(true)
    expect(wrapper.find('.empty-state').exists()).toBe(false)
  })

  it('shows the empty state when the list resolves empty', async () => {
    const CompanyContracts = (
      await import('../../components/dashboard/company/contracts.vue')
    ).default
    const wrapper = mount(CompanyContracts as any, {
      global: {
        stubs: {
          ...baseStubs,
          CompanyContractCard: { props: ['contract'], template: '<div class="company-contract-card" />' },
        },
      },
    })

    getResolver()!({ data: { items: [] }, error: null })
    await flushPromises()

    expect(wrapper.findAllComponents({ name: 'CompanyContractCardSkeleton' }).length).toBe(0)
    expect(wrapper.find('.empty-state').exists()).toBe(true)
  })
})
