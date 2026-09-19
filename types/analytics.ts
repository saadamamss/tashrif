export interface EntityAnalytics {
  totalJobs: number
  activeJobs: number
  totalApplications: number
  applicationsByStatus: Record<string, number>
  conversionRate: number
  averageTimeToHireDays: number | null
  topJobs: TopJob[]
  applicationsOverTime: DailyCount[]
  applicantDemographics: Demographics
}

export interface TopJob {
  jobId: number
  title: string
  applicationCount: number
  hiredCount: number
}

export interface DailyCount {
  date: string
  count: number
}

export interface Demographics {
  byGender: Record<string, number>
  byNationality: Record<string, number>
}
