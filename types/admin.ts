export interface AdminStats {
  totalUsers: number
  totalIndividuals: number
  totalEntities: number
  totalAdmins: number
  totalJobs: number
  activeJobs: number
  totalApplications: number
  totalContracts: number
}

export interface AdminUser {
  id: number
  name: string
  email: string
  nationalId?: string
  phone?: string
  type: 'individual' | 'entity' | 'admin'
  gender?: string
  nationality?: string
  avatarUrl?: string
  isDeleted?: boolean
  createdAt: string
}

export interface AdminAuditLog {
  id: number
  userId: number
  userName?: string
  action: string
  entityType: string
  entityId: number
  oldValue?: string
  newValue?: string
  ipAddress?: string
  createdAt: string
}
