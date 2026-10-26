// Startup/teardown for dashboard realtime services (notifications + SignalR).
//
// Called from layouts/dashboard.vue — which mounts ONLY on dashboard entry
// (direct load or SPA navigation) and NEVER on public pages — so nothing here
// ever fires on the website. Deliberately NOT a global plugin: plugins run once
// per boot and would miss website→dashboard and login-without-reload entries.
//
// Fire-and-forget by design: boot() is never awaited and must never block.
export function useRealtimeBoot() {
  function boot(): void {
    if (import.meta.server) return
    const { isAuthenticated } = useAuth()
    if (!isAuthenticated.value) return

    const { fetchNotifications } = useNotifications()
    fetchNotifications()

    const { start, isConnected } = useSignalr()
    if (!isConnected.value) {
      start()
    }
  }

  function teardown(): void {
    if (import.meta.server) return
    const { stop } = useSignalr()
    const { clearAll } = useNotifications()
    stop()
    clearAll()
  }

  return { boot, teardown }
}
