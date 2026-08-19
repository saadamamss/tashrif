export interface Contract {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  fileUrl: string
  fileSize?: number
  fileName?: string
  userName?: string
  userAvatar?: string
  jobTitle?: string
  entityName?: string
  entityLogo?: string
  notes?: string
  endDate?: string
  status: ContractStatus
  signedAt?: string
  createdAt: string
}

export type ContractStatus = 'sent' | 'signed' | 'rejected'

export interface ContractFormData {
  applicationId: number
}
