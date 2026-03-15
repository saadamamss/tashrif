<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import PercentCircle from "~/components/percent-circle.vue";
import JobCard from "~/components/job-card.vue";
import JobRequestCard from "~/components/job-request-card.vue";
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
import AddQualification from "~/components/add-qualification.vue";
import AddExperts from "~/components/add-experts.vue";
import AddCv from "~/components/add-cv.vue";

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
                class="flex items-center justify-center gap-2 bg-[#f5f5f5] px-4 py-3 rounded-full text-sm border border-[#ecb42b]/0 hover:border-[#ecb42b] transition"
              >
                <svg
                  width="16"
                  height="16"
                  viewBox="0 0 16 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M10.1348 1.19588C10.9246 0.726207 11.8985 0.711489 12.7017 1.15741C13.0349 1.34238 13.3388 1.65304 13.7908 2.11506L13.9135 2.24045C14.3671 2.70347 14.6706 3.01335 14.8509 3.35212C15.2832 4.16485 15.2692 5.14805 14.8134 5.94772C14.6751 6.19049 14.4718 6.41596 14.1912 6.69408C14.0605 6.82368 13.9951 6.88848 13.9125 6.88831C13.8299 6.88813 13.764 6.82221 13.6322 6.69037L9.37133 2.42951C9.24222 2.3004 9.17767 2.23585 9.17647 2.15474C9.17527 2.07364 9.23724 2.00788 9.36118 1.87638C9.65262 1.56717 9.88411 1.345 10.1348 1.19588Z"
                    fill="#161614"
                  />
                  <path
                    d="M9.16538 13.833C8.7972 13.833 8.49872 14.1315 8.49872 14.4997C8.49872 14.8679 8.7972 15.1663 9.16539 15.1663H14.4987C14.8669 15.1663 15.1654 14.8679 15.1654 14.4997C15.1654 14.1315 14.8669 13.833 14.4987 13.833H9.16538Z"
                    fill="#161614"
                  />
                  <path
                    opacity="0.4"
                    d="M12.6558 8.17533C12.7939 8.04226 12.863 7.97572 12.8638 7.89178C12.8646 7.80784 12.7968 7.74002 12.6611 7.60439L8.48908 3.43233C8.35078 3.29404 8.28163 3.22489 8.19645 3.22659C8.11127 3.2283 8.04495 3.30016 7.9123 3.44388L2.76168 9.02433C2.01497 9.83311 1.55731 10.3288 1.28941 10.9454C1.02202 11.5609 0.969369 12.238 0.883273 13.3452L0.883124 13.3471C0.882969 13.3491 0.826131 14.0762 0.834905 14.2336C0.844841 14.4119 0.887303 14.6287 1.05278 14.8164C1.21974 15.0059 1.43079 15.0741 1.60847 15.1037C1.76303 15.1295 2.45915 15.1476 2.46056 15.1477C3.72532 15.181 4.50716 15.2016 5.22614 14.9207C5.9441 14.6403 6.51148 14.0936 7.43205 13.2066L12.6558 8.17533Z"
                    fill="#161614"
                  />
                </svg>
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
                <h3 class="text-sm text-[#667178]">الطلبات المكتملة</h3>
              </div>
              <span>
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M7.43605 22.6472C8.4183 22.75 9.6511 22.75 11.1922 22.75H12.0675C13.9605 22.75 15.4744 22.75 16.6624 22.5991C17.8869 22.4436 18.9123 22.1129 19.7311 21.3394C20.5583 20.558 20.9197 19.5662 21.0882 18.3821C21.2496 17.2484 21.2495 15.8086 21.2495 14.0298V7.88003C21.2495 7.08088 21.2495 6.43332 21.2148 5.90474C21.179 5.36012 21.1034 4.87444 20.9201 4.41028C20.3913 3.07097 19.2829 2.0367 17.9063 1.5517C17.0475 1.24911 15.5754 1.2495 14.0204 1.25005C11.1808 1.24969 9.49416 1.24947 8.11172 1.73655C5.89173 2.51873 4.11799 4.18147 3.2746 6.31753C2.99349 7.02949 2.86895 7.7942 2.8087 8.71104C2.74951 9.61176 2.74951 10.7244 2.74951 12.1427V14.7262C2.74949 16.1743 2.74947 17.3469 2.85936 18.2842C2.97347 19.2576 3.21672 20.0955 3.78655 20.8146C4.08174 21.1871 4.43028 21.5159 4.82065 21.7919C5.56541 22.3186 6.42663 22.5416 7.43605 22.6472ZM18.3964 19.9161C18.0033 20.2875 17.4472 20.5293 16.4179 20.66C15.3664 20.7936 13.9762 20.7954 11.9995 20.7954H11.2471C9.63849 20.7954 8.50736 20.7942 7.63791 20.7032C6.78399 20.6139 6.30006 20.4471 5.94223 20.1941C5.70078 20.0233 5.4884 19.8222 5.31061 19.5979C5.05505 19.2754 4.8856 18.8426 4.79337 18.0558C4.69844 17.246 4.69686 16.1887 4.69686 14.6653L4.69678 11.7651C4.69676 11.4981 4.69694 10.8595 4.97756 10.3873C5.15049 10.0963 5.3719 9.85324 5.59935 9.72942C5.96586 9.5299 6.38606 9.41654 6.83273 9.41654L7.87309 9.45302C8.25319 9.46092 8.69672 9.44891 9.12487 9.33419C9.95885 9.11073 10.6103 8.45932 10.8337 7.62535C10.9484 7.1972 10.9604 6.75366 10.9525 6.37357L10.9161 5.3332C10.9161 4.86551 11.0404 4.45838 11.2578 4.07997C11.3845 3.85938 11.6052 3.65749 11.91 3.48018C12.3721 3.21127 12.9255 3.20878 13.2637 3.20726C13.6018 3.20573 13.9586 3.20456 14.2567 3.20456C16.0908 3.20456 16.7579 3.21851 17.2613 3.39587C18.1339 3.70332 18.7996 4.34497 19.1097 5.13033C19.1873 5.32697 19.2422 5.58539 19.2716 6.03334C19.3016 6.49002 19.3021 7.07296 19.3021 7.91322V13.9545C19.3021 15.8266 19.2998 17.1263 19.1604 18.1056C19.0259 19.0505 18.7813 19.5525 18.3964 19.9161Z"
                    fill="#ECB42B"
                  />
                  <g opacity="0.4">
                    <path
                      d="M15.9995 14C16.5518 14 16.9995 13.5523 16.9995 13C16.9995 12.4477 16.5518 12 15.9995 12H12.9995C12.4472 12 11.9995 12.4477 11.9995 13C11.9995 13.5523 12.4472 14 12.9995 14L15.9995 14Z"
                      fill="#ECB42B"
                    />
                    <path
                      d="M15.9995 18C16.5518 18 16.9995 17.5523 16.9995 17C16.9995 16.4477 16.5518 16 15.9995 16L8.99951 16C8.44723 16 7.99951 16.4477 7.99951 17C7.99951 17.5523 8.44723 18 8.99951 18L15.9995 18Z"
                      fill="#ECB42B"
                    />
                  </g>
                </svg>
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
                <h3 class="text-sm text-[#667178]">مقابلة عمل</h3>
              </div>
              <span>
                <svg
                  width="22"
                  height="22"
                  viewBox="0 0 22 22"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M13.001 1C12.6544 1 12.3326 1.17945 12.1503 1.47427C11.9681 1.76909 11.9516 2.13723 12.1066 2.44721L12.9637 4.1615C13.1608 4.55572 13.5832 4.76285 13.9968 4.7049C14.2097 4.67508 14.3829 4.53122 14.5349 4.37922L15.3373 3.57679C16.9226 4.41457 18.001 6.0795 18.001 8C18.001 8.55229 18.4487 9 19.001 9C19.5533 9 20.001 8.55229 20.001 8C20.001 4.13057 16.8704 1 13.001 1Z"
                    fill="#ECB42B"
                  />
                  <path
                    d="M4.00098 14C4.00098 13.4477 3.55326 13 3.00098 13C2.4487 13 2.00098 13.4477 2.00098 14C2.00098 17.8694 5.13155 21 9.00098 21C9.34758 21 9.66938 20.8205 9.85168 20.5257C10.0339 20.2309 10.0504 19.8628 9.89538 19.5528L9.03828 17.8385C8.84116 17.4443 8.4188 17.2372 8.00517 17.2951C7.79229 17.3249 7.61913 17.4688 7.46713 17.6208L6.6647 18.4232C5.07941 17.5854 4.00098 15.9205 4.00098 14Z"
                    fill="#ECB42B"
                  />
                  <path
                    opacity="0.4"
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M5.5 0.25C3.83606 0.25 2.5 1.60022 2.5 3.25C2.5 4.89978 3.83606 6.25 5.5 6.25C7.16395 6.25 8.5 4.89978 8.5 3.25C8.5 1.60022 7.16395 0.25 5.5 0.25ZM8.56853 7.4936C6.68698 6.50213 4.3136 6.50213 2.43204 7.4936C2.38492 7.51843 2.32269 7.54939 2.24941 7.58585C1.89914 7.7601 1.29648 8.05991 0.885386 8.416C0.633456 8.63423 0.318726 8.98195 0.259756 9.4594C0.193716 9.994 0.470366 10.4406 0.865606 10.7738C1.49204 11.302 2.27504 11.75 3.29572 11.75H7.70485C8.72554 11.75 9.50858 11.302 10.135 10.7738C10.5302 10.4406 10.8069 9.994 10.7408 9.4594C10.6819 8.98195 10.3671 8.63423 10.1152 8.416C9.70407 8.05991 9.10148 7.7601 8.75117 7.58585C8.67789 7.54939 8.61565 7.51843 8.56853 7.4936ZM16.501 10.25C14.8371 10.25 13.501 11.6002 13.501 13.25C13.501 14.8998 14.8371 16.25 16.501 16.25C18.1649 16.25 19.501 14.8998 19.501 13.25C19.501 11.6002 18.1649 10.25 16.501 10.25ZM19.5695 17.4936C17.688 16.5021 15.3146 16.5021 13.433 17.4936C13.3859 17.5184 13.3237 17.5494 13.2504 17.5858C12.9001 17.76 12.2975 18.0599 11.8864 18.416C11.6345 18.6342 11.3197 18.982 11.2608 19.4594C11.1947 19.994 11.4714 20.4406 11.8666 20.7738C12.493 21.302 13.276 21.75 14.2967 21.75H18.7059C19.7265 21.75 20.5095 21.302 21.136 20.7738C21.5312 20.4406 21.8079 19.994 21.7418 19.4594C21.6828 18.982 21.3681 18.6342 21.1162 18.416C20.7051 18.0599 20.1024 17.7601 19.7522 17.5858C19.6789 17.5494 19.6167 17.5184 19.5695 17.4936Z"
                    fill="#ECB42B"
                  />
                </svg>
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
                <h3 class="text-sm text-[#667178]">تقدمت للوظائف</h3>
              </div>
              <span>
                <svg
                  width="22"
                  height="22"
                  viewBox="0 0 22 22"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M2.5 9.5C2.08579 9.5 1.75 9.83579 1.75 10.25L1.75 12.5536C1.74999 14.3938 1.74998 15.8471 1.89429 16.9836C2.0422 18.1485 2.35321 19.0937 3.06123 19.8434C3.77496 20.5991 4.68459 20.9366 5.80461 21.0961C6.88619 21.25 8.26578 21.25 9.99654 21.25H12.0034C13.7342 21.25 15.1138 21.25 16.1954 21.0961C17.3154 20.9366 18.225 20.5991 18.9388 19.8434C19.6468 19.0937 19.9578 18.1485 20.1057 16.9836C20.25 15.8471 20.25 14.3938 20.25 12.5536V10.25C20.25 9.83579 19.9142 9.5 19.5 9.5H2.5Z"
                    fill="#ECB42B"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M10.8288 0.500011H11.1683C11.7186 0.499881 12.251 0.499756 12.6993 0.577738C13.2276 0.669646 13.7142 0.879072 14.1249 1.33013C14.5029 1.74536 14.7187 2.25975 14.8873 2.81184C15.0434 3.32327 15.1872 3.96926 15.3581 4.75005H18.6097C19.0059 4.74969 19.3364 4.7494 19.6193 4.82013C20.4284 5.02236 21.0908 5.6558 21.2285 6.50124C21.2761 6.79361 21.2303 7.11991 21.1822 7.46275C21.1619 7.60897 21.1432 7.75665 21.1245 7.90501C21.0376 8.59173 20.9489 9.29289 20.687 9.93061C20.0185 11.558 18.5102 12.7367 16.7104 13.118C16.1859 13.2291 15.5976 13.2468 14.7212 13.2496C14.5212 13.2502 14.4211 13.2506 14.3623 13.1919C14.3035 13.1332 14.3036 13.0286 14.3039 12.8194C14.304 12.7255 14.3028 12.6315 14.3016 12.5377C14.3005 12.4468 14.2994 12.3559 14.2994 12.2654C14.3004 12.0376 14.3018 11.7073 14.2208 11.4049C14.0081 10.6112 13.3882 9.9912 12.5945 9.77852C12.2948 9.69823 11.9833 9.69918 11.6755 9.70012L11.6658 9.70015H10.3325L10.3228 9.70012C10.015 9.69918 9.70354 9.69823 9.40389 9.77852C8.61018 9.9912 7.99022 10.6112 7.77755 11.4049C7.6965 11.7073 7.69796 12.0376 7.69897 12.2654C7.69899 12.3559 7.69786 12.4467 7.69673 12.5375C7.69556 12.6314 7.69439 12.7255 7.69449 12.8194C7.69472 13.0286 7.69483 13.1332 7.63602 13.1919C7.5772 13.2506 7.4772 13.2502 7.27718 13.2496C6.40084 13.2468 5.81261 13.2291 5.28824 13.118C3.48842 12.7367 1.98015 11.558 1.31165 9.93061C1.04967 9.29286 0.960983 8.59168 0.874125 7.90493C0.855364 7.7566 0.836689 7.60894 0.816371 7.46274C0.768266 7.11991 0.722481 6.7936 0.770094 6.50124C0.907777 5.6558 1.57022 5.02236 2.3793 4.82013C2.66225 4.7494 2.99273 4.74969 3.38888 4.75005H6.63895C6.80988 3.96926 6.95367 3.32327 7.10981 2.81184C7.27836 2.25975 7.49414 1.74536 7.8722 1.33013C8.28289 0.879072 8.76952 0.669646 9.29781 0.577738C9.74606 0.499756 10.2785 0.499881 10.8288 0.500011ZM12.9744 3.39582C13.0793 3.73921 13.1818 4.17263 13.3101 4.75005H8.68694C8.81526 4.17263 8.91782 3.73921 9.02265 3.39582C9.16118 2.94205 9.2708 2.76475 9.35104 2.67662C9.39866 2.62432 9.44556 2.58207 9.6406 2.54814C9.8974 2.50347 10.2528 2.50002 10.8962 2.50002H11.1009C11.7443 2.50002 12.0997 2.50347 12.3565 2.54814C12.5515 2.58207 12.5984 2.62432 12.646 2.67662C12.7263 2.76475 12.8359 2.94205 12.9744 3.39582Z"
                    fill="#ECB42B"
                  />
                  <path
                    d="M9.03423 11.7412C9.00015 11.8683 9.00015 12.0233 9.00015 12.3333V13C9.00015 14.1046 9.89558 15 11.0002 15C12.1047 15 13.0002 14.1046 13.0002 13V12.3333C13.0002 12.0233 13.0002 11.8683 12.9661 11.7412C12.8736 11.3961 12.6041 11.1265 12.259 11.0341C12.1318 11 11.9768 11 11.6668 11H10.3335C10.0235 11 9.8685 11 9.74133 11.0341C9.39624 11.1265 9.12669 11.3961 9.03423 11.7412Z"
                    fill="#ECB42B"
                  />
                </svg>
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
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g opacity="0.4">
                    <path
                      d="M11.4381 1.36606C10.5745 1.24994 9.48324 1.24997 8.15991 1.25H8.02194C6.69862 1.24997 5.60735 1.24994 4.74369 1.36606C3.83695 1.48797 3.03615 1.75375 2.39495 2.39495C1.75375 3.03615 1.48797 3.83695 1.36606 4.74369C1.24994 5.60735 1.24997 6.69858 1.25 8.0219L1.25 11.0227C1.25 11.5625 1.68754 12 2.22728 12C2.76701 12 3.20455 11.5625 3.20455 11.0227V8.09091C3.20455 6.68121 3.20662 5.72225 3.30317 5.00413C3.39594 4.31412 3.55951 3.99453 3.77702 3.77702C3.99453 3.55951 4.31412 3.39594 5.00413 3.30317C5.72225 3.20662 6.68121 3.20455 8.09091 3.20455C9.50061 3.20455 10.4596 3.20662 11.1777 3.30317C11.8677 3.39594 12.1873 3.55951 12.4048 3.77702C12.6223 3.99453 12.7859 4.31412 12.8786 5.00413C12.9752 5.72225 12.9773 6.68121 12.9773 8.09091V21.7727C12.9773 22.3125 13.4148 22.75 13.9545 22.75L17.9279 22.75C18.7943 22.75 19.5408 22.7501 20.1385 22.6697C20.7791 22.5836 21.3932 22.3894 21.8913 21.8913C22.3894 21.3932 22.5836 20.7791 22.6697 20.1385C22.7501 19.5408 22.75 18.7943 22.75 17.928V11.9357C22.75 11.0694 22.7501 10.3229 22.6697 9.72511C22.5836 9.0845 22.3894 8.47043 21.8913 7.97235C21.3932 7.47427 20.7791 7.28004 20.1385 7.19391C19.5408 7.11354 18.7943 7.11359 17.928 7.11363L14.9303 7.11364C14.9252 6.19045 14.9042 5.40114 14.8158 4.74369C14.6939 3.83695 14.4281 3.03615 13.7869 2.39495C13.1457 1.75375 12.3449 1.48797 11.4381 1.36606Z"
                      fill="#ECB42B"
                    />
                    <path
                      d="M5.5 5C4.94772 5 4.5 5.44772 4.5 6C4.5 6.55229 4.94772 7 5.5 7H6.5C7.05229 7 7.5 6.55229 7.5 6C7.5 5.44772 7.05229 5 6.5 5H5.5Z"
                      fill="#ECB42B"
                    />
                    <path
                      d="M9.5 5C8.94772 5 8.5 5.44772 8.5 6C8.5 6.55229 8.94772 7 9.5 7H10.5C11.0523 7 11.5 6.55229 11.5 6C11.5 5.44772 11.0523 5 10.5 5H9.5Z"
                      fill="#ECB42B"
                    />
                    <path
                      d="M5.5 8C4.94772 8 4.5 8.44772 4.5 9C4.5 9.55229 4.94772 10 5.5 10H6.5C7.05229 10 7.5 9.55229 7.5 9C7.5 8.44772 7.05229 8 6.5 8H5.5Z"
                      fill="#ECB42B"
                    />
                    <path
                      d="M9.5 8C8.94772 8 8.5 8.44772 8.5 9C8.5 9.55229 8.94772 10 9.5 10H10.5C11.0523 10 11.5 9.55229 11.5 9C11.5 8.44772 11.0523 8 10.5 8H9.5Z"
                      fill="#ECB42B"
                    />
                  </g>
                  <path
                    d="M17.5 10.25C17.0858 10.25 16.75 10.5858 16.75 11C16.75 11.4142 17.0858 11.75 17.5 11.75H18.5C18.9142 11.75 19.25 11.4142 19.25 11C19.25 10.5858 18.9142 10.25 18.5 10.25H17.5Z"
                    fill="#ECB42B"
                  />
                  <path
                    d="M17.5 14.25C17.0858 14.25 16.75 14.5858 16.75 15C16.75 15.4142 17.0858 15.75 17.5 15.75H18.5C18.9142 15.75 19.25 15.4142 19.25 15C19.25 14.5858 18.9142 14.25 18.5 14.25H17.5Z"
                    fill="#ECB42B"
                  />
                  <path
                    d="M6.0001 11.25C4.48132 11.25 3.2501 12.4812 3.2501 14C3.2501 15.5188 4.48132 16.75 6.0001 16.75C7.51888 16.75 8.7501 15.5188 8.7501 14C8.7501 12.4812 7.51888 11.25 6.0001 11.25Z"
                    fill="#ECB42B"
                  />
                  <path
                    d="M10.2143 19.1697C9.84353 18.7614 9.13753 18.3137 8.82747 18.1177C7.10042 16.9608 4.89978 16.9608 3.17273 18.1177C2.86266 18.3137 2.15667 18.7614 1.78592 19.1697C1.55059 19.4288 1.30253 19.7967 1.25726 20.2626C1.20867 20.7627 1.40665 21.2221 1.76846 21.6099C2.31558 22.1963 3.04719 22.75 4.04048 22.75H7.95971C8.95301 22.75 9.68462 22.1963 10.2317 21.6099C10.5935 21.2221 10.7915 20.7627 10.7429 20.2626C10.6977 19.7967 10.4496 19.4288 10.2143 19.1697Z"
                    fill="#ECB42B"
                  />
                </svg>
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
              <span class="block text-[#696C68] mb-2">{{ i.key }}</span>
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
            class="bg-[#F8F9F9] rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-y-8 gap-x-4 text-right items-start"
            >
              <div class="text-xs space-y-2">
                <p class="font-bold text-[#161614]">المؤهل</p>
                <p class="text-[#696C68]">{{ qual.qualification }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-[#161614]">التخصص</p>
                <p class="text-[#696C68]">{{ qual.specialization || "-" }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-[#161614]">المؤسسة التعليمية</p>
                <p class="text-[#696C68]">{{ qual.educational_institution }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-[#161614]">سنة التخرج</p>
                <p class="text-[#696C68]">{{ qual.graduation_year }}</p>
              </div>
              <div class="text-xs space-y-2">
                <p class="font-bold text-[#161614]">التقدير</p>
                <p class="text-[#696C68]">{{ qual.grade }}</p>
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
            class="bg-[#F8F9F9] rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-6 lg:grid-cols-5 gap-y-8 gap-x-4 lg:gap-x-6 text-right items-start"
            >
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">المسمى الوظيفى</p>
                <p class="text-[#696C68]">{{ expert.job_title }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">الجهة</p>
                <p class="text-[#696C68]">{{ expert.authority || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">الفترة</p>
                <p class="text-[#696C68]">{{ expert.duration }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">الموقع</p>
                <p class="text-[#696C68]">{{ expert.location }}</p>
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
            class="bg-[#F8F9F9] rounded-2xl p-4 shadow-sm"
          >
            <div
              class="grid grid-cols-6 lg:grid-cols-5 gap-y-8 gap-x-4 lg:gap-x-6 text-right items-start"
            >
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">رقم الأيبان</p>
                <p class="text-[#696C68]">{{ item.iban }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">اسم البنك</p>
                <p class="text-[#696C68]">{{ item.bank_name || "-" }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">
                  حالة إرتباط الأيبان بالمستخدم
                </p>
                <p class="text-[#696C68]">{{ item.iban_status }}</p>
              </div>
              <div
                class="text-xs space-y-2 col-span-3 sm:col-span-2 lg:col-span-1"
              >
                <p class="font-bold text-[#161614]">حالة الحساب</p>
                <p class="text-[#696C68]">{{ item.account_status }}</p>
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
            class="flex items-center justify-between p-4 bg-[#f5f5f5] rounded-lg hover:border-primary transition-colors"
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
