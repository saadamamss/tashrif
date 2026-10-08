<script setup>
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
import BriefcaseIcon from "~/components/icons/BriefcaseIcon.vue";
import PersonIcon from "~/components/icons/person.vue";
import FileIcon from "~/components/icons/file.vue";
import ProfileHeroSection from "~/components/dashboard/home/ProfileHeroSection.vue";
import CompletionSection from "~/components/dashboard/home/CompletionSection.vue";
import ProfileInfoSection from "~/components/dashboard/profile/ProfileInfoSection.vue";
import EditEntityProfile from "~/components/EditEntityProfile.vue";
import { buildImageUrl } from "~/services/help";

const loading = ref(true);
const editProfile = ref(false);
const companyData = ref({ name: '', email: '', phone: '', nationalId: '', companyField: '', sector: '', companySize: '', commercialReg: '', country: '', city: '', zone: '', district: '', street: '', zipcode: '', website: '', facebookUrl: '', twitterUrl: '', youtubeUrl: '', logoUrl: '' })
const contactPerson = ref(null)
const stats = ref({ totalJobs: 0, activeJobs: 0, totalApplicants: 0 })
const profileCompletion = ref(0)

const logoSrc = computed(() => buildImageUrl(companyData.value.logoUrl, '/images/partner-3.svg'));

const statsItems = computed(() => [
  { value: stats.value.totalJobs, label: 'وظائفى المنشورة', icon: BriefcaseIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.activeJobs, label: 'الوظائف النشطة', icon: PersonIcon, iconProps: { width: '22', height: '22', color: '#ECB42B' } },
  { value: stats.value.totalApplicants, label: 'إجمالي المتقدمين', icon: FileIcon, iconProps: { width: '24', height: '24', color: '#ECB42B' } },
])

const userComunicationInformation = computed(() => {
  const cp = contactPerson.value || {}
  return {
    name: {
      value: cp.name || companyData.value.name,
      key: " الإسم",
      icon: Earth,
    },
    role: {
      value: cp.role || cp.jobTitle || '',
      key: " المنصب",
      icon: Earth,
    },
    nationality: {
      value: cp.nationality || '',
      key: " الجنسية",
      icon: Earth,
    },
    phone: {
      value: cp.phone || companyData.value.phone,
      key: " رقم الحوال",
      icon: Earth,
    },
    email: {
      value: cp.email || companyData.value.email,
      key: " البريد الإلكترونى",
      icon: Earth,
    },
  };
});
const userInformation = computed(() => {
  return {
    companyFiled: {
      value: companyData.value.companyField,
      key: " المجال",
      icon: Zone,
    },
    sector: {
      value: companyData.value.sector,
      key: "القطاع",
      icon: City,
    },
    companySize: {
      value: companyData.value.companySize,
      key: "حجم الشركة",
      icon: Zone,
    },
    nationalId: {
      value: companyData.value.nationalId,
      key: " الرقم الوطني",
      icon: Identity,
    },
    commercialReg: {
      value: companyData.value.commercialReg,
      key: " رقم السجل التجاري",
      icon: Identity,
    },
    country: {
      value: companyData.value.country,
      key: " الدولة",
      icon: Earth,
    },
    zone: {
      value: companyData.value.zone,
      key: " المنطقة",
      icon: Zone,
    },
    website: {
      value: companyData.value.website,
      key: " موقعك الإلكتروني",
      icon: Email,
    },
    facebook: {
      value: companyData.value.facebookUrl,
      key: "فيسبوك",
      icon: Call,
    },
    twitter: {
      value: companyData.value.twitterUrl,
      key: "تويتر",
      icon: Call,
    },
    youtube: {
      value: companyData.value.youtubeUrl,
      key: "يوتيوب",
      icon: Call,
    },
  };
});

const loadProfile = async () => {
  const [profileRes, cpRes, statsRes] = await Promise.allSettled([
    useApi().get('/entities/profile'),
    useApi().get('/contact-persons'),
    useApi().get('/stats/entity'),
  ])
  if (profileRes.status === 'fulfilled' && profileRes.value.data) {
    companyData.value = profileRes.value.data
    profileCompletion.value = profileRes.value.data.profileCompletionPct || 0
  }
  if (cpRes.status === 'fulfilled' && cpRes.value.data) contactPerson.value = cpRes.value.data.items?.[0] || cpRes.value.data
  if (statsRes.status === 'fulfilled' && statsRes.value.data) stats.value = statsRes.value.data
}

onMounted(async () => {
  loading.value = true
  try {
    await loadProfile()
  } finally {
    loading.value = false
  }
})
</script>
<template>
  <div class="px-4 lg:px-0">
    <ProfileHeroSection
      :avatar-src="logoSrc"
      :name="companyData.name"
      :subtitle="companyData.companyField"
      :stats-items="statsItems"
      :columns="3"
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
      :skeleton-count="11"
    >
      <template #header-action>
        <NuxtLink to="/dashboard/change-password" class="text-sm text-primary hover:underline">
          تغيير كلمة المرور
        </NuxtLink>
      </template>
    </ProfileInfoSection>
    <ProfileInfoSection
      title="بيانات الإتصال"
      :items="Object.values(userComunicationInformation)"
      :loading="loading"
      :skeleton-count="5"
    />

    <EditEntityProfile v-model="editProfile" @saved="loadProfile" />
  </div>
</template>
