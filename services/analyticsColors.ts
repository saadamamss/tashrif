// Single palette for analytics — no hex scattered in pages
import type { ApplicationStatus } from '~/types/application'

export const statusColors: Record<ApplicationStatus, string> = {
  new: '#ECB42B',
  shortlisted: '#3B82F6',
  interview: '#8B5CF6',
  contract_sent: '#F59E0B',
  accepted: '#10B981',
  refused: '#EF4444',
  withdrawn: '#9CA3AF',
}

export const genderColors: Record<string, string> = {
  male: '#3B82F6',
  female: '#EC4899',
  'غير محدد': '#9CA3AF',
}

export function genderColor(key: string): string {
  return genderColors[key] ?? '#9CA3AF'
}
