import { statusLabels } from '~/services/statusLabels'

export { statusLabels }

export const genderLabels: Record<string, string> = {
  male: 'ذكر',
  female: 'أنثى',
  'غير محدد': 'غير محدد',
}

export function genderLabel(key: string): string {
  return genderLabels[key] ?? key
}

export function pct(count: number, total: number): string {
  if (!total) return '0%'
  return `${Math.round((count / total) * 100)}%`
}
