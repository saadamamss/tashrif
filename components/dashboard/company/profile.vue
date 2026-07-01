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

const userData = ref({
  companyFiled: "لخدمات الحج والعمرة", // Saudi
  sector: "اسم القطاع", // 10-digit Saudi ID
  companySize: "متوسطة", // YYYY-MM-DD format
  number100700: "1829891", // Calculated from birthDate
  country: "سعودية", // Male (use "أنثى" for female)
  city: "الرياض", // Riyadh
  website: "www.capitalx.com",
  facebook: "facebook.com/capitalx",
  twitter: "twitter.com/capitalx",
  youtube: "youtube.com/capitalx",
  zone: "الرياض", // Eastern Province
  district: "حي النخيل", // Al-Nakheel District
  street: "شارع الملك فهد", // King Fahd Road
  zipcode: "12345", // Saudi ZIP code
  name: "سعودى",
  nationality: "سعودى",
  role: "مدير تنفيذى",
  email: "user.example@domain.com",
  phone: "+966501234567", // Saudi phone number with country code
});

const userComunicationInformation = computed(() => {
  return {
    name: {
      value: userData.value.name,
      key: " الإسم",
      icon: Earth,
    },

    role: {
      value: userData.value.role,
      key: " المنصب",
      icon: Earth,
    },
    nationality: {
      value: userData.value.nationality,
      key: " الجنسية",
      icon: Earth,
    },
    phone: {
      value: userData.value.phone,
      key: " رقم الحوال",
      icon: Earth,
    },
    email: {
      value: userData.value.email,
      key: " البريد الإلكترونى",
      icon: Earth,
    },
  };
});
const userInformation = computed(() => {
  return {
    companyFiled: {
      value: userData.value.companyFiled,
      key: " المجال",
      icon: Zone,
    },
    sector: {
      value: userData.value.sector,
      key: "القطاع",
      icon: City,
    },
    companySize: {
      value: userData.value.companySize,
      key: "حجم الشركة",
      icon: Zone,
    },
    number100700: {
      value: userData.value.number100700,
      key: " رقم ال 100-700",
      icon: Identity,
    },
    country: {
      value: userData.value.country,
      key: " الدولة",
      icon: Earth,
    },
    zone: {
      value: userData.value.zone,
      key: " المنطقة",
      icon: Zone,
    },
    website: {
      value: userData.value.website,
      key: " موقعك الإلكتروني",
      icon: Email,
    },
    facebook: {
      value: userData.value.facebook,
      key: "فيسبوك",
      icon: Call,
    },

    twitter: {
      value: userData.value.twitter,
      key: "تويتر",
      icon: Call,
    },
    youtube: {
      value: userData.value.youtube,
      key: "يوتيوب",
      icon: Call,
    },
  };
});

const userQualifications = ref([
  {
    qualification: "بكالوريوس",
    specialization: "إدارة أعمال",
    educational_institution: "جامعة أم القرى",
    graduation_year: "2025",
    grade: "جيد جداً",
  },
  {
    qualification: "الثانوية العامة",
    specialization: "",
    educational_institution: "مدرية مكة الثانوية",
    graduation_year: "2020",
    grade: "جيد جداً",
  },
]);
const userExperts = ref([
  {
    job_title: "مشرف ميدانى",
    authority: "شركة التنظيم الموسمى",
    duration: "من ذو القعدة 1444هـ إلى ذو الحجة 1444هـ",
    location: "مكةة المكرمة",
  },
  {
    job_title: "مشرف ميدانى",
    authority: "شركة التنظيم الموسمى",
    duration: "من ذو القعدة 1444هـ إلى ذو الحجة 1444هـ",
    location: "مكةة المكرمة",
  },
]);

const userFinancial = ref([
  {
    iban: "955435712452245445",
    bank_name: "الراجحى",
    iban_status: "صحيح",
    account_status: "فعال",
  },
]);

const cvs = ref([
  { name: "السيرة الذاتية 1", size: "2.67 ميجابايت" },
  { name: "السيرة الذاتية 2", size: "1.89 ميجابايت" },
  { name: "السيرة الذاتية 3", size: "3.12 ميجابايت" },
]);

const editQualification = () => {};
const deleteQualification = () => {};
//
const addqualifications = ref(false);
const addexperts = ref(false);
const addcvs = ref(false);
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
                class="border-4 border-[#f5f5f5] mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
              >
                <img
                  src="/images/partner-3.svg"
                  class="w-full h-full"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">شركة نسك لخدمات الحجاج</h1>
                <p class="text-base text-gray-500">لخدمات الحج والعمرة</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center justify-center gap-2 bg-[#f5f5f5] px-4 py-3 rounded-full text-sm border border-[#ecb42b]/0 hover:border-[#ecb42b] transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
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
                  24
                </h1>
                <h3 class="text-sm text-[#667178]">وظائفى المنشورة</h3>
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
                  12
                </h1>
                <h3 class="text-sm text-[#667178]">مقابلات العمل</h3>
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
                  12
                </h1>
                <h3 class="text-sm text-[#667178]">عروض العمل</h3>
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
              :percent="57"
              gradient-start="#ECB42B05"
              gradient-middle="#ECB42B01"
              gradient-end="#ECB42B"
              :size="94"
            />
            <div>
              <h1 class="text-base font-semibold mb-3">أستكمل ملفك الشخصي</h1>
              <p class="text-sm text-[#667178]">
                استكمالك لملفك الشخصي يساعدنا في ترشيح الوظائف الأنسب لك، ويزيد
                من فرص قبولك لدى الجهات. أضف معلوماتك الشخصية، مؤهلاتك، وخبراتك
                العملية لتظهر بشكل احترافي أمام أصحاب العمل.
              </p>
            </div>
          </div>
          <div class="">
            <nuxt-link to="#" class="btn-primary text-nowrap text-sm px-8">
              إستكمل ملفك الشخصى
            </nuxt-link>
          </div>
        </div>
      </div>
    </section>

    <!-- personal information  -->
    <div class="px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2">
        <h1 class="text-base font-bold">البيانات الشخصية</h1>
      </div>
      <div class="py-6">
        <div class="grid xs:grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-6">
          <div
            class="flex items-start gap-2"
            v-for="i in Object.values(userInformation)"
          >
            <component :is="i.icon"> </component>
            <div class="text-xs">
              <span class="block text-[#696C68] mb-2">{{ i.key }}</span>
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
              <span class="block text-[#696C68] mb-2">{{ i.key }}</span>
              <span class="block text-slate-800 font-bold">{{ i.value }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
