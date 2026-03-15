import { useAuthStore } from "~/stores/authStore";
import { useUserStore } from "~/stores/userStore";
export default defineNuxtRouteMiddleware(async (to) => {
  const authStore = useAuthStore();
  const userStore = useUserStore();

  await authStore.initialize();
  // If route doesn't require auth, skip
  // if (to.meta.auth === false) return;

  // If user data not loaded, fetch it
  if (authStore.isAuthenticated && !authStore.currentUser) {
    await authStore.fetchUser();
  }

  // Sync with user store
  if (authStore.currentUser) {
    userStore.setUser(authStore.currentUser);
  }
});
