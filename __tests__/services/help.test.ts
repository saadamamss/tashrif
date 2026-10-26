import { describe, it, expect } from 'vitest'
import { isPast } from '../../services/help'

describe('isPast', () => {
  it('returns true for a past date', () => {
    expect(isPast('2000-01-01T00:00:00.000Z')).toBe(true)
  })

  it('returns false for a future date', () => {
    expect(isPast('2999-01-01T00:00:00.000Z')).toBe(false)
  })

  it('returns false for null/undefined/empty', () => {
    expect(isPast(null)).toBe(false)
    expect(isPast(undefined)).toBe(false)
    expect(isPast('')).toBe(false)
  })

  it('returns false for an unparsable value', () => {
    expect(isPast('not-a-date')).toBe(false)
  })
})
