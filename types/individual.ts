export interface Individual {
  id: number
  name: string
  email: string
  phone: string
  type: 'individual'
  gender?: string
  nationality?: string
  nationalId?: string
  birthDate?: string
  city?: string
  zone?: string
  district?: string
  street?: string
  zipcode?: string
  jobTitle?: string
  avatarUrl?: string
  profileCompletionPct?: number
  idFile?: string
  stats?: IndividualStats
}

export interface IndividualFormData {
  firstName: string
  lastName: string
  nationalId: string
  phone: string
  email: string
  gender: string
  nationality: string
  idFile?: File
}

export interface IndividualStats {
  totalApplications: number
  pendingApps: number
  interviews: number
  contracts: number
}

export interface Qualification {
  id: number
  type: string
  specialization?: string
  institution?: string
  graduationYear?: number
  grade?: string
}
