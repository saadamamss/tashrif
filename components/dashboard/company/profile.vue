<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import PercentCircle from "~/components/PercentCircle.vue";
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
import EditEntityProfile from "~/components/EditEntityProfile.vue";
import { buildImageUrl } from "~/services/help";

const loading = ref(false);
const editProfile = ref(false);
const companyData = ref({ name: '', email: '', phone: '', companyField: '', sector: '', companySize: '', commercialReg: '', country: '', city: '', zone: '', district: '', street: '', zipcode: '', website: '', facebookUrl: '', twitterUrl: '', youtubeUrl: '', logoUrl: '' })
const contactPerson = ref(null)
const stats = ref({ totalJobs: 0, activeJobs: 0, totalApplicants: 0 })
const profileCompletion = ref(0)

const logoSrc = computed(() => buildImageUrl(companyData.value.logoUrl, '/images/partner-3.svg'));

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
    <section
      class="bg-white profile-statistics rounded-xl shadow-sm overflow-hidden min-h-[495px]"
    >
      <div class="container-fluid">
        <div class="back-image h-[197px] overflow-hidden">
          <img
            src="~/assets/images/profile-back.png"
            class="w-full h-full object-cover"
            alt="back-profile"
          />
        </div>
        <div class="px-8 pb-8">
          <div
            class="flex flex-col sm:flex-row justify-between gap-6 items-center -mt-16"
          >
            <div class="text-center sm:text-right">
              <div
                class="border-4 border-bg-light mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
              >
                <img
                  :src="logoSrc"
                  class="w-full h-full object-cover"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">{{ companyData.name }}</h1>
                <p class="text-base text-gray-500">{{ companyData.companyField }}</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <button
                @click="editProfile = true"
                class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </button>
            </div>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-3 gap-6 mt-8">
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.totalJobs }}
                </h1>
                <h3 class="text-sm text-muted">وظائفى المنشورة</h3>
              </div>
              <span>
                <BriefcaseIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.activeJobs }}
                </h1>
                <h3 class="text-sm text-muted">مقابلات العمل</h3>
              </div>
              <span>
                <PersonIcon width="22" height="22" color="#ECB42B" />
              </span>
            </div>

            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  {{ stats.totalApplicants }}
                </h1>
                <h3 class="text-sm text-muted">عروض العمل</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
              </span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!--  -->
    <section class="complete-profile bg-white rounded-xl shadow-sm mt-4">
      <div class="container-fluid p-6 sm:p-8">
        <div
          class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
        >
          <div class="flex flex-col xs:flex-row gap-6 max-w-[760px]">
            <PercentCircle
              :percent="profileCompletion"
              gradient-start="#ECB42B05"
              gradient-middle="#ECB42B01"
              gradient-end="#ECB42B"
              :size="94"
            />
            <div>
              <h1 class="text-base font-semibold mb-3">أستكمل ملفك الشخصي</h1>
              <p class="text-sm text-muted">
                استكمالك لملفك الشخصي يساعدنا في ترشيح الوظائف الأنسب لك، ويزيد
                من فرص قبولك لدى الجهات. أضف معلوماتك الشخصية، مؤهلاتك، وخبراتك
                العملية لتظهر بشكل احترافي أمام أصحاب العمل.
              </p>
            </div>
          </div>
          <div class="">
            <button
              @click="editProfile = true"
              class="btn-primary text-nowrap text-sm px-8"
            >
              إستكمل ملفك الشخصى
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- personal information  -->
    <div class="px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2 flex justify-between items-center">
        <h1 class="text-base font-bold">البيانات الشخصية</h1>
        <NuxtLink to="/dashboard/change-password" class="text-sm text-primary hover:underline">
          تغيير كلمة المرور
        </NuxtLink>
      </div>
      <div class="py-6">
        <div class="grid xs:grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-6">
          <div
            class="flex items-start gap-2"
            v-for="i in Object.values(userInformation)"
          >
            <component :is="i.icon"> </component>
            <div class="text-xs">
              <span class="block text-icon-muted mb-2">{{ i.key }}</span>
              <span class="block text-slate-800 font-bold">{{ i.value }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2">
        <h1 class="text-base font-bold">بيانات الإتصال</h1>
      </div>
      <div class="py-6">
        <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-6">
          <div
            class="flex items-start gap-2"
            v-for="i in Object.values(userComunicationInformation)"
          >
            <component :is="i.icon"> </component>
            <div class="text-xs">
              <span class="block text-icon-muted mb-2">{{ i.key }}</span>
              <span class="block text-slate-800 font-bold">{{ i.value }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <EditEntityProfile v-model="editProfile" @saved="loadProfile" />
  </div>
</template>
