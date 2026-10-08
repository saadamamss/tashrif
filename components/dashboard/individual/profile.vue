<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import Earth from "~/components/icons/earth.vue";
import Identity from "~/components/icons/identity.vue";
import Calender from "~/components/icons/calender.vue";
import CalenderDay from "~/components/icons/calender-day.vue";
import Gender from "~/components/icons/gender.vue";
import Email from "~/components/icons/email.vue";
import Call from "~/components/icons/call.vue";
import City from "~/components/icons/city.vue";
import Zone from "~/components/icons/zone.vue";
import District from "~/components/icons/district.vue";
import Road from "~/components/icons/road.vue";
import ZipCode from "~/components/icons/zip-code.vue";
import Delete from "~/components/icons/delete.vue";
import Edit from "~/components/icons/edit.vue";
import Pdf from "~/components/icons/pdf.vue";
import ProfileHeroSection from "~/components/dashboard/home/ProfileHeroSection.vue";
import CompletionSection from "~/components/dashboard/home/CompletionSection.vue";
import ProfileInfoSection from "~/components/dashboard/profile/ProfileInfoSection.vue";
import ProfileListSection from "~/components/dashboard/profile/ProfileListSection.vue";
import AddQualification from "~/components/AddQualification.vue";
import AddExperts from "~/components/AddExperts.vue";
import AddCv from "~/components/AddCv.vue";
import AddBankAccount from "~/components/AddBankAccount.vue";
import EditIndividualProfile from "~/components/EditIndividualProfile.vue";
import BriefcaseIcon from "~/components/icons/BriefcaseIcon.vue";
import PersonIcon from "~/components/icons/person.vue";
import FileIcon from "~/components/icons/file.vue";
import { buildImageUrl } from "~/services/help";

const loading = ref(true);
const editProfile = ref(false);
const userData = ref({ name: '', email: '', phone: '', gender: '', nationality: '', birthDate: '', city: '', zone: '', district: '', street: '', zipcode: '', jobTitle: '', avatarUrl: '' })
const stats = ref({ totalApplications: 0, pendingApps: 0, interviews: 0, signedContracts: 0 })
const profileCompletion = ref(0)
const qualifications = ref([])
const experiences = ref([])
const bankAccounts = ref([])
const cvs = ref([])

const avatarSrc = computed(() => buildImageUrl(userData.value.avatarUrl, '/images/avatar.png'));

