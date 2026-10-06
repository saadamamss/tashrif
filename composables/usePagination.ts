import { useRoute, useRouter } from 'vue-router'
import { ref, computed, watch } from 'vue'

export function usePagination(options: {
  perPage?: number
  routeQuery?: boolean
} = {}) {
  const route = useRoute()
  const router = useRouter()

  const perPage = ref(options.perPage || 9)
  const total = ref(0)
  const page = ref(1)

  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / perPage.value)))

  if (options.routeQuery !== false) {
    const queryPage = Number(route.query.page) || 1
    page.value = Math.max(1, queryPage)
  }

  function goToPage(p: number) {
    if (p < 1 || p > totalPages.value || p === page.value) return
    page.value = p
    if (options.routeQuery !== false) {
      router.push({ query: { ...route.query, page: p || undefined } })
    }
  }

  function nextPage() {
    goToPage(page.value + 1)
  }

  function prevPage() {
    goToPage(page.value - 1)
  }

  function onPerPageChange(val: number) {
    perPage.value = val
    page.value = 1
    if (options.routeQuery !== false) {
      router.push({ query: { ...route.query, page: undefined } })
    }
  }

  return {
    page,
    perPage,
    total,
    totalPages,
    goToPage,
    nextPage,
    prevPage,
    onPerPageChange,
  }
}
