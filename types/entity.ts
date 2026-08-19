export interface Entity {
  id: number
  name: string
  email: string
  phone: string
  type: 'entity'
  companyField?: string
  companySize?: string
  commercialReg?: string
  sector?: string
  country?: string
  city?: string
  zone?: string
  district?: string
  street?: string
  zipcode?: string
  region?: string
  description?: string
  website?: string
  facebookUrl?: string
  twitterUrl?: string
  youtubeUrl?: string
  logoUrl?: string
  profileCompletionPct?: number
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
