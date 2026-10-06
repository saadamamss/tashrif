import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mockNavigateTo } from '../mocks/nuxt'

let mockUserType: string | null = null

vi.stubGlobal('useAuth', vi.fn(() => ({
  isAuthenticated: { value: !!mockUserType },
  userType: { value: mockUserType },
  user: { value: mockUserType ? { type: mockUserType } : null },
})))

beforeEach(() => {
  vi.clearAllMocks()
  mockUserType = null
})

describe('role guard middleware', () => {
  describe('entity middleware', () => {
    it('allows entity users', async () => {
      mockUserType = 'entity'

      const middleware = (await import('../../middleware/entity')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('redirects non-entity users', async () => {
      mockUserType = 'individual'

      const middleware = (await import('../../middleware/entity')).default
      await middleware({})

      expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
    })
  })

  describe('individual middleware', () => {
    it('allows individual users', async () => {
      mockUserType = 'individual'

      const middleware = (await import('../../middleware/individual')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('redirects non-individual users', async () => {
      mockUserType = 'entity'

      const middleware = (await import('../../middleware/individual')).default
      await middleware({})

      expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
    })
  })

  describe('user-type middleware', () => {
    it('allows individual users', async () => {
      mockUserType = 'individual'

      const middleware = (await import('../../middleware/user-type')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('allows entity users', async () => {
      mockUserType = 'entity'

      const middleware = (await import('../../middleware/user-type')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('allows admin users', async () => {
      mockUserType = 'admin'

      const middleware = (await import('../../middleware/user-type')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('redirects unknown user types', async () => {
      mockUserType = 'unknown'

      const middleware = (await import('../../middleware/user-type')).default
      await middleware({})

      expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
    })
  })

  describe('admin middleware', () => {
    it('allows admin users', async () => {
      mockUserType = 'admin'

      const middleware = (await import('../../middleware/admin')).default
      const result = await middleware({})

      expect(mockNavigateTo).not.toHaveBeenCalled()
      expect(result).toBeUndefined()
    })

    it('redirects non-admin users', async () => {
      mockUserType = 'individual'

      const middleware = (await import('../../middleware/admin')).default
      await middleware({})

      expect(mockNavigateTo).toHaveBeenCalledWith('/dashboard')
    })
  })
})
