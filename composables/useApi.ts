import { useAuthStore } from '~/stores/authStore'

interface ApiResponse<T> {
  data: T | null
  error: string | null
  pending: boolean
}

export function useApi() {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl || '/api'

  async function request<T>(endpoint: string, options: any = {}): Promise<ApiResponse<T>> {
    const authStore = useAuthStore()
    const headers: Record<string, string> = {
      ...options.headers,
    }

    if (authStore.authToken) {
      headers['Authorization'] = `Bearer ${authStore.authToken}`
    }

    if (!(options.body instanceof FormData)) {
      headers['Content-Type'] = 'application/json'
    }

    try {
      const data = await $fetch<T>(`${baseURL}${endpoint}`, {
        ...options,
        headers,
        retry: 0,
      })
      return { data, error: null, pending: false }
    } catch (err: any) {
      if (err?.statusCode === 401) {
        authStore.logout()
      }
      const message = err?.data?.statusMessage || err?.message || 'An error occurred'
      return { data: null, error: message, pending: false }
    }
  }

  return {
    get: <T>(endpoint: string, params?: any) =>
      request<T>(endpoint, { method: 'GET', params }),

    post: <T>(endpoint: string, body?: any) =>
      request<T>(endpoint, { method: 'POST', body }),

    put: <T>(endpoint: string, body?: any) =>
      request<T>(endpoint, { method: 'PUT', body }),

    delete: <T>(endpoint: string) =>
      request<T>(endpoint, { method: 'DELETE' }),
  }
}
