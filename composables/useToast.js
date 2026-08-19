let toastId = 0;

export function useToast() {
  const toasts = useState("toasts", () => []);

  function show(message, type = "info", duration = 3000) {
    const id = ++toastId;
    toasts.value.push({ id, message, type });
    setTimeout(() => {
      toasts.value = toasts.value.filter((t) => t.id !== id);
    }, duration);
  }

  return { toasts, show };
}
