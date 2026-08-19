import type { User } from "~/types/auth";

export function useAuth() {
  const user = useState<User | null>("auth:user", () => null);
  const isLoading = useState<boolean>("auth:isLoading", () => false);
  const isInitialized = useState<boolean>("auth:isInitialized", () => false);
  const error = useState<string | null>("auth:error", () => null);

  const isAuthenticated = computed(() => !!user.value);
  const userType = computed(() => user.value?.type || null);
  const isIndividual = computed(() => user.value?.type === "individual");
  const isEntity = computed(() => user.value?.type === "entity");
  const isAdmin = computed(() => user.value?.type === "admin");

  function setUser(u: User | null) {
    user.value = u;
  }

  function clearUser() {
    user.value = null;
    error.value = null;
  }

  function api() {
    return useNuxtApp().$api;
  }

  async function init() {
    try {
      const { data } = await api().get<User>("/auth/me");
      setUser(data);
    } catch (err: any) {
      clearUser();
    } finally {
      isInitialized.value = true;
    }
  }

  async function login(credentials: { nationalId: string; password: string }) {
    isLoading.value = true;
    error.value = null;
    try {
      const { data } = await api().post<User>("/auth/login", credentials);
      setUser(data);
      isInitialized.value = true;
      return true;
    } catch (err: any) {
      error.value =
        err?.response?.data?.message || err?.message || "Login failed";
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  async function register(userData: any) {
    isLoading.value = true;
    error.value = null;
    try {
      const { data } = await api().post<User>("/auth/register", userData);
      setUser(data);
      isInitialized.value = true;
      return true;
    } catch (err: any) {
      error.value =
        err?.response?.data?.message || err?.message || "Registration failed";
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  async function logout() {
    try {
      await api().post("/auth/logout");
    } catch {
      /* even if server logout fails, clear local state */
    }
    clearUser();
    isInitialized.value = true;
    navigateTo("/");
  }

  return {
    user: readonly(user),
    isLoading: readonly(isLoading),
    isInitialized: readonly(isInitialized),
    error,
    isAuthenticated,
    userType,
    isIndividual,
    isEntity,
    isAdmin,
    setUser,
    clearUser,
    init,
    login,
    register,
    logout,
  };
}