const statsItems = computed(() => [
  { value: stats.value.pendingApps, label: 'الطلبات المكتملة', icon: FileIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
  { value: stats.value.interviews, label: 'مقابلة عمل', icon: PersonIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.totalApplications, label: 'تقدمت للوظائف', icon: BriefcaseIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.signedContracts, label: 'عروض العمل', icon: FileIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
])

const userInformation = computed(() => {
  return {
    nationality: {
      value: userData.value.nationality,
      key: " الجنسية",
      icon: Earth,
    },
    nationalID: {
      value: userData.value.nationalId || '1012345678',
      key: "رقم الهوية الوطنية",
      icon: Identity,
    },
    birthDate: {
      value: userData.value.birthDate,
      key: " تاريخ الميلاد",
      icon: Calender,
    },
    age: {
      value: userData.value.birthDate ? Math.floor((Date.now() - new Date(userData.value.birthDate).getTime()) / 31557600000) : '',
      key: " العمر",
      icon: CalenderDay,
    },
    gender: {
      value: userData.value.gender,
      key: " الجنس",
      icon: Gender,
    },
    email: {
      value: userData.value.email,
      key: " البريد الإلكتروني",
      icon: Email,
    },
    phone: {
      value: userData.value.phone,
      key: " رقم الجوال",
      icon: Call,
    },
    city: {
      value: userData.value.city,
      key: " المدينة",
      icon: City,
    },
    zone: {
      value: userData.value.zone,
      key: " المنطقة",
      icon: Zone,
    },
    district: {
      value: userData.value.district,
      key: " الحي",
      icon: District,
    },
    street: {
      value: userData.value.street,
      key: " الشارع",
      icon: Road,
    },
    zipcode: {
      value: userData.value.zipcode,
      key: " الرمز البريدي",
      icon: ZipCode,
    },
  };
});

const addqualifications = ref(false);
const editingQualification = ref(null);
const addexperts = ref(false);
const addcvs = ref(false);
const cvToDelete = ref(null);
const deletingCv = ref(false);
const confirmDeleteCv = ref(false);

const loadCvs = async () => {
  const cvsRes = await useApi().get('/cvs')
  if (cvsRes.data) cvs.value = cvsRes.data.items || []
};

const deleteCv = (index) => {
  const cv = cvs.value[index]
  if (!cv) return
  cvToDelete.value = cv
  confirmDeleteCv.value = true
};

const confirmDeleteCvAction = async () => {
  if (!cvToDelete.value) return
  deletingCv.value = true
  try {
    const { error } = await useApi().delete(`/cvs/${cvToDelete.value.id}`)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم حذف السيرة الذاتية", "success")
    confirmDeleteCv.value = false
    cvToDelete.value = null
    await loadCvs()
  } catch {
    useToast().show("حدث خطأ أثناء حذف السيرة الذاتية", "error")
  } finally {
    deletingCv.value = false
  }
};

const cvUrl = (filePath) => buildImageUrl(filePath);
const qualificationToDelete = ref(null);
const deletingQualification = ref(false);
const confirmDeleteQualification = ref(false);

const loadQualifications = async () => {
  const qualsRes = await useApi().get('/qualifications')
  if (qualsRes.data) qualifications.value = qualsRes.data.items || qualsRes.data
};

const editQualification = (index) => {
  editingQualification.value = qualifications.value[index] || null
  addqualifications.value = true
};

const deleteQualification = (index) => {
  const q = qualifications.value[index]
  if (!q) return
  qualificationToDelete.value = q
  confirmDeleteQualification.value = true
};

const confirmDelete = async () => {
  if (!qualificationToDelete.value) return
  deletingQualification.value = true
  try {
    const { error } = await useApi().delete(`/qualifications/${qualificationToDelete.value.id}`)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم حذف المؤهل", "success")
    confirmDeleteQualification.value = false
    qualificationToDelete.value = null
    await loadQualifications()
  } catch {
    useToast().show("حدث خطأ أثناء حذف المؤهل", "error")
  } finally {
    deletingQualification.value = false
  }
};

const openAddQualification = () => {
  editingQualification.value = null
  addqualifications.value = true
};

const loadExperiences = async () => {
  const expsRes = await useApi().get('/experiences')
  if (expsRes.data) experiences.value = expsRes.data.items || expsRes.data
};

const editingExperience = ref(null);
const experienceToDelete = ref(null);
const deletingExperience = ref(false);
const confirmDeleteExperience = ref(false);

const openAddExperience = () => {
  editingExperience.value = null
  addexperts.value = true
};

const editExperience = (index) => {
  editingExperience.value = experiences.value[index] || null
  addexperts.value = true
};

const deleteExperience = (index) => {
  const e = experiences.value[index]
  if (!e) return
  experienceToDelete.value = e
  confirmDeleteExperience.value = true
};

const confirmDeleteExperienceAction = async () => {
  if (!experienceToDelete.value) return
  deletingExperience.value = true
  try {
    const { error } = await useApi().delete(`/experiences/${experienceToDelete.value.id}`)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم حذف الخبرة", "success")
    confirmDeleteExperience.value = false
    experienceToDelete.value = null
    await loadExperiences()
  } catch {
    useToast().show("حدث خطأ أثناء حذف الخبرة", "error")
  } finally {
    deletingExperience.value = false
  }
};

const addbank = ref(false);
const editingBank = ref(null);
const bankToDelete = ref(null);
const deletingBank = ref(false);
const confirmDeleteBank = ref(false);

const loadBankAccounts = async () => {
  const bankRes = await useApi().get('/bank-accounts')
  if (bankRes.data) bankAccounts.value = bankRes.data.items || []
};

const openAddBank = () => {
  editingBank.value = null
  addbank.value = true
};

const editBank = (index) => {
  editingBank.value = bankAccounts.value[index] || null
  addbank.value = true
};

const deleteBank = (index) => {
  const b = bankAccounts.value[index]
  if (!b) return
  bankToDelete.value = b
  confirmDeleteBank.value = true
};

const confirmDeleteBankAction = async () => {
  if (!bankToDelete.value) return
  deletingBank.value = true
  try {
    const { error } = await useApi().delete(`/bank-accounts/${bankToDelete.value.id}`)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم حذف الحساب البنكي", "success")
    confirmDeleteBank.value = false
    bankToDelete.value = null
    await loadBankAccounts()
  } catch {
    useToast().show("حدث خطأ أثناء حذف الحساب البنكي", "error")
  } finally {
    deletingBank.value = false
  }
};

const loadProfile = async () => {
  const [profileRes, statsRes] = await Promise.allSettled([
    useApi().get('/individuals/profile'),
    useApi().get('/stats/individual'),
  ])
  if (profileRes.status === 'fulfilled' && profileRes.value.data) {
    userData.value = profileRes.value.data
    profileCompletion.value = profileRes.value.data.profileCompletionPct || 0
  }
  if (statsRes.status === 'fulfilled' && statsRes.value.data) stats.value = statsRes.value.data
}

onMounted(async () => {
  loading.value = true
  try {
    const [qualsRes, expsRes, bankRes, cvsRes] = await Promise.allSettled([
      useApi().get('/qualifications'),
      useApi().get('/experiences'),
      useApi().get('/bank-accounts'),
      useApi().get('/cvs'),
      loadProfile(),
    ])
    if (qualsRes.status === 'fulfilled' && qualsRes.value.data) qualifications.value = qualsRes.value.data.items || qualsRes.value.data
    if (expsRes.status === 'fulfilled' && expsRes.value.data) experiences.value = expsRes.value.data.items || expsRes.value.data
    if (bankRes.status === 'fulfilled' && bankRes.value.data) bankAccounts.value = bankRes.value.data.items || bankRes.value.data
    if (cvsRes.status === 'fulfilled' && cvsRes.value.data) cvs.value = cvsRes.value.data.items || cvsRes.value.data
  } finally {
    loading.value = false
  }
})
</script>
<template>
  <div class="px-4 lg:px-0">
    <ProfileHeroSection
      :avatar-src="avatarSrc"
      :name="userData.name"
      :subtitle="userData.jobTitle"
      :stats-items="statsItems"
      :columns="4"
      :loading="loading"
    >
      <template #actions>
        <button
          @click="editProfile = true"
          class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
        >
          <EditSmall />
          <span> تعديل الملف الشخصى </span>
        </button>
      </template>
    </ProfileHeroSection>

    <CompletionSection :percent="profileCompletion" :loading="loading" @action="editProfile = true" />

    <!-- personal information  -->
    <ProfileInfoSection
      title="البيانات الشخصية"
      :items="Object.values(userInformation)"
      :loading="loading"
      :skeleton-count="12"
    >
      <template #header-action>
        <NuxtLink to="/dashboard/change-password" class="text-sm text-primary hover:underline">
          تغيير كلمة المرور
        </NuxtLink>
      </template>
    </ProfileInfoSection>

    <!-- user Qualifications -->
    <ProfileListSection title="المؤهلات" :loading="loading" :skeleton-count="2">
      <template #header-action>
        <button class="btn-primary text-sm" @click="openAddQualification">
          إضافة مؤهلات
        </button>
      </template>
      <div class="space-y-3">
          <div
            v-for="(q, index) in qualifications"
            :key="index"
            class="bg-bg-subtle rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-y-8 gap-x-4 text-right items-start"
            >
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">المؤهل</p>
                <p class="text-icon-muted">{{ q.type }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">التخصص</p>
                <p class="text-icon-muted">{{ q.specialization || "-" }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">المؤسسة التعليمية</p>
                <p class="text-icon-muted">{{ q.institution }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">سنة التخرج</p>
                <p class="text-icon-muted">{{ q.graduationYear }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">التقدير</p>
                <p class="text-icon-muted">{{ q.grade }}</p>
              </div>
              <div class="flex justify-end space-x-3 space-x-reverse">
                <button
                  @click="editQualification(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Edit class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
                <button
                  @click="deleteQualification(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Delete class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
              </div>
            </div>
          </div>
        </div>
    </ProfileListSection>

    <!-- Experts -->
    <ProfileListSection title="الخبرات" :loading="loading" :skeleton-count="2">
      <template #header-action>
        <button class="btn-primary text-sm" @click="openAddExperience">
          إضافة خبرة
        </button>
      </template>
      <div class="space-y-3">
          <div
            v-for="(e, index) in experiences"
            :key="index"
            class="bg-bg-subtle rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-6 lg:grid-cols-5 gap-y-8 gap-x-4 lg:gap-x-6 text-right items-start"
            >
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">المسمى الوظيفى</p>
                <p class="text-icon-muted">{{ e.jobTitle }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الجهة</p>
                <p class="text-icon-muted">{{ e.employer || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الفترة</p>
                <p class="text-icon-muted">{{ e.duration }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الموقع</p>
                <p class="text-icon-muted">{{ e.location }}</p>
              </div>
              <div
                class="flex justify-end space-x-3 space-x-reverse col-span-6 sm:col-span-4 lg:col-span-1"
              >
                <button
                  @click="editExperience(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Edit class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
                <button
                  @click="deleteExperience(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Delete class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
              </div>
            </div>
          </div>
        </div>
    </ProfileListSection>

    <!-- financial -->
    <ProfileListSection title="المعلومات المالية" :loading="loading" :skeleton-count="1">
      <template #header-action>
        <button class="btn-primary text-sm" @click="openAddBank">
          إضافة حساب بنكي
        </button>
      </template>
      <div class="space-y-3">
          <div
            v-for="(b, index) in bankAccounts"
            :key="index"
            class="bg-bg-subtle rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-6 lg:grid-cols-5 gap-y-8 gap-x-4 lg:gap-x-6 text-right items-start"
            >
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">رقم الأيبان</p>
                <p class="text-icon-muted">{{ b.iban }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">اسم البنك</p>
                <p class="text-icon-muted">{{ b.bankName || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">
                  حالة إرتباط الأيبان بالمستخدم
                </p>
                <p class="text-icon-muted">{{ b.ibanStatus }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">حالة الحساب</p>
                <p class="text-icon-muted">{{ b.accountStatus }}</p>
              </div>
              <div
                class="flex justify-end space-x-3 space-x-reverse col-span-6 sm:col-span-4 lg:col-span-1"
              >
                <button
                  @click="editBank(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Edit class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
                <button
                  @click="deleteBank(index)"
                  class="p-3 bg-white shadow-sm rounded-lg"
                >
                  <Delete class="w-4 h-4 lg:w-6 lg:h-6" />
                </button>
              </div>
            </div>
          </div>
        </div>
    </ProfileListSection>
    <!-- CVs -->
    <ProfileListSection title="السيرة الذاتية" :loading="loading" :skeleton-count="1" skeleton-height="72px">
      <template #header-action>
        <button class="btn-primary text-sm" @click="addcvs = true">
          إضافة سيرة ذاتية
        </button>
      </template>
      <div class="space-y-3">
          <div
            v-for="(cv, index) in cvs"
            :key="index"
            class="flex items-center justify-between p-4 bg-bg-light rounded-lg hover:border-primary transition-colors"
          >
            <div class="flex items-center">
              <a
                :href="cvUrl(cv.filePath)"
                target="_blank"
                class="flex gap-2 items-center cursor-pointer"
              >
                <span class="block p-2 bg-white rounded-xl">
                  <Pdf />
                </span>
                <div>
                  <span class="block text-slate-900 text-sm mb-1">
                    {{ cv.fileName || 'pdf السيرة الذاتية' }}
                  </span>
                  <span class="text-xs block text-slate-400">
                    {{ cv.fileSize }}
                  </span>
                </div>
              </a>
            </div>
            <button
              @click="deleteCv(index)"
              class="p-3 bg-white shadow-sm rounded-lg"
            >
              <Delete class="w-4 h-4 lg:w-6 lg:h-6" />
            </button>
          </div>
      </div>
    </ProfileListSection>

    <!-- dialogs -->
    <AddQualification
      v-model="addqualifications"
      :editing="editingQualification"
      @saved="loadQualifications"
    />
    <!--  -->
    <AddExperts
      v-model="addexperts"
      :editing="editingExperience"
      @saved="loadExperiences"
    />
    <!--  -->
    <AddCv v-model="addcvs" @saved="loadCvs" />
    <EditIndividualProfile v-model="editProfile" @saved="loadProfile" />
    <!--  -->
    <AddBankAccount
      v-model="addbank"
      :editing="editingBank"
      @saved="loadBankAccounts"
    />
    <!--  -->
    <ConfirmDialog
      v-model="confirmDeleteQualification"
      title="تأكيد حذف المؤهل"
      message="هل أنت متأكد من حذف هذا المؤهل؟ لا يمكن التراجع عن هذه العملية."
      confirm-text="حذف"
      :is-loading="deletingQualification"
      @confirm="confirmDelete"
    />
    <!--  -->
    <ConfirmDialog
      v-model="confirmDeleteExperience"
      title="تأكيد حذف الخبرة"
      message="هل أنت متأكد من حذف هذه الخبرة؟ لا يمكن التراجع عن هذه العملية."
      confirm-text="حذف"
      :is-loading="deletingExperience"
      @confirm="confirmDeleteExperienceAction"
    />
    <!--  -->
    <ConfirmDialog
      v-model="confirmDeleteBank"
      title="تأكيد حذف الحساب البنكي"
      message="هل أنت متأكد من حذف هذا الحساب البنكي؟ لا يمكن التراجع عن هذه العملية."
      confirm-text="حذف"
      :is-loading="deletingBank"
      @confirm="confirmDeleteBankAction"
    />
    <!--  -->
    <ConfirmDialog
      v-model="confirmDeleteCv"
      title="تأكيد حذف السيرة الذاتية"
      message="هل أنت متأكد من حذف هذه السيرة الذاتية؟ لا يمكن التراجع عن هذه العملية."
      confirm-text="حذف"
      :is-loading="deletingCv"
      @confirm="confirmDeleteCvAction"
    />
  </div>
</template>
