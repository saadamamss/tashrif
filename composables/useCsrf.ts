// composables/useCsrf.ts
// CSRF token in memory only (via useState).
// Set from X-CSRF-Token response header on login/auth/refresh.
// Read by axios interceptor to echo back on state-changing requests.

export function useCsrf() {
  const token = useState<string | null>("csrf_token", () => null);

  function set(val: string | null) {
    token.value = val;
  }

  function get(): string | null {
    return token.value;
  }

  return { get, set };
}
