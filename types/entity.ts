export interface Entity {
  id: number
  name: string
  email: string
  phone: string
  type: 'entity'
  commercialRegister?: string
  sector?: string
  country?: string
  region?: string
  description?: string
  website?: string
  twitter?: string
  facebook?: string
  youtube?: string
  contactPerson?: EntityContactPerson
  stats?: EntityStats
}

export interface EntityContactPerson {
  name: string
  phone: string
  email: string
  role: string
  nationality: string
}

export interface EntityFormData {
  name: string
  sector: string
  country: string
  region: string
  description: string
  website?: string
  twitter?: string
  facebook?: string
  youtube?: string
  contactPerson: EntityContactPerson
}

export interface EntityRegistrationData extends EntityFormData {
  password: string
  nationalId: string
}

export interface EntityStats {
  totalJobs: number
  activeJobs: number
  totalApplicants: number
}
