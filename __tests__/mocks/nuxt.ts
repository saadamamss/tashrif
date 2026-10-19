import { vi } from 'vitest'

export const mockUseCookie = vi.fn(() => ({
  value: null,
}))

export const mockNavigateTo = vi.fn()

export const mockUseRouter = vi.fn(() => ({
  push: vi.fn(),
  replace: vi.fn(),
}))

export const mockUseRoute = vi.fn(() => ({
  query: {},
  path: '/',
  fullPath: '/',
}))

export const mockUseRuntimeConfig = vi.fn(() => ({
  public: {
    apiBaseUrl: '/api',
  },
}))

export function setupNuxtMocks() {
  vi.stubGlobal('useCookie', mockUseCookie)
  vi.stubGlobal('navigateTo', mockNavigateTo)
  vi.stubGlobal('useRouter', mockUseRouter)
  vi.stubGlobal('useRoute', mockUseRoute)
  vi.stubGlobal('useRuntimeConfig', mockUseRuntimeConfig)
  vi.stubGlobal('defineNuxtRouteMiddleware', (fn: Function) => (to: any, from?: any) => fn(to))
  vi.stubGlobal('definePageMeta', vi.fn())
  vi.stubGlobal('useHead', vi.fn())
}

export function cleanupNuxtMocks() {
  vi.unstubAllGlobals()
  vi.clearAllMocks()
}
