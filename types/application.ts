export interface Application {
  id: number
  jobId: number
  userId: number
  userName: string
  userGender: string
  userCity: string
  qualification: string
  status: ApplicationStatus
  createdAt: string
}

export type ApplicationStatus = 'new' | 'shortlisted' | 'interview' | 'contract_sent' | 'accepted' | 'refused'

export interface ApplicationFilter {
  status?: ApplicationStatus
  search?: string
  page?: number
  limit?: number
}
