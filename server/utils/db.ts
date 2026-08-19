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
  avatarUrl?: string
  createdAt: string
}

interface IndividualProfile {
  id: number
  userId: number
  birthDate: string
  city: string
  zone: string
  district: string
  street: string
  zipcode: string
  jobTitle: string
  profileCompletionPct: number
}

interface Qualification {
  id: number
  userId: number
  type: string
  specialization: string
  institution: string
  graduationYear: number
  grade: string
}

interface Experience {
  id: number
  userId: number
  jobTitle: string
  employer: string
  duration: string
  location: string
  isCurrent: boolean
  startDate: string
  endDate: string
}

interface Cv {
  id: number
  userId: number
  fileName: string
  filePath: string
  fileSize: number
  isDefault: boolean
  uploadedAt: string
}

interface BankAccount {
  id: number
  userId: number
  iban: string
  bankName: string
  ibanStatus: string
  accountStatus: string
}

interface EntityProfile {
  id: number
  userId: number
  companyField: string
  sector: string
  companySize: string
  commercialReg: string
  country: string
  city: string
  zone: string
  district: string
  street: string
  zipcode: string
  website: string
  facebookUrl: string
  twitterUrl: string
  youtubeUrl: string
  logoUrl: string
  profileCompletionPct: number
}

interface ContactPerson {
  id: number
  entityId: number
  name: string
  role: string
  nationality: string
  phone: string
  email: string
  isPrimary: boolean
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
  applicantCount?: number
}

interface Application {
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
  fileSize: number
  userName?: string
  userAvatar?: string
  jobTitle?: string
  entityName?: string
  entityLogo?: string
  fileName?: string
  notes?: string
  endDate?: string
  status: 'sent' | 'signed' | 'rejected'
  signedAt?: string
  createdAt: string
}

interface Notification {
  id: number
  userId: number
  title: string
  body: string
  type: string
  referenceId?: number
  referenceType?: string
  isRead: boolean
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
  { id: 1, nationalId: '1010101010', password: hashPassword('individual'), name: 'عبدالله محمد الحربي', email: 'abdullah@gmail.com', phone: '0500000001', type: 'individual', gender: 'male', nationality: 'سعودي', avatarUrl: '/images/avatar-1.png', createdAt: '2025-01-15' },
  { id: 2, nationalId: '2020202020', password: hashPassword('entity'), name: 'شركة نسك لخدمات الحجاج', email: 'info@nusk.com', phone: '0500000002', type: 'entity', avatarUrl: '/images/partner-3.svg', createdAt: '2025-01-10' },
  { id: 3, nationalId: '3030303030', password: hashPassword('individual2'), name: 'سارة القحطاني', email: 'sara@gmail.com', phone: '0500000003', type: 'individual', gender: 'female', nationality: 'سعودية', avatarUrl: '/images/avatar-2.png', createdAt: '2025-02-01' },
  { id: 4, nationalId: '4040404040', password: hashPassword('individual3'), name: 'محمد الشهري', email: 'mohammed@gmail.com', phone: '0500000004', type: 'individual', gender: 'male', nationality: 'سعودي', createdAt: '2025-02-10' },
]

const individualProfiles: IndividualProfile[] = [
  { id: 1, userId: 1, birthDate: '1990-05-15', city: 'الرياض', zone: 'منطقة الرياض', district: 'حي النخيل', street: 'شارع الملك فهد', zipcode: '12345', jobTitle: 'مشرف تنظيم حشود', profileCompletionPct: 57 },
  { id: 2, userId: 3, birthDate: '1995-08-22', city: 'جدة', zone: 'منطقة مكة المكرمة', district: 'حي الشاطئ', street: 'شارع الأمير سلطان', zipcode: '23456', jobTitle: 'طبيبة أسنان', profileCompletionPct: 70 },
  { id: 3, userId: 4, birthDate: '1988-12-10', city: 'الطائف', zone: 'منطقة مكة المكرمة', district: 'حي المسفلة', street: 'شارع أبو بكر الصديق', zipcode: '34567', jobTitle: 'مهندس برمجيات', profileCompletionPct: 45 },
]

