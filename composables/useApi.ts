import type { ApiResponse } from '~/types/common'

export function useApi() {
  function getApi() {
    return useNuxtApp().$api as ReturnType<typeof import('axios').create>
  }

  async function request<T>(method: string, endpoint: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const api = getApi()
      let response
      if (method === 'GET') {
        response = await api.get<T>(endpoint, { params: data })
      } else if (method === 'POST') {
        response = await api.post<T>(endpoint, data)
      } else if (method === 'PUT') {
        response = await api.put<T>(endpoint, data)
      } else if (method === 'DELETE') {
        response = await api.delete<T>(endpoint)
      }
      return { data: response!.data, error: null, pending: false }
    } catch (err: any) {
      const message = err?.response?.data?.message || err?.response?.data?.statusMessage || err?.message || 'An error occurred'
      return { data: null, error: message, pending: false }
    }
  }

  return {
    get: <T>(endpoint: string, params?: any): Promise<ApiResponse<T>> =>
      request<T>('GET', endpoint, params),

    post: <T>(endpoint: string, body?: any): Promise<ApiResponse<T>> =>
      request<T>('POST', endpoint, body),

    put: <T>(endpoint: string, body?: any): Promise<ApiResponse<T>> =>
      request<T>('PUT', endpoint, body),

    delete: <T>(endpoint: string): Promise<ApiResponse<T>> =>
      request<T>('DELETE', endpoint),
  }
}
