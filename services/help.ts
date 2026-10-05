export function buildImageUrl(path: string | null | undefined, fallback?: string): string {
  if (!path) return fallback || ''
  if (/^https?:\/\//.test(path)) return path
  try {
    const config = useRuntimeConfig()
    return new URL(config.public.apiBaseUrl).origin + path
  } catch {
    return path
  }
}

export function toDateInputValue(value: string) {
  if (!value) return "";
  const d = new Date(value);
  if (isNaN(d.getTime())) return value;
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

export function formatDate(value: string, withTime = false) {
  if (!value) return "—";
  const d = new Date(value);
  if (isNaN(d.getTime())) return value;
  return withTime
    ? d.toLocaleString("ar-SA")
    : d.toLocaleDateString("ar-SA", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
}

export function isPast(value: string | null | undefined): boolean {
  if (!value) return false;
  const d = new Date(value);
  return !isNaN(d.getTime()) && d.getTime() < Date.now();
}

export function formatTime(time24: string) {
  const [hours, minutes] = time24.split(":");

  // Create a date object with the given hours and minutes
  const date = new Date();
  date.setHours(parseInt(hours), parseInt(minutes), 0, 0);

  const time12 = date.toLocaleTimeString("ar-SA", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: true,
  });

  return time12;
}

export function formatPeriod(start: string, end: string) {
  if (!start || !end) return "—";

  const startDate = new Date(start);
  const endDate = new Date(end);

  const startDateDay = startDate.getDate();
  const endDateDay = endDate.getDate();

  const startDateMonth = startDate.getMonth();
  const endDateMonth = endDate.getMonth();

  const startDateYear = startDate.getFullYear();
  const endDateYear = endDate.getFullYear();

  // same month and year
  if (startDateMonth == endDateMonth && startDateYear == endDateYear) {
    const d = endDate.toLocaleDateString("ar-SA", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });

    return `${startDateDay} – ${d}`;
  }

  const d1 = startDate.toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
  const d2 = endDate.toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  return `${d1} – ${d2}`;
}