const qualifications: Qualification[] = [
  { id: 1, userId: 1, type: 'بكالوريوس', specialization: 'إدارة أعمال', institution: 'جامعة الملك سعود', graduationYear: 2015, grade: 'جيد جداً' },
  { id: 2, userId: 1, type: 'شهادة مهنية', specialization: 'إدارة الحشود', institution: 'الهيئة العامة للحج والعمرة', graduationYear: 2023, grade: 'ممتاز' },
  { id: 3, userId: 3, type: 'دكتوراه', specialization: 'طب الأسنان', institution: 'جامعة الملك عبدالعزيز', graduationYear: 2020, grade: 'ممتاز' },
  { id: 4, userId: 4, type: 'بكالوريوس', specialization: 'علوم الحاسب', institution: 'جامعة الملك فهد للبترول والمعادن', graduationYear: 2012, grade: 'جيد جداً' },
]

const experiences: Experience[] = [
  { id: 1, userId: 1, jobTitle: 'مشرف فرقة', employer: 'شركة المشاعر المقدسة', duration: 'سنتان', location: 'مكة المكرمة', isCurrent: false, startDate: '2022-01-01', endDate: '2023-12-31' },
  { id: 2, userId: 1, jobTitle: 'منسق خدمات', employer: 'مؤسسة الحج الرائدة', duration: 'سنة', location: 'الرياض', isCurrent: true, startDate: '2024-01-01', endDate: '' },
  { id: 3, userId: 3, jobTitle: 'طبيبة أسنان', employer: 'مستشفى الملك فيصل التخصصي', duration: '٣ سنوات', location: 'جدة', isCurrent: true, startDate: '2021-06-01', endDate: '' },
  { id: 4, userId: 4, jobTitle: 'مطور ويب', employer: 'شركة تقنية المعلومات', duration: '٥ سنوات', location: 'الرياض', isCurrent: false, startDate: '2017-03-01', endDate: '2022-02-28' },
]

const cvs: Cv[] = [
  { id: 1, userId: 1, fileName: 'السيرة الذاتية - عبدالله الحربي.pdf', filePath: '/cvs/cv-1.pdf', fileSize: 2_400_000, isDefault: true, uploadedAt: '2025-01-20' },
  { id: 2, userId: 1, fileName: 'السيرة الذاتية بالعربية.pdf', filePath: '/cvs/cv-1-ar.pdf', fileSize: 1_800_000, isDefault: false, uploadedAt: '2025-02-10' },
  { id: 3, userId: 3, fileName: 'السيرة الذاتية - سارة.pdf', filePath: '/cvs/cv-3.pdf', fileSize: 1_500_000, isDefault: true, uploadedAt: '2025-02-05' },
  { id: 4, userId: 4, fileName: 'CV - Mohammed AlShehri.pdf', filePath: '/cvs/cv-4.pdf', fileSize: 2_100_000, isDefault: true, uploadedAt: '2025-02-12' },
]

const bankAccounts: BankAccount[] = [
  { id: 1, userId: 1, iban: 'SA0380000000608010167515', bankName: 'البنك الأهلي السعودي', ibanStatus: 'verified', accountStatus: 'active' },
  { id: 2, userId: 3, iban: 'SA1210000001234567891234', bankName: 'بنك الراجحي', ibanStatus: 'pending', accountStatus: 'active' },
  { id: 3, userId: 4, iban: 'SA5560000000987654321098', bankName: 'بنك الرياض', ibanStatus: 'verified', accountStatus: 'active' },
]

const entityProfiles: EntityProfile[] = [
  {
    id: 1, userId: 2,
    companyField: 'لخدمات الحج والعمرة', sector: 'قطاع الحج والعمرة', companySize: 'متوسطة',
    commercialReg: '101829891', country: 'السعودية', city: 'الرياض', zone: 'منطقة الرياض',
    district: 'حي الملقا', street: 'شارع التحلية', zipcode: '13515',
    website: 'www.nusk.sa', facebookUrl: 'facebook.com/nusk', twitterUrl: 'twitter.com/nusk', youtubeUrl: 'youtube.com/nusk',
    logoUrl: '/images/partner-3.svg', profileCompletionPct: 85,
  },
]

