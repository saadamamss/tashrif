export interface Interview {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  userName: string
  userAvatar: string
  entityName: string
  entityLogo: string
  method: InterviewMethod
  date: string
  time: string
  location?: string
  link?: string
  notes: string
  status: 'scheduled' | 'completed' | 'cancelled'
  attendance: 'pending' | 'present' | 'absent'
  createdAt: string
}

export type InterviewMethod = 'in-person' | 'phone' | 'video'

export interface InterviewFormData {
  applicationId: number
  method: InterviewMethod
  date: string
  time: string
  location?: string
  link?: string
  notes?: string
}
