export interface Individual {
  id: number
  name: string
  email: string
  phone: string
  type: 'individual'
  gender?: string
  nationality?: string
  nationalId?: string
  cvFile?: string
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
  cvFile?: File
  idFile?: File
}

export interface IndividualStats {
  totalApplications: number
  pendingApps: number
  interviews: number
  contracts: number
}