const contactPersons: ContactPerson[] = [
  { id: 1, entityId: 1, name: 'أحمد السبيعي', role: 'مدير الموارد البشرية', nationality: 'سعودي', phone: '0599999001', email: 'ahmed@nusk.sa', isPrimary: true },
  { id: 2, entityId: 1, name: 'خالد الحارثي', role: 'مساعد إداري', nationality: 'سعودي', phone: '0599999002', email: 'khalid@nusk.sa', isPrimary: false },
]

const jobs: Job[] = [
  { id: 1, entityId: 2, entityName: 'شركة نسك لخدمات الحجاج', entityLogo: '/images/partner-3.svg', title: 'مشرف حجاج', description: 'الإشراف على مجموعة من الحجاج وضمان تنقلهم بين المشاعر المقدسة وفق الخطة المقررة. تبَحث شركة الإسناد الموسمي لخدمات الحجاج عن أفراد مؤهلين للانضمام إلى فريقها كمشرفين ميدانيين خلال موسم الحج. ستكون مسؤولاً عن تنظيم وتوجيه الحجاج والتأكد من سلامتهم وراحتهم طوال فترة المناسك.', location: 'مكة المكرمة – المشاعر المقدسة (منى – مزدلفة – عرفات)', type: 'field', target: 'رجال', vacancies: 15, qualification: 'بكالوريوس', salary: '٨٬٠٠٠ - ١٠٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن مجاني', 'وجبات يومية', 'تذاكر سفر', 'بدل مواصلات'], responsibilities: ['متابعة الحجاج يومياً والتأكد من التزامهم بالجدول', 'التنسيق مع مشرفي المجموعات الأخرى', 'الإبلاغ عن أي طوارئ أو حالات مرضية', 'تقديم تقارير يومية للإدارة', 'تنظيم تحركات المجموعة بين المشاعر'], conditions: ['خبرة سنتين على الأقل في مجال الإشراف', 'القدرة على العمل لساعات طويلة', 'حسن السيرة والسلوك', 'اللياقة البدنية والقدرة على المشي لمسافات طويلة', 'إجادة استخدام تطبيقات التواصل', 'شهادة حسن سيرة وسلوك'], gender: 'male', hours: 'دوام كامل – 8 ساعات', duration: '10 أيام (من 1 ذو الحجة حتى 10 ذو الحجة)', status: 'active', publishDate: '2025-02-01', endDate: '2025-05-01', createdAt: '2025-02-01' },
  { id: 2, entityId: 2, entityName: 'شركة نسك لخدمات الحجاج', entityLogo: '/images/partner-3.svg', title: 'منسق نقل', description: 'تنسيق عمليات نقل الحجاج بين مكة والمشاعر المقدسة.', location: 'منى', type: 'field', target: 'رجال', vacancies: 10, qualification: 'دبلوم', salary: '٦٬٠٠٠ - ٨٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات'], responsibilities: ['جدولة الحافلات', 'التنسيق مع شركات النقل', 'متابعة مواعيد التحرك'], conditions: ['رخصة قيادة سارية', 'خبرة في مجال النقل'], gender: 'male', hours: 'دوام كامل', duration: '٢٥ يوماً', status: 'active', publishDate: '2025-02-05', endDate: '2025-05-05', createdAt: '2025-02-05' },
  { id: 3, entityId: 2, entityName: 'شركة نسك لخدمات الحجاج', entityLogo: '/images/partner-3.svg', title: 'مضيفة خدمة', description: 'استقبال الحجاج وتقديم الخدمات المساندة لهم.', location: 'المدينة المنورة', type: 'office', target: 'نساء', vacancies: 20, qualification: 'ثانوية عامة', salary: '٥٬٠٠٠ - ٧٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات', 'بدل مواصلات'], responsibilities: ['استقبال الحجاج', 'تقديم الإرشادات', 'مساعدة كبار السن'], conditions: ['لغة إنجليزية جيدة', 'مهارات تواصل ممتازة'], gender: 'female', hours: 'دوام كامل', duration: '٢٠ يوماً', status: 'active', publishDate: '2025-02-10', endDate: '2025-05-10', createdAt: '2025-02-10' },
  { id: 4, entityId: 2, entityName: 'شركة نسك لخدمات الحجاج', entityLogo: '/images/partner-3.svg', title: 'سائق حافلة', description: 'قيادة حافلات نقل الحجاج بين المشاعر المقدسة.', location: 'مكة المكرمة', type: 'field', target: 'رجال', vacancies: 25, qualification: 'ثانوية عامة', salary: '٧٬٠٠٠ - ٩٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات'], responsibilities: ['قيادة الحافلة بأمان', 'الصيانة الدورية', 'التقيد بالجداول'], conditions: ['رخصة قيادة عامة', 'خبرة ٣ سنوات في قيادة الحافلات'], gender: 'male', hours: 'دوام كامل', duration: '٣٠ يوماً', status: 'active', publishDate: '2025-02-15', endDate: '2025-05-15', createdAt: '2025-02-15' },
  { id: 5, entityId: 2, entityName: 'شركة نسك لخدمات الحجاج', entityLogo: '/images/partner-3.svg', title: 'أخصائي صحي', description: 'تقديم الإسعافات الأولية والرعاية الصحية للحجاج.', location: 'عرفات', type: 'field', target: 'رجال ونساء', vacancies: 5, qualification: 'بكالوريوس تمريض', salary: '١٠٬٠٠٠ - ١٢٬٠٠٠ ريال', benefits: ['تأمين طبي', 'سكن', 'وجبات', 'بدل خطورة'], responsibilities: ['تقديم الإسعافات', 'متابعة الحالات', 'التنسيق مع المستشفيات'], conditions: ['ترخيص هيئة التخصصات الصحية', 'خبرة سنة'], gender: 'male', hours: 'دوام كامل', duration: '٢٥ يوماً', status: 'active', publishDate: '2025-02-20', endDate: '2025-05-20', createdAt: '2025-02-20' },
]

