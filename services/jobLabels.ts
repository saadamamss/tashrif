// Canonical job option lists — English codes in DB, Arabic labels in UI.
// Single frontend source: publish form, filters, and display mappers all use this.
// Mirrors the backend allow-lists on CreateJobDto (Type/Location) — keep the three in sync.

export interface JobOption {
  value: string
  label: string
}

export const cityOptions: JobOption[] = [
  { value: 'makkah', label: 'مكة المكرمة' },
  { value: 'madinah', label: 'المدينة المنورة' },
  { value: 'jeddah', label: 'جدة' },
  { value: 'taif', label: 'الطائف' },
  { value: 'mina', label: 'منى' },
  { value: 'arafat', label: 'عرفات' },
  { value: 'muzdalifah', label: 'مزدلفة' },
  { value: 'rabigh', label: 'رابغ' },
  { value: 'khulais', label: 'خليص' },
  { value: 'bahrah', label: 'بحرة' },
  { value: 'jumum', label: 'الجموم' },
  { value: 'allith', label: 'الليث' },
  { value: 'qunfudhah', label: 'القنفذة' },
  { value: 'yanbu', label: 'ينبع' },
  { value: 'badr', label: 'بدر' },
  { value: 'riyadh', label: 'الرياض' },
]

export const cityLabels: Record<string, string> = Object.fromEntries(
  cityOptions.map((o) => [o.value, o.label]),
)

export function cityLabel(key: string | null | undefined): string {
  if (!key) return ''
  return cityLabels[key] ?? key
}

export const workTypeOptions: JobOption[] = [
  { value: 'full-time', label: 'دوام كامل' },
  { value: 'part-time', label: 'دوام جزئي' },
  { value: 'seasonal', label: 'موسمي' },
]

export const workTypeLabels: Record<string, string> = Object.fromEntries(
  workTypeOptions.map((o) => [o.value, o.label]),
)

export function workTypeLabel(key: string | null | undefined): string {
  if (!key) return ''
  return workTypeLabels[key] ?? key
}
