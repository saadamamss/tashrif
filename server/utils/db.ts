interface User {
  id: number
  nationalId: string
  password: string
  name: string
  email: string
  phone: string
  type: 'individual' | 'entity'
  gender?: string
  nationality?: string
  createdAt: string
}

interface Job {
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

interface Application {
  id: number
  jobId: number
  userId: number
  userName: string
  userGender: string
  userCity: string
  qualification: string
  status: 'new' | 'shortlisted' | 'interview' | 'contract_sent' | 'accepted' | 'refused'
  createdAt: string
}

interface Interview {
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
}

interface Contract {
  id: number
  applicationId: number
  jobId: number
  userId: number
  entityId: number
  fileUrl: string
  status: 'sent' | 'signed' | 'rejected'
  signedAt?: string
  createdAt: string
}

const tokens: Record<string, { userId: number, type: 'individual' | 'entity' }> = {}

let tokenCounter = 0
function generateToken(userId: number, type: 'individual' | 'entity'): string {
  tokenCounter++
  const token = `tok_${userId}_${tokenCounter}_${Date.now()}`
  tokens[token] = { userId, type }
  return token
}

function hashPassword(pw: string): string {
  return `hashed_${pw}`
}

const users: User[] = [
  { id: 1, nationalId: '1010101010', password: hashPassword('individual'), name: 'عبدالله الحربي', email: 'abdullah@gmail.com', phone: '0500000001', type: 'individual', gender: 'male', nationality: 'سعودي', createdAt: '2025-01-15' },
  { id: 2, nationalId: '2020202020', password: hashPassword('entity'), name: 'مؤسسة الحج الرائدة', email: 'info@hajjlead.com', phone: '0500000002', type: 'entity', createdAt: '2025-01-10' },
  { id: 3, nationalId: '3030303030', password: hashPassword('individual2'), name: 'سارة القحطاني', email: 'sara@gmail.com', phone: '0500000003', type: 'individual', gender: 'female', nationality: 'سعودية', createdAt: '2025-02-01' },
  { id: 4, nationalId: '4040404040', password: hashPassword('individual3'), name: 'محمد الشهري', email: 'mohammed@gmail.com', phone: '0500000004', type: 'individual', gender: 'male', nationality: 'سعودي', createdAt: '2025-02-10' },
]

const jobs: Job[] = [
  { id: 1, entityId: 2, entityName: 'مؤسسة الحج الرائدة', entityLogo: '', title: 'مشرف حجاج', description: 'الإشراف على مجموعة من الحجاج وضمان تنقلهم بين المشاعر المقدسة وفق الخطة المقررة.', location: 'مكة المكرمة', type: 'field', target: 'رجال', vacancies: 15, qualification: 'بكالوريوس', salary: '٨٬٠٠٠ - ١٠٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات', 'تذاكر سفر'], responsibilities: ['متابعة الحجاج يومياً', 'التنسيق مع مشرفي المجموعات', 'الإبلاغ عن أي طوارئ'], conditions: ['خبرة سنتين على الأقل', 'القدرة على العمل لساعات طويلة', 'حسن السيرة والسلوك'], gender: 'male', hours: 'دوام كامل', duration: '٣٠ يوماً', status: 'active', publishDate: '2025-02-01', endDate: '2025-05-01', createdAt: '2025-02-01' },
  { id: 2, entityId: 2, entityName: 'مؤسسة الحج الرائدة', entityLogo: '', title: 'منسق نقل', description: 'تنسيق عمليات نقل الحجاج بين مكة والمشاعر المقدسة.', location: 'منى', type: 'field', target: 'رجال', vacancies: 10, qualification: 'دبلوم', salary: '٦٬٠٠٠ - ٨٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات'], responsibilities: ['جدولة الحافلات', 'التنسيق مع شركات النقل', 'متابعة مواعيد التحرك'], conditions: ['رخصة قيادة سارية', 'خبرة في مجال النقل'], gender: 'male', hours: 'دوام كامل', duration: '٢٥ يوماً', status: 'active', publishDate: '2025-02-05', endDate: '2025-05-05', createdAt: '2025-02-05' },
  { id: 3, entityId: 2, entityName: 'مؤسسة الحج الرائدة', entityLogo: '', title: 'مضيفة خدمة', description: 'استقبال الحجاج وتقديم الخدمات المساندة لهم.', location: 'المدينة المنورة', type: 'office', target: 'نساء', vacancies: 20, qualification: 'ثانوية عامة', salary: '٥٬٠٠٠ - ٧٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات', 'بدل مواصلات'], responsibilities: ['استقبال الحجاج', 'تقديم الإرشادات', 'مساعدة كبار السن'], conditions: ['لغة إنجليزية جيدة', 'مهارات تواصل ممتازة'], gender: 'female', hours: 'دوام كامل', duration: '٢٠ يوماً', status: 'active', publishDate: '2025-02-10', endDate: '2025-05-10', createdAt: '2025-02-10' },
  { id: 4, entityId: 2, entityName: 'مؤسسة الحج الرائدة', entityLogo: '', title: 'سائق حافلة', description: 'قيادة حافلات نقل الحجاج بين المشاعر المقدسة.', location: 'مكة المكرمة', type: 'field', target: 'رجال', vacancies: 25, qualification: 'ثانوية عامة', salary: '٧٬٠٠٠ - ٩٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات'], responsibilities: ['قيادة الحافلة بأمان', 'الصيانة الدورية', 'التقيد بالجداول'], conditions: ['رخصة قيادة عامة', 'خبرة ٣ سنوات في قيادة الحافلات'], gender: 'male', hours: 'دوام كامل', duration: '٣٠ يوماً', status: 'active', publishDate: '2025-02-15', endDate: '2025-05-15', createdAt: '2025-02-15' },
  { id: 5, entityId: 2, entityName: 'مؤسسة الحج الرائدة', entityLogo: '', title: 'أخصائي صحي', description: 'تقديم الإسعافات الأولية والرعاية الصحية للحجاج.', location: 'عرفات', type: 'field', target: 'رجال ونساء', vacancies: 5, qualification: 'بكالوريوس تمريض', salary: '١٠٬٠٠٠ - ١٢٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات', 'بدل خطورة'], responsibilities: ['تقديم الإسعافات', 'متابعة الحالات', 'التنسيق مع المستشفيات'], conditions: ['ترخيص هيئة التخصصات الصحية', 'خبرة سنة'], gender: 'male', hours: 'دوام كامل', duration: '٢٥ يوماً', status: 'active', publishDate: '2025-02-20', endDate: '2025-05-20', createdAt: '2025-02-20' },
]