const applications: Application[] = [
  { id: 1, jobId: 1, userId: 1, userName: 'عبدالله محمد الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', experience: 'خبرة سنتين في إدارة الحشود', cvId: 1, status: 'new', createdAt: '2025-02-10' },
  { id: 2, jobId: 1, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', experience: 'خبرة في القطاع الصحي', cvId: 3, status: 'shortlisted', createdAt: '2025-02-11' },
  { id: 3, jobId: 3, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', coverLetter: 'أنا مهتمة جداً بهذه الوظيفة وأتمنى الانضمام لفريقكم', cvId: 3, status: 'interview', createdAt: '2025-02-12' },
  { id: 4, jobId: 1, userId: 4, userName: 'محمد الشهري', userGender: 'male', userCity: 'الطائف', qualification: 'بكالوريوس', cvId: 4, status: 'new', createdAt: '2025-02-13' },
  { id: 5, jobId: 2, userId: 1, userName: 'عبدالله محمد الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', cvId: 1, status: 'contract_sent', createdAt: '2025-02-14' },
  { id: 6, jobId: 4, userId: 4, userName: 'محمد الشهري', userGender: 'male', userCity: 'الطائف', qualification: 'ثانوية عامة', cvId: 4, status: 'accepted', createdAt: '2025-02-15' },
  { id: 7, jobId: 5, userId: 1, userName: 'عبدالله محمد الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس تمريض', cvId: 1, status: 'refused', createdAt: '2025-02-16' },
  { id: 8, jobId: 2, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', cvId: 3, status: 'new', createdAt: '2025-02-18' },
  { id: 9, jobId: 4, userId: 1, userName: 'عبدالله محمد الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', cvId: 1, status: 'interview', createdAt: '2025-02-20' },
  { id: 10, jobId: 1, userId: 1, userName: 'عبدالله محمد الحربي', userGender: 'male', userCity: 'الرياض', qualification: 'بكالوريوس', cvId: 2, status: 'new', createdAt: '2025-02-22' },
  { id: 11, jobId: 3, userId: 4, userName: 'محمد الشهري', userGender: 'male', userCity: 'الطائف', qualification: 'بكالوريوس', cvId: 4, status: 'shortlisted', createdAt: '2025-02-25' },
  { id: 12, jobId: 5, userId: 3, userName: 'سارة القحطاني', userGender: 'female', userCity: 'جدة', qualification: 'ماجستير', coverLetter: 'لدي خبرة واسعة في المجال الصحي', cvId: 3, status: 'new', createdAt: '2025-03-01' },
]

const interviews: Interview[] = [
  { id: 1, applicationId: 3, jobId: 3, userId: 3, entityId: 2, method: 'in-person', date: '2025-03-01', time: '10:00', location: 'مكتب المؤسسة - مكة المكرمة', link: '', notes: 'يرجى إحضار الهوية', status: 'completed', attendance: 'present', createdAt: '2025-02-20' },
  { id: 2, applicationId: 2, jobId: 1, userId: 3, entityId: 2, method: 'video', date: '2025-03-05', time: '14:00', location: '', link: 'https://meet.google.com/abc-defg-hij', notes: 'مقابلة عبر Google Meet', status: 'completed', attendance: 'present', createdAt: '2025-02-21' },
  { id: 3, applicationId: 9, jobId: 4, userId: 1, entityId: 2, method: 'in-person', date: '2025-07-10', time: '10:00', location: 'حي العزيزية, مكة المكرمة', link: '', notes: 'يرجى الحضور قبل الموعد بـ 15 دقيقة', status: 'scheduled', attendance: 'pending', createdAt: '2025-03-01' },
  { id: 4, applicationId: 11, jobId: 3, userId: 4, entityId: 2, method: 'phone', date: '2025-07-15', time: '11:00', location: '', link: '', notes: 'سيتم الاتصال على الرقم المسجل', status: 'scheduled', attendance: 'pending', createdAt: '2025-03-02' },
]

const contracts: Contract[] = [
  { id: 1, applicationId: 5, jobId: 2, userId: 1, entityId: 2, fileUrl: '/contracts/contract-1.pdf', fileSize: 1_200_000, status: 'sent', createdAt: '2025-02-25' },
  { id: 2, applicationId: 6, jobId: 4, userId: 4, entityId: 2, fileUrl: '/contracts/contract-2.pdf', fileSize: 1_000_000, status: 'signed', signedAt: '2025-02-28', createdAt: '2025-02-25' },
]

const notifications: Notification[] = [
  { id: 1, userId: 1, title: 'تم تقديم طلبك بنجاح', body: 'تم استلام طلب التقديم على وظيفة مشرف حجاج', type: 'application', referenceId: 1, referenceType: 'application', isRead: false, createdAt: '2025-02-10' },
  { id: 2, userId: 1, title: 'تم إرسال العقد', body: 'تم إرسال عقد العمل لوظيفة منسق نقل, يرجى مراجعته وتوقيعه', type: 'contract', referenceId: 1, referenceType: 'contract', isRead: false, createdAt: '2025-02-25' },
  { id: 3, userId: 3, title: 'تم تحديد مقابلة', body: 'تم تحديد موعد مقابلة لوظيفة مضيفة خدمة', type: 'interview', referenceId: 1, referenceType: 'interview', isRead: false, createdAt: '2025-02-20' },
  { id: 4, userId: 2, title: 'متقدم جديد', body: 'تم استلام طلب تقديم جديد لوظيفة مشرف حجاج من عبدالله الحربي', type: 'application', referenceId: 4, referenceType: 'application', isRead: true, createdAt: '2025-02-13' },
  { id: 5, userId: 2, title: 'تم توقيع العقد', body: 'قام محمد الشهري بتوقيع عقد العمل لوظيفة سائق حافلة', type: 'contract', referenceId: 2, referenceType: 'contract', isRead: false, createdAt: '2025-02-28' },
  { id: 6, userId: 2, title: 'تحديث حالة المتقدم', body: 'تم تغيير حالة المتقدم سارة القحطاني إلى مقابلة', type: 'application', referenceId: 3, referenceType: 'application', isRead: false, createdAt: '2025-02-20' },
]

function delay(ms = 300): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms))
}

function getUserFromToken(event: any): { userId: number, type: 'individual' | 'entity' } | null {
  const accessToken = getCookie(event, 'access_token')
  if (accessToken && tokens[accessToken]) return tokens[accessToken]

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

export type { User, IndividualProfile, Qualification, Experience, Cv, BankAccount, EntityProfile, ContactPerson, Job, Application, Interview, Contract, Notification }

export {
  users, individualProfiles, qualifications, experiences, cvs, bankAccounts,
  entityProfiles, contactPersons,
  jobs, applications, interviews, contracts, notifications, tokens,
  generateToken, hashPassword, delay, getUserFromToken, requireAuth,
}
