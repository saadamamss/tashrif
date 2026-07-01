export interface Contract {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  fileUrl: string
  status: ContractStatus
  signedAt?: string
  createdAt: string
}

export type ContractStatus = 'sent' | 'signed' | 'rejected'

export interface ContractFormData {
  applicationId: number
}
