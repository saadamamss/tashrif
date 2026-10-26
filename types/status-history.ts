export interface StatusHistoryEntry {
  id: number
  applicationId: number
  oldStatus: string | null
  newStatus: string
  changedBy: number
  changedByName?: string
  changedAt: string
}