const applications: Application[] = [
  { id: 1, jobId: 1, userId: 1, userName: 'عبدالله الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', status: 'new', createdAt: '2025-02-10' },
  { id: 2, jobId: 1, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', status: 'shortlisted', createdAt: '2025-02-11' },
  { id: 3, jobId: 3, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', status: 'interview', createdAt: '2025-02-12' },
  { id: 4, jobId: 1, userId: 4, userName: 'محمد الشهري', userGender: 'male', userCity: 'الطائف', qualification: 'بكالوريوس', status: 'new', createdAt: '2025-02-13' },
  { id: 5, jobId: 2, userId: 1, userName: 'عبدالله الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', status: 'contract_sent', createdAt: '2025-02-14' },
  { id: 6, jobId: 4, userId: 4, userName: 'محمد الشهري', userGender: 'male', userCity: 'الطائف', qualification: 'ثانوية عامة', status: 'accepted', createdAt: '2025-02-15' },
  { id: 7, jobId: 5, userId: 1, userName: 'عبدالله الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس تمريض', status: 'refused', createdAt: '2025-02-16' },
]

const interviews: Interview[] = [
  { id: 1, applicationId: 3, jobId: 3, userId: 3, entityId: 2, method: 'in-person', date: '2025-03-01', time: '10:00', location: 'مكتب المؤسسة - مكة المكرمة', notes: 'يرجى إحضار الهوية', status: 'scheduled', attendance: 'pending', createdAt: '2025-02-20' },
  { id: 2, applicationId: 2, jobId: 1, userId: 3, entityId: 2, method: 'video', date: '2025-03-05', time: '14:00', link: 'https://meet.google.com/abc-defg-hij', notes: 'مقابلة عبر Google Meet', status: 'scheduled', attendance: 'pending', createdAt: '2025-02-21' },
]

const contracts: Contract[] = [
  { id: 1, applicationId: 5, jobId: 2, userId: 1, entityId: 2, fileUrl: '/contracts/contract-1.pdf', status: 'sent', createdAt: '2025-02-25' },
  { id: 2, applicationId: 6, jobId: 4, userId: 4, entityId: 2, fileUrl: '/contracts/contract-2.pdf', status: 'signed', signedAt: '2025-02-28', createdAt: '2025-02-25' },
]

function delay(ms = 300): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms))
}

function getUserFromToken(event: any): { userId: number, type: 'individual' | 'entity' } | null {
  const auth = getHeader(event, 'authorization')
  if (!auth || !auth.startsWith('Bearer ')) return null
  const token = auth.slice(7)
  return tokens[token] || null
}

function requireAuth(event: any): { userId: number, type: 'individual' | 'entity' } {
  const session = getUserFromToken(event)
  if (!session) {
    throw createError({ statusCode: 401, statusMessage: 'Unauthorized' })
  }
  return session
}

export type { User, Job, Application, Interview, Contract }

export {
  users, jobs, applications, interviews, contracts, tokens,
  generateToken, hashPassword, delay, getUserFromToken, requireAuth,
}
