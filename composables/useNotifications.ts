import type { Notification } from "~/types";

export function useNotifications() {
  const notifications = useState<Notification[]>("notifications", () => []);
  const unreadCount = computed(() => notifications.value.filter((n) => !n.isRead).length);
  const loading = useState("notifications:loading", () => false);
  const loadingMore = useState("notifications:loadingMore", () => false);
  const page = useState("notifications:page", () => 1);
  const hasMore = useState("notifications:hasMore", () => true);
  const LIMIT = 10;

  async function fetchNotifications() {
    const { get } = useApi();
    loading.value = true;
    page.value = 1;
    hasMore.value = true;
    try {
      const result = await get<{ items: Notification[]; total: number }>(
        "/notifications",
        { page: 1, limit: LIMIT }
      );
      if (result.data) {
        notifications.value = result.data.items;
        hasMore.value = result.data.items.length >= LIMIT;
      }
    } catch (e) {
      console.error("[Notifications] Failed to fetch:", e);
    } finally {
      loading.value = false;
    }
  }

  async function loadMore() {
    if (loadingMore.value || !hasMore.value) return;
    const { get } = useApi();
    loadingMore.value = true;
    try {
      page.value += 1;
      const result = await get<{ items: Notification[]; total: number }>(
        "/notifications",
        { page: page.value, limit: LIMIT }
      );
      if (result.data) {
        notifications.value = [...notifications.value, ...result.data.items];
        hasMore.value = result.data.items.length >= LIMIT;
      }
    } catch (e) {
      page.value -= 1;
      console.error("[Notifications] Failed to load more:", e);
    } finally {
      loadingMore.value = false;
    }
  }

  function addNotification(n: Notification) {
    // Avoid duplicates by id
    if (notifications.value.some((existing) => existing.id === n.id)) return;
    notifications.value = [n, ...notifications.value];
  }

  async function markAsRead(id: number) {
    const { post } = useApi();
    try {
      await post(`/notifications/${id}/read`);
      notifications.value = notifications.value.map((n) =>
        n.id === id ? { ...n, isRead: true } : n
      );
    } catch (e) {
      console.error("[Notifications] Failed to mark as read:", e);
    }
  }

  async function markAllAsRead() {
    const { post } = useApi();
    try {
      await post("/notifications/read-all");
      notifications.value = notifications.value.map((n) => ({ ...n, isRead: true }));
    } catch (e) {
      console.error("[Notifications] Failed to mark all as read:", e);
    }
  }

  function clearAll() {
    notifications.value = [];
  }

  return {
    notifications,
    unreadCount,
    loading,
    loadingMore,
    hasMore,
    fetchNotifications,
    loadMore,
    addNotification,
    markAsRead,
    markAllAsRead,
    clearAll,
  };
}
