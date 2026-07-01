<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import PercentCircle from "~/components/PercentCircle.vue";
import JobCard from "~/components/JobCard.vue";
import JobRequestCard from "~/components/JobRequestCard.vue";
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
import AddQualification from "~/components/AddQualification.vue";
import AddExperts from "~/components/AddExperts.vue";
import AddCv from "~/components/AddCv.vue";
import BriefcaseIcon from "~/components/icons/BriefcaseIcon.vue";
import PersonIcon from "~/components/icons/person.vue";
import FileIcon from "~/components/icons/file.vue";

const userData = ref({
  nationality: "سعودي", // Saudi
  nationalID: "1012345678", // 10-digit Saudi ID
  birthDate: "1990-05-15", // YYYY-MM-DD format
  age: "33", // Calculated from birthDate
  gender: "ذكر", // Male (use "أنثى" for female)
  email: "user.example@domain.com",
  phone: "+966501234567", // Saudi phone number with country code
  city: "الرياض", // Riyadh
  zone: "المنطقة الشرقية", // Eastern Province
  district: "حي النخيل", // Al-Nakheel District
  street: "شارع الملك فهد", // King Fahd Road
  zipcode: "12345", // Saudi ZIP code
});

const userInformation = computed(() => {
  return {
    nationality: {
      value: userData.value.nationality,
      key: " الجنسية",
      icon: Earth,
    },
    nationalID: {
      value: userData.value.nationalID,
      key: "رقم الهوية الوطنية",
      icon: Identity,
    },
    birthDate: {
      value: userData.value.birthDate,
      key: " تاريخ الميلاد",
      icon: Calender,
    },
    age: {
      value: userData.value.age,
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
                class="border-4 mx-auto sm:mx-0 border-[#fff] rounded-full w-32 h-32 mb-4"
              >
                <img
                  src="~/assets/images/avatar.png"
                  class="w-full h-full"
                  alt="user avatar"
                />
              </div>
              <div>
                <h1 class="text-xl font-bold mb-3">عبدالله محمد الحربي</h1>
                <p class="text-base text-gray-500">مشرف تنظيم حشود</p>
              </div>
            </div>

            <div class="text-center sm:text-right">
              <NuxtLink
                to="/dashboard/profile"
                class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
            </div>
          </div>

          <div
            class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6 mt-8"
          >
            <div
              class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
            >
              <div>
                <h1
                  class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]"
                >
                  10
                </h1>
                <h3 class="text-sm text-muted">الطلبات المكتملة</h3>
              </div>
              <span>
                <FileIcon width="24" height="24" color="#ECB42B" />
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
                <h3 class="text-sm text-muted">مقابلة عمل</h3>
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
                  24
                </h1>
                <h3 class="text-sm text-muted">تقدمت للوظائف</h3>
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
      <div class="container-fluid p-8">
        <div
          class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between"
        >
          <div class="flex flex-col sm:flex-row gap-6 max-w-[760px]">
            <PercentCircle
              :percent="57"
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
            <nuxt-link to="#" class="btn-primary text-nowrap text-sm px-8">
              إستكمل ملفك الشخصى
            </nuxt-link>
          </div>
        </div>
      </div>
    </section>

    <!-- personal information  -->
    <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
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
              <span class="block text-icon-muted mb-2">{{ i.key }}</span>
              <span class="block text-slate-800 font-bold">{{ i.value }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- user Qualifications -->
    <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2 flex justify-between items-center">
        <h1 class="text-base font-bold">المؤهلات</h1>
        <button class="btn-primary text-sm" @click="addqualifications = true">
          إضافة مؤهلات
        </button>
      </div>
      <div class="py-6">
        <div class="space-y-3">
          <div
            v-for="(qual, index) in userQualifications"
            :key="index"
            class="bg-bg-subtle rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-y-8 gap-x-4 text-right items-start"
            >
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">المؤهل</p>
                <p class="text-icon-muted">{{ qual.qualification }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">التخصص</p>
                <p class="text-icon-muted">{{ qual.specialization || "-" }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">المؤسسة التعليمية</p>
                <p class="text-icon-muted">{{ qual.educational_institution }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">سنة التخرج</p>
                <p class="text-icon-muted">{{ qual.graduation_year }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-dark">التقدير</p>
                <p class="text-icon-muted">{{ qual.grade }}</p>
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
      </div>
    </div>

    <!-- Experts -->
    <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2 flex justify-between items-center">
        <h1 class="text-base font-bold">الخبرات</h1>
        <button class="btn-primary text-sm" @click="addexperts = true">
          إضافة خبرة
        </button>
      </div>
      <div class="py-6">
        <div class="space-y-3">
          <div
            v-for="(expert, index) in userExperts"
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
                <p class="text-icon-muted">{{ expert.job_title }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الجهة</p>
                <p class="text-icon-muted">{{ expert.authority || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الفترة</p>
                <p class="text-icon-muted">{{ expert.duration }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">الموقع</p>
                <p class="text-icon-muted">{{ expert.location }}</p>
              </div>
              <div
                class="flex justify-end space-x-3 space-x-reverse col-span-6 sm:col-span-4 lg:col-span-1"
              >
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
      </div>
    </div>

    <!-- financial -->
    <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2 flex justify-between items-center">
        <h1 class="text-base font-bold">المعلومات المالية</h1>
      </div>
      <div class="py-6">
        <div class="space-y-3">
          <div
            v-for="(item, index) in userFinancial"
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
                <p class="text-icon-muted">{{ item.iban }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">اسم البنك</p>
                <p class="text-icon-muted">{{ item.bank_name || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">
                  حالة إرتباط الأيبان بالمستخدم
                </p>
                <p class="text-icon-muted">{{ item.iban_status }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-dark">حالة الحساب</p>
                <p class="text-icon-muted">{{ item.account_status }}</p>
              </div>
              <div
                class="flex justify-end space-x-3 space-x-reverse col-span-6 sm:col-span-4 lg:col-span-1"
              >
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
      </div>
    </div>
    <!-- CVs -->
    <div class="px-4 sm:px-6 rounded-xl bg-white shadow-sm mt-4">
      <div class="py-6 border-b-2 flex justify-between items-center">
        <h1 class="text-base font-bold">السيرة الذاتية</h1>
        <button class="btn-primary text-sm" @click="addcvs = true">
          إضافة سيرة ذاتية
        </button>
      </div>
      <div class="py-6">
        <div class="space-y-3">
          <div
            v-for="(cv, index) in cvs"
            :key="index"
            class="flex items-center justify-between p-4 bg-bg-light rounded-lg hover:border-primary transition-colors"
          >
            <div class="flex items-center">
              <div class="flex gap-2 items-center cursor-pointer">
                <span class="block p-2 bg-white rounded-xl">
                  <Pdf />
                </span>
                <div>
                  <span class="block text-slate-900 text-sm mb-1">
                    pdf السيرة الذاتية
                  </span>
                  <span class="text-xs block text-slate-400">
                    {{ cv.size }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- dialogs -->
    <AddQualification v-model="addqualifications" />
    <!--  -->
    <AddExperts v-model="addexperts" />
    <!--  -->
    <AddCv v-model="addcvs" />
  </div>
</template>
