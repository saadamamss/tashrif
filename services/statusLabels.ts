import type { ApplicationStatus } from '~/types/application'

export const statusLabels: Record<ApplicationStatus, string> = {
  new: 'جديد',
  shortlisted: 'مرشح',
  interview: 'مقابلة',
  contract_sent: 'تم إرسال العقد',
  accepted: 'مقبول',
  refused: 'مرفوض',
  withdrawn: 'مسحوب',
}

export const statusBadgeStyles: Record<ApplicationStatus, string> = {
  new: 'bg-primary/10 text-primary',
  shortlisted: 'bg-badge-blue/10 text-badge-blue',
  interview: 'bg-badge-yellow/10 text-badge-yellow',
  contract_sent: 'bg-badge-green/10 text-badge-green',
  accepted: 'bg-success/10 text-success',
  refused: 'bg-danger/10 text-danger',
  withdrawn: 'bg-muted/10 text-muted',
}
