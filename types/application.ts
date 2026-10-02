import type { Job } from './job'

export interface Application {
  id: number
  jobId: number
  userId: number
  userName: string
  userGender: string
  userCity: string
  qualification: string
  experience?: string
  cvId?: number | null
  cvFileName?: string | null
  cvFilePath?: string | null
  status: ApplicationStatus
  createdAt: string
  job?: Job
}

export type ApplicationStatus = 'new' | 'shortlisted' | 'interview' | 'contract_sent' | 'accepted' | 'refused' | 'withdrawn'

export interface ApplicationFilter {
  status?: ApplicationStatus
  search?: string
  page?: number
  limit?: number
}
