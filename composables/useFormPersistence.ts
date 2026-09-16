import { ref, watch, onMounted, onBeforeUnmount } from 'vue'

export function useFormPersistence(key: string) {
  const storageKey = `tashrif_form_${key}`

  function save(data: Record<string, any>) {
    try {
      sessionStorage.setItem(storageKey, JSON.stringify(data))
    } catch {
      // sessionStorage may be full or unavailable
    }
  }

  function restore(): Record<string, any> | null {
    try {
      const raw = sessionStorage.getItem(storageKey)
      return raw ? JSON.parse(raw) : null
    } catch {
      return null
    }
  }

  function clear() {
    try {
      sessionStorage.removeItem(storageKey)
    } catch {
      // ignore
    }
  }

  return { save, restore, clear }
}

export function useBeforeUnload(message: string) {
  function handler(e: BeforeUnloadEvent) {
    e.preventDefault()
    e.returnValue = message
    return message
  }

  function enable() {
    if (typeof window !== 'undefined') {
      window.addEventListener('beforeunload', handler)
    }
  }

  function disable() {
    if (typeof window !== 'undefined') {
      window.removeEventListener('beforeunload', handler)
    }
  }

  onBeforeUnmount(disable)

  return { enable, disable }
}
