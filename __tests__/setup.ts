import { vi } from 'vitest'
import { config } from '@vue/test-utils'
import { setupNuxtMocks } from './mocks/nuxt'

setupNuxtMocks()

vi.stubGlobal('$fetch', vi.fn())

config.global.stubs = {
  NuxtLink: true,
  NuxtPage: true,
  NuxtLayout: true,
  ClientOnly: true,
}
