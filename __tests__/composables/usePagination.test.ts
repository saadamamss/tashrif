import { describe, it, expect, beforeEach, vi } from 'vitest'

const mockRoute = { query: {} }
const mockRouter = { push: vi.fn() }

vi.mock('vue-router', () => ({
  useRoute: vi.fn(() => mockRoute),
  useRouter: vi.fn(() => mockRouter),
}))

beforeEach(() => {
  mockRoute.query = {}
  vi.clearAllMocks()
})

describe('usePagination', () => {
  it('initializes with default values', async () => {
    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination()

    expect(pag.page.value).toBe(1)
    expect(pag.perPage.value).toBe(9)
    expect(pag.total.value).toBe(0)
    expect(pag.totalPages.value).toBe(1)
  })

  it('reads page from route query', async () => {
    mockRoute.query = { page: '3' }

    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination()

    expect(pag.page.value).toBe(3)
  })

  it('computes totalPages correctly', async () => {
    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination()
    pag.total.value = 25

    expect(pag.totalPages.value).toBe(3)
  })

  it('goToPage updates page and pushes route', async () => {
    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination()
    pag.total.value = 25
    pag.goToPage(2)

    expect(pag.page.value).toBe(2)
    expect(mockRouter.push).toHaveBeenCalledWith({ query: { page: 2 } })
  })

  it('goToPage does nothing with invalid page', async () => {
    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination()
    pag.goToPage(0)

    expect(pag.page.value).toBe(1)
    expect(mockRouter.push).not.toHaveBeenCalled()
  })

  it('onPerPageChange resets to page 1', async () => {
    const { usePagination } = await import('../../composables/usePagination')
    const pag = usePagination({ perPage: 9 })
    pag.total.value = 50
    pag.onPerPageChange(12)

    expect(pag.perPage.value).toBe(12)
    expect(pag.page.value).toBe(1)
  })
})
