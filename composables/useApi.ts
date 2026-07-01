import { useAuthStore } from '~/stores/authStore'
import type { ApiResponse } from '~/types/common'

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
      const data = await $fetch(`${baseURL}${endpoint}`, {
        ...options,
        headers,
        retry: 0,
      }) as T
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
    get: <T>(endpoint: string, params?: any): Promise<ApiResponse<T>> =>
      request<T>(endpoint, { method: 'GET', params }),

    post: <T>(endpoint: string, body?: any): Promise<ApiResponse<T>> =>
      request<T>(endpoint, { method: 'POST', body }),

    put: <T>(endpoint: string, body?: any): Promise<ApiResponse<T>> =>
      request<T>(endpoint, { method: 'PUT', body }),

    delete: <T>(endpoint: string): Promise<ApiResponse<T>> =>
      request<T>(endpoint, { method: 'DELETE' }),
  }
}
