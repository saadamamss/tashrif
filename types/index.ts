export interface User {
  id: number
  nationalId: string
  name: string
  email: string
  phone: string
  type: 'individual' | 'entity' | 'admin'
  gender?: string
  nationality?: string
  avatarUrl?: string
  createdAt: string
  updatedAt?: string
}

export interface IndividualProfile {
  id: number
  userId: number
  birthDate?: string
  city?: string
  zone?: string
  district?: string
  street?: string
  zipcode?: string
  jobTitle?: string
  profileCompletionPct: number
  createdAt: string
  updatedAt?: string
}

export interface Qualification {
  id: number
  userId: number
  type: string
  specialization?: string
  institution?: string
  graduationYear?: number
  grade?: string
  createdAt: string
}

export interface Experience {
  id: number
  userId: number
  jobTitle: string
  employer: string
  duration?: string
  location?: string
  isCurrent: boolean
  startDate?: string
  endDate?: string
  createdAt: string
}

export interface Cv {
  id: number
  userId: number
  fileName: string
  filePath: string
  fileSize: number
  isDefault: boolean
  uploadedAt: string
}

export interface BankAccount {
  id: number
  userId: number
  iban: string
  bankName: string
  ibanStatus: 'pending' | 'verified' | 'rejected'
  accountStatus: 'active' | 'inactive'
  createdAt: string
  updatedAt?: string
}

export interface EntityProfile {
  id: number
  userId: number
  companyField?: string
  sector?: string
  companySize?: string
  commercialReg?: string
  country?: string
  city?: string
  zone?: string
  district?: string
  street?: string
  zipcode?: string
  website?: string
  facebookUrl?: string
  twitterUrl?: string
  youtubeUrl?: string
  logoUrl?: string
  profileCompletionPct: number
  createdAt: string
  updatedAt?: string
}

export interface ContactPerson {
  id: number
  entityId: number
  name: string
  role?: string
  nationality?: string
  phone: string
  email: string
  isPrimary: boolean
  createdAt: string
}

export interface Job {
  id: number
  entityId: number
  entityName: string
  entityLogo: string
  title: string
  description: string
  location: 'makkah' | 'madinah' | 'jeddah' | 'taif' | 'mina' | 'arafat' | 'muzdalifah' | 'rabigh' | 'khulais' | 'bahrah' | 'jumum' | 'allith' | 'qunfudhah' | 'yanbu' | 'badr' | 'riyadh'
  workType: 'full-time' | 'part-time' | 'seasonal'
  target?: string
  vacancies: number
  qualification?: string
  salaryMin?: number
  salaryMax?: number
  salaryText?: string
  gender: 'male' | 'female' | 'both'
  hours?: string
  duration?: string
  status: 'draft' | 'active' | 'closed'
  publishDate?: string
  endDate?: string
  benefits: JobBenefit[]
  responsibilities: JobResponsibility[]
  conditions: JobCondition[]
  createdAt: string
  updatedAt?: string
}

export interface JobBenefit {
  id: number
  jobId: number
  benefitText: string
  sortOrder: number
}

export interface JobResponsibility {
  id: number
  jobId: number
  responsibility: string
  sortOrder: number
}

export interface JobCondition {
  id: number
  jobId: number
  conditionText: string
  sortOrder: number
}

export interface Application {
  id: number
  jobId: number
  userId: number
  userName: string
  userGender: string
  userCity: string
  qualification: string
  experience?: string
  coverLetter?: string
  cvId?: number
  status: 'new' | 'shortlisted' | 'interview' | 'contract_sent' | 'accepted' | 'refused'
  createdAt: string
  updatedAt?: string
}

export interface Interview {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  method: 'in-person' | 'phone' | 'video'
  date: string
  time: string
  location?: string
  link?: string
  notes: string
  status: 'scheduled' | 'completed' | 'cancelled'
  attendance: 'pending' | 'present' | 'absent'
  createdAt: string
  updatedAt?: string
}

export interface Contract {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  fileUrl: string
  fileSize?: number
  status: 'sent' | 'signed' | 'rejected'
  signedAt?: string
  createdAt: string
  updatedAt?: string
}

export interface Notification {
  id: number
  userId: number
  title: string
  body?: string
  type: string
  referenceId?: number
  referenceType?: string
  isRead: boolean
  createdAt: string
}

export interface Review {
  id: number
  applicationId: number
  reviewerId: number
  rating: number
  notes?: string
  decision: 'pass' | 'fail' | 'hold'
  createdAt: string
}

export interface IndividualStats {
  totalApplications: number
  pendingApps: number
  interviews: number
  signedContracts: number
}

export interface EntityStats {
  totalJobs: number
  activeJobs: number
  totalApplicants: number
}

export interface LoginCredentials {
  nationalId: string
  password: string
}

export interface RegisterData {
  nationalId: string
  password: string
  name: string
  email: string
  phone: string
  type: 'individual' | 'entity'
  gender?: string
  nationality?: string
}

export interface AuthResponse {
  user: User
  token: string
  refreshToken: string
}

export interface AuthState {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  refreshToken: string | null
  isLoading: boolean
  error: string | null
}

export interface JobFormData {
  title: string
  description: string
  location: string
  workType: string
  target: string
  vacancies: number
  qualification: string
  salaryText: string
  benefits: string[]
  responsibilities: string[]
  conditions: string[]
  gender: string
  hours: string
  duration: string
  endDate: string
}

export interface ApplicationFilter {
  status?: string
  search?: string
  page?: number
  limit?: number
}

export interface InterviewFormData {
  applicationId: number
  method: string
  date: string
  time: string
  location?: string
  link?: string
  notes?: string
}

export interface Pagination {
  page: number
  limit: number
  total: number
  totalPages: number
}

export interface ApiResponse<T> {
  data: T | null
  error: string | null
  pending: boolean
}

export interface FileUpload {
  name: string
  size: number
  type: string
  file?: File
  url?: string
}
