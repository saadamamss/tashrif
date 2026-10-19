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

export interface ApiError {
  statusCode: number
  statusMessage: string
  data?: any
}

export interface FileUpload {
  name: string
  size: number
  type: string
  file?: File
  url?: string
}
