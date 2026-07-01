export interface Job {
  id: number
  entityId: number
  entityName: string
  entityLogo: string
  title: string
  description: string
  location: string
  type: string
  target: string
  vacancies: number
  qualification: string
  salary: string
  benefits: string[]
  responsibilities: string[]
  conditions: string[]
  gender: string
  hours: string
  duration: string
  status: 'active' | 'closed' | 'draft'
  publishDate: string
  endDate: string
  createdAt: string
}

export interface JobFormData {
  title: string
  description: string
  location: string
  type: string
  target: string
  vacancies: number
  qualification: string
  salary: string
  benefits: string[]
  responsibilities: string[]
  conditions: string[]
  gender: string
  hours: string
  duration: string
  endDate: string
}

export interface JobFilter {
  type?: string
  location?: string
  gender?: string
  entityId?: number
  search?: string
  page?: number
  limit?: number
}

export type JobStatus = 'active' | 'closed' | 'draft'
