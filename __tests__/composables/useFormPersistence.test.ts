import { describe, it, expect, beforeEach, vi } from 'vitest'

const storage: Record<string, string> = {}

beforeEach(() => {
  Object.keys(storage).forEach(k => delete storage[k])
  vi.stubGlobal('sessionStorage', {
    getItem: vi.fn((key: string) => storage[key] ?? null),
    setItem: vi.fn((key: string, val: string) => { storage[key] = val }),
    removeItem: vi.fn((key: string) => { delete storage[key] }),
  })
})

describe('useFormPersistence', () => {
  it('saves and restores data', async () => {
    const { useFormPersistence } = await import('../../composables/useFormPersistence')
    const { save, restore } = useFormPersistence('test-form')

    save({ name: 'test', value: 42 })
    const restored = restore()

    expect(restored).toEqual({ name: 'test', value: 42 })
  })

  it('clears data', async () => {
    const { useFormPersistence } = await import('../../composables/useFormPersistence')
    const { save, restore, clear } = useFormPersistence('test-form')

    save({ name: 'test' })
    clear()
    const restored = restore()

    expect(restored).toBeNull()
  })

  it('returns null for non-existent key', async () => {
    const { useFormPersistence } = await import('../../composables/useFormPersistence')
    const { restore } = useFormPersistence('non-existent')

    expect(restore()).toBeNull()
  })

  it('handles corrupted data gracefully', async () => {
    storage['tashrif_form_bad'] = 'not-json'
    const { useFormPersistence } = await import('../../composables/useFormPersistence')
    const { restore } = useFormPersistence('bad')

    expect(restore()).toBeNull()
  })
})
