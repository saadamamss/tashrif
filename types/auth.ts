export interface User {
  id: number
  name: string
  email: string
  type: 'individual' | 'entity' | 'admin'
  phone?: string
  nationalId?: string
  gender?: string
  nationality?: string
  createdAt?: string
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
