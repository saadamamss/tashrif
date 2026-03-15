<script setup>
import ArrowTabs from "~/components/elements/arrow-tabs.vue";
import Breadcrumbs from "~/components/elements/breadcrumbs.vue";
import CustomSelect from "~/components/elements/custom-select.vue";
import CustomTabs from "~/components/elements/custom-tabs.vue";
import Avatar1 from "~/assets/images/avatar-1.png";
import Avatar2 from "~/assets/images/avatar-2.png";
import JobShortlist from "~/components/job-shortlist.vue";
import JobInterviews from "~/components/job-interviews.vue";
import ApplicantCard from "~/components/applicant-card.vue";
import FilterDrawer from "~/components/filter-drawer.vue";
definePageMeta({
  layout: "dashboard",
  middleware: ["auth-global", "auth-guard", "entity"],
});

const breadcrumbs = [
  {
    label: "وظائفى المنشورة",
    to: "/dashboard/published-jobs",
    active: true,
  },

  {
    label: "مشرف حجاج",
    active: false,
  },
];

const applicants = [
  {
    id: 1,
    name: "محمد أحمد آل سعود",
    jobTitle: "مهندس برمجيات",
    qualification: "بكالوريوس في علوم الحاسب",
    applyDate: "2023-10-15",
    gender: "ذكر",
    avatar: Avatar1,
    city: "الرياض",
  },
  {
    id: 2,
    name: "سارة عبدالله الغامدي",
    jobTitle: "طبيبة أسنان",
    qualification: "دكتوراه في طب الأسنان",
    applyDate: "2023-11-02",
    gender: "أنثى",
    avatar: Avatar2,
    city: "جدة",
  },
  {
    id: 3,
    name: "خالد إبراهيم الحارثي",
    jobTitle: "محاسب قانوني",
    qualification: "ماجستير في المحاسبة",
    applyDate: "2023-09-28",
    gender: "ذكر",
    avatar: Avatar1,
    city: "الدمام",
  },
  {
    id: 4,
    name: "نورة سعد القحطاني",
    jobTitle: "معلمة لغة عربية",
    qualification: "بكالوريوس في اللغة العربية",
    applyDate: "2023-12-05",
    gender: "أنثى",
    avatar: Avatar2,
    city: "مكة المكرمة",
  },
  {
    id: 5,
    name: "فيصل ناصر العتيبي",
    jobTitle: "مدير مشاريع",
    qualification: "ماجستير في إدارة الأعمال",
    applyDate: "2023-08-17",
    gender: "ذكر",
    avatar: Avatar1,
    city: "الخبر",
  },
  {
    id: 6,
    name: "لطيفة عمر الزهراني",
    jobTitle: "ممرضة مسجلة",
    qualification: "دبلوم تمريض",
    applyDate: "2023-07-22",
    gender: "أنثى",
    avatar: Avatar2,
    city: "الطائف",
  },
  {
    id: 7,
    name: "عبدالرحمن خالد السبيعي",
    jobTitle: "مهندس مدني",
    qualification: "بكالوريوس في الهندسة المدنية",
    applyDate: "2023-11-30",
    gender: "ذكر",
    avatar: Avatar1,
    city: "بريدة",
  },
  {
    id: 8,
    name: "أمل حسين الحربي",
    jobTitle: "أخصائية موارد بشرية",
    qualification: "بكالوريوس في إدارة الموارد البشرية",
    applyDate: "2023-10-08",
    gender: "أنثى",
    avatar: Avatar2,
    city: "تبوك",
  },
  {
    id: 9,
    name: "تركي فهد الشمري",
    jobTitle: "مطور ويب",
    qualification: "بكالوريوس في تقنية المعلومات",
    applyDate: "2023-09-14",
    gender: "ذكر",
    avatar: Avatar1,
    city: "حائل",
  },
  {
    id: 10,
    name: "هناء علي القرشي",
    jobTitle: "صيدلانية",
    qualification: "دكتوراه في الصيدلة",
    applyDate: "2023-12-18",
    gender: "أنثى",
    avatar: Avatar2,
    city: "نجران",
  },
  {
    id: 11,
    name: "بدر سلمان الغامدي",
    jobTitle: "مدير تسويق",
    qualification: "ماجستير في التسويق الرقمي",
    applyDate: "2023-07-05",
    gender: "ذكر",
    avatar: Avatar1,
    city: "الجبيل",
  },
  {
    id: 12,
    name: "شهد محمد الثبيتي",
    jobTitle: "محامية",
    qualification: "بكالوريوس في القانون",
    applyDate: "2023-08-29",
    gender: "أنثى",
    avatar: Avatar2,
    city: "أبها",
  },
];
const selectedApplicants = ref([]);
const filter = ref({
  search: "",
  dateOrder: "الأحدث",
});
const displayMethod = ref("card");
const handleDisplayMethod = (method) => {
  displayMethod.value = method;
};

const shorList = ref(applicants.slice(0, 2));
const interviewList = ref(applicants.slice(0, 2));
const applicantsRecievedContract = ref(applicants.slice(0, 2));
const applicantsAcceptContract = ref(applicants.slice(0, 2));
const applicantsRefuseContract = ref(applicants.slice(0, 2));

//
const setIntoShortList = () => {
  selectedApplicants.value.forEach((item) => {
    const isFound = shorList.value.find((i) => i.id == item.id);
    if (isFound) return;
    shorList.value.push(item);
  });
  selectedApplicants.value = [];
};

const filterDrawer = ref(false);
const openFilterDrawer = () => {
  filterDrawer.value = true;
};
</script>
<template>
  <div class="px-4 lg:px-0">
    <Breadcrumbs :items="breadcrumbs" />

    <div class="mt-3">
      <!--  -->
      <CustomTabs
        :tabs="[
          { id: 'about', title: 'عن الوظيفة' },
          { id: 'applicants', title: 'المتقدمون' },
        ]"
        initial-tab="applicants"
      >
        <template #about>
          <div>
            <!--  -->
            <div class="bg-white rounded-2xl shadow-sm p-4 sm:p-6 md:p-8 mb-4">
              <div class="flex flex-col gap-6">
                <h1
                  class="job-title text-lg lg:text-xl font-bold text-[#161614]"
                >
                  مشرف حجاج
                </h1>
                <p class="job-desc text-sm text-[#161614]/70 leading-[2]">
                  تبحث شركة الإسناد الموسمي لخدمات الحجاج عن أفراد مؤهلين
                  للانضمام إلى فريقها كمشرفين ميدانيين خلال موسم الحج. ستكون
                  مسؤولاً عن تنظيم وإرشاد مجموعة من الحجاج أثناء تنقلهم بين
                  المشاعر المقدسة، وضمان التزامهم بالتعليمات والخطط التشغيلية.
                </p>

                <div class="flex items-center gap-2">
                  <span
                    class="company-logo border rounded-md overflow-hidden py-1 px-2"
                  >
                    <img
                      src="/images/partner-3.svg"
                      class="w-10 h-6 object-cover"
                    />
                  </span>
                  <span class="company-name text-sm text-[#161614]">
                    شركة نسك لخدمات الحجاج
                  </span>
                </div>
              </div>
            </div>
            <!--  -->
            <div class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                مميزات خاصة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-[#667178] mb-4">
                  الإشراف اليومي على مجموعة محددة من الحجاج.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  التأكد من التزام الحجاج بخطط التنقل وجدول الحركة بين المشاعر.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  التنسيق المستمر مع فرق النقل والدعم اللوجستي.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  التعامل مع الحالات الطارئة ورفع التقارير إلى المسؤول المباشر.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  ضمان سلامة وراحة الحجاج خلال تنقلهم وإقامتهم.
                </li>
              </ul>
            </div>
            <!--  -->
            <div class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                شروط القبول
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-[#667178] mb-4">
                  أن يكون المتقدم سعودي الجنسية.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  العمر بين 22 و45 سنة.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  القدرة على العمل الميداني المكثف لساعات طويلة.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  يفضّل من لديه خبرة سابقة في العمل الموسمي أو الإشراف الميداني.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  الالتزام بالأخلاقيات المهنية والسلوكيات المناسبة.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  الأولوية لسكان منطقة مكة المكرمة لتسهيل التنقل السريع.
                </li>
              </ul>
            </div>
            <!--  -->
            <div class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3
                class="text-base font-semibold text-primary pb-5 border-b mb-4"
              >
                المزايا والمكافأة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li class="text-sm text-[#667178] mb-4">
                  مكافأة مقطوعة قدرها 3000 ريال سعودي.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  شهادة خبرة بعد انتهاء المهمة بنجاح.
                </li>
                <li class="text-sm text-[#667178] mb-4">
                  تشمل بدل السكن والتنقل.
                </li>
              </ul>
            </div>
            <!--  -->
          </div>
        </template>
        <!--  -->
        <template #applicants>
          <div :class="displayMethod">
            <ArrowTabs
              :tabs="[
                { id: 'new', title: 'جديد', number: '12' },
                { id: 'shortlist', title: 'المرشحين', number: '3' },
                { id: 'interviews', title: 'المقابلات', number: '3' },
                { id: 'contract', title: 'القعد', number: '2' },
                { id: 'accepted', title: 'وافق', number: '2' },
                { id: 'refused', title: 'رفض', number: '0' },
              ]"
              initial-tab="new"
            >
              <template #filter>
                <div class="filters">
                  <div class="flex flex-wrap items-center gap-2">
                    <div
                      class="flex-1 min-w-[250px] flex search relative items-center"
                    >
                      <input
                        class="w-full ps-10 h-[48px] border bg-[#f5f5f5] py-2 text-sm rounded-xl focus:outline-none focus:border-[#ecb42b] transition"
                        type="text"
                        v-model="filter.search"
                        @change="handleSearch"
                      />
                      <span
                        class="flex items-center justify-center bg-white shadow-sm block h-[28px] w-[28px] rounded-lg absolute right-[10px]"
                      >
                        <svg
                          width="16"
                          height="16"
                          viewBox="0 0 16 16"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            fill-rule="evenodd"
                            clip-rule="evenodd"
                            d="M5.99992 0.833008C3.14645 0.833008 0.833252 3.1462 0.833252 5.99967C0.833252 8.85315 3.14645 11.1663 5.99992 11.1663C7.24658 11.1663 8.39012 10.7248 9.28268 9.98959L10.3093 11.0162C10.048 11.5678 10.1455 12.247 10.6019 12.7033L12.6296 14.7311C13.21 15.3114 14.1509 15.3114 14.7313 14.7311C15.3117 14.1507 15.3117 13.2097 14.7313 12.6293L12.7036 10.6016C12.2473 10.1453 11.568 10.0478 11.0163 10.309L9.9898 9.28249C10.725 8.38991 11.1666 7.24636 11.1666 5.99967C11.1666 3.1462 8.85339 0.833008 5.99992 0.833008ZM1.83325 5.99967C1.83325 3.69849 3.69873 1.83301 5.99992 1.83301C8.30111 1.83301 10.1666 3.69849 10.1666 5.99967C10.1666 8.30086 8.30111 10.1663 5.99992 10.1663C3.69873 10.1663 1.83325 8.30086 1.83325 5.99967ZM11.9965 11.3087C11.8066 11.1189 11.4988 11.1189 11.309 11.3087C11.1191 11.4986 11.1191 11.8064 11.309 11.9962L13.3367 14.024C13.5265 14.2138 13.8343 14.2138 14.0242 14.024C14.214 13.8341 14.214 13.5263 14.0242 13.3364L11.9965 11.3087Z"
                            fill="#9DA4AE"
                          />
                        </svg>
                      </span>
                    </div>

                    <div class="flex flex-wrap gap-2 items-center">
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                      >
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
                            d="M11.001 22.7344H10.728C7.31309 22.7344 5.59799 22.7344 4.27101 21.7964C3.89101 21.5264 3.553 21.2104 3.265 20.8554C2.25 19.5744 2.25 17.9504 2.25 14.7134V12.1634C2.25 12.1043 2.25 12.0457 2.25 11.9878L2.25 11.9844C2.25 11.9824 2.25 11.9803 2.25 11.9783C2.25022 9.04992 2.26065 7.55437 2.77101 6.2584C3.60601 4.1534 5.362 2.49843 7.589 1.71543C8.968 1.23343 10.589 1.2334 13.818 1.2334C15.6989 1.2334 16.644 1.2334 17.482 1.52539C18.844 2.00839 19.917 3.02137 20.429 4.30537C20.75 5.12036 20.75 6.0174 20.75 7.80336V12.9834C20.75 13.3974 20.414 13.7334 20 13.7334C19.586 13.7334 19.25 13.3974 19.25 12.9834V7.80342C19.25 6.20756 19.25 5.40637 19.034 4.8584C18.683 3.9784 17.935 3.27743 16.984 2.94043C16.39 2.73343 15.53 2.7334 13.818 2.7334H13.8168C13.5435 2.7334 13.2823 2.7334 13.0325 2.73368C13.0217 2.73414 13.0109 2.73438 13 2.73438C11.576 2.73438 10.417 3.89136 10.417 5.31436C10.417 5.47573 10.425 5.64962 10.4335 5.83342L10.4353 5.87265C10.4602 6.41612 10.4884 7.03063 10.335 7.60742C10.112 8.44642 9.45501 9.10341 8.62001 9.32041C8.04401 9.47141 7.423 9.44441 6.875 9.42041L6.84328 9.41909C6.66294 9.41155 6.49173 9.4044 6.33301 9.4044C4.91097 9.4044 3.75319 10.5582 3.75 11.9785C3.75 12.0395 3.75 12.1011 3.75 12.1634V14.7134C3.75 17.6024 3.75 19.0524 4.436 19.9174C4.632 20.1594 4.87 20.3814 5.137 20.5714C6.074 21.2334 7.629 21.2334 10.727 21.2334H11C11.414 21.2334 11.75 21.5694 11.75 21.9834C11.75 22.3974 11.414 22.7334 11 22.7334L11.001 22.7344ZM3.80363 8.78371C4.49967 8.23331 5.37868 7.90439 6.33301 7.90439C6.53401 7.90439 6.74 7.91336 6.94 7.92236L6.94478 7.92257C7.4063 7.94251 7.88323 7.96311 8.24 7.86943C8.554 7.78743 8.8 7.54036 8.885 7.22236C8.98 6.86736 8.958 6.39343 8.936 5.93643C8.926 5.73243 8.917 5.52036 8.917 5.31436C8.917 4.37298 9.23769 3.50514 9.77558 2.81407C9.0919 2.87073 8.555 2.96717 8.086 3.13135C6.27 3.76935 4.841 5.11035 4.166 6.81035C3.96016 7.33289 3.85617 7.94847 3.80363 8.78371Z"
                            fill="#696C68"
                          />
                          <path
                            d="M15.501 21.7337C15.313 21.7337 15.125 21.6638 14.979 21.5228C14.872 21.4188 14.633 21.2317 14.38 21.0337C13.133 20.0557 12.251 19.2998 12.25 18.4858V18.4837C12.25 17.6697 13.133 16.9138 14.38 15.9358C14.633 15.7378 14.872 15.5497 14.979 15.4467C15.277 15.1587 15.751 15.1657 16.04 15.4637C16.328 15.7607 16.321 16.2357 16.023 16.5237C15.86 16.6817 15.616 16.8737 15.307 17.1157L15.3015 17.1201C15.1143 17.2672 14.8232 17.496 14.541 17.7347H21.001C21.415 17.7347 21.751 18.0707 21.751 18.4847C21.751 18.8987 21.415 19.2347 21.001 19.2347H14.541C14.826 19.4757 15.12 19.7077 15.307 19.8537C15.616 20.0957 15.86 20.2877 16.023 20.4457C16.321 20.7337 16.328 21.2087 16.04 21.5057C15.893 21.6577 15.697 21.7337 15.501 21.7337Z"
                            fill="#696C68"
                          />
                        </svg>
                      </button>
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                      >
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
                            d="M10.998 22.75H10.7261C7.31105 22.75 5.59606 22.75 4.26906 21.813C3.88906 21.544 3.55105 21.227 3.26305 20.872C2.24805 19.591 2.24805 17.967 2.24805 14.73V12.18C2.24805 9.12898 2.24806 7.598 2.76906 6.275C3.60406 4.17 5.36005 2.51493 7.58705 1.73193C8.83564 1.29552 10.2826 1.2543 12.942 1.25041C12.9613 1.25014 12.9806 1.25 13 1.25C13.0072 1.25 13.0143 1.2501 13.0214 1.2503C13.2747 1.25 13.5388 1.25 13.8147 1.25H13.8161C15.697 1.25 16.6421 1.25 17.4801 1.54199C18.8421 2.02499 19.9151 3.03797 20.4271 4.32197C20.748 5.13796 20.748 6.0339 20.748 7.81986V13C20.748 13.414 20.412 13.75 19.998 13.75C19.584 13.75 19.248 13.414 19.248 13V7.81992C19.248 6.22406 19.248 5.42198 19.0321 4.875C18.6811 3.995 17.9331 3.29393 16.9821 2.95693C16.3882 2.75 15.5286 2.75 13.8177 2.75H13.8149C13.514 2.75 13.2279 2.75 12.9554 2.75038C11.5519 2.77424 10.417 3.92187 10.417 5.32998C10.417 5.49128 10.425 5.66499 10.4335 5.84869C10.434 5.85973 10.4345 5.87082 10.435 5.88193L10.4353 5.88818C10.4602 6.43165 10.4884 7.04616 10.335 7.62295C10.112 8.46195 9.45501 9.11894 8.62001 9.33594C8.04681 9.4862 7.42905 9.46027 6.88301 9.43735L6.84328 9.43569C6.66294 9.42815 6.49173 9.421 6.33301 9.421C4.90901 9.421 3.75 10.578 3.75 12.001C3.75 12.0193 3.74934 12.0375 3.74805 12.0555C3.74805 12.0967 3.74805 12.1382 3.74805 12.18V14.73C3.74805 17.619 3.74805 19.069 4.43405 19.934C4.63005 20.176 4.86805 20.398 5.13505 20.588C6.07205 21.25 7.62705 21.25 10.725 21.25H10.998C11.412 21.25 11.748 21.586 11.748 22C11.748 22.414 11.412 22.75 10.998 22.75ZM3.80165 8.80085C4.49799 8.24948 5.37777 7.91992 6.33301 7.91992C6.53401 7.91992 6.74 7.92899 6.94 7.93799L6.94356 7.93814C7.40545 7.9581 7.88292 7.97872 8.24 7.88496C8.554 7.80296 8.8 7.55599 8.885 7.23799C8.98 6.88299 8.958 6.40995 8.936 5.95195C8.926 5.74795 8.917 5.53598 8.917 5.32998C8.917 4.389 9.23742 3.52149 9.7749 2.83057C9.09062 2.88721 8.55334 2.98367 8.08405 3.14795C6.26805 3.78595 4.83905 5.12695 4.16405 6.82695C3.95816 7.3496 3.85417 7.96534 3.80165 8.80085Z"
                            fill="#696C68"
                          />
                          <path
                            d="M18.5 21.75C18.304 21.75 18.108 21.674 17.961 21.522C17.673 21.224 17.68 20.75 17.978 20.461C18.141 20.303 18.385 20.111 18.694 19.869L18.6991 19.8649C18.8864 19.7178 19.1776 19.4888 19.46 19.25H13C12.586 19.25 12.25 18.914 12.25 18.5C12.25 18.086 12.586 17.75 13 17.75H19.46C19.175 17.509 18.881 17.277 18.694 17.131C18.385 16.889 18.141 16.697 17.978 16.539C17.68 16.251 17.673 15.776 17.961 15.478C18.249 15.181 18.724 15.173 19.022 15.461C19.1243 15.5604 19.3472 15.7357 19.5876 15.9247L19.621 15.951C20.868 16.929 21.75 17.685 21.751 18.5C21.751 19.316 20.868 20.072 19.621 21.05C19.368 21.248 19.129 21.436 19.022 21.54C18.876 21.681 18.688 21.75 18.5 21.75Z"
                            fill="#696C68"
                          />
                        </svg>
                      </button>
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                        @click="openFilterDrawer()"
                      >
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
                            d="M6.18213 2.25C6.19946 2.25 6.21686 2.25 6.23431 2.25L17.8178 2.25C18.5884 2.24997 19.2426 2.24994 19.7587 2.3203C20.3049 2.39478 20.8273 2.56291 21.2272 3.0031C21.6306 3.44714 21.741 3.98226 21.7494 4.52927C21.7573 5.04013 21.6757 5.67472 21.5805 6.41461L21.5735 6.4685C21.5399 6.73033 21.4893 6.98356 21.384 7.23711C21.2771 7.49453 21.1298 7.71377 20.9464 7.93165C19.9667 9.09554 18.1454 11.1989 15.5814 13.1144C15.54 13.1454 15.4876 13.2188 15.4776 13.329C15.2285 16.0819 15.0094 17.5385 14.8513 18.3823C14.6803 19.2948 13.9843 19.9264 13.3841 20.3612C13.0701 20.5888 12.7367 20.7937 12.4398 20.9744C12.416 20.9889 12.3925 21.0032 12.3693 21.0173C12.0919 21.186 11.8561 21.3294 11.659 21.4687C11.1185 21.8509 10.4949 21.8241 10.0183 21.5464C9.56847 21.2844 9.25196 20.8065 9.18796 20.266C9.0475 19.0796 8.79286 16.7569 8.51176 13.3226C8.50274 13.2124 8.48553 13.1805 8.4841 13.1778C8.48311 13.1759 8.48058 13.1713 8.47219 13.1621C8.4628 13.1517 8.44403 13.1335 8.40835 13.1068C5.84952 11.1937 4.03182 9.094 3.05351 7.93162C2.8708 7.71453 2.71921 7.50051 2.61112 7.24022C2.50531 6.98541 2.46019 6.73089 2.42644 6.46849C2.42412 6.45047 2.42181 6.43251 2.41951 6.4146C2.32429 5.67472 2.24262 5.04013 2.25053 4.52927C2.259 3.98226 2.36942 3.44714 2.7728 3.0031C3.17269 2.56291 3.69502 2.39478 4.2413 2.3203C4.75741 2.24994 5.41157 2.24997 6.18213 2.25ZM4.44394 3.80655C4.05984 3.85892 3.94414 3.94449 3.88307 4.01171C3.82551 4.07507 3.75601 4.18665 3.75035 4.55249C3.74432 4.94212 3.81012 5.46812 3.91418 6.27712C3.9431 6.50193 3.96843 6.59754 3.99643 6.66496C4.02215 6.72691 4.06773 6.80722 4.20114 6.96572C5.15942 8.10431 6.88846 10.0976 9.30654 11.9054C9.50081 12.0507 9.67905 12.2305 9.808 12.4726C9.93505 12.7112 9.98704 12.9594 10.0068 13.2002C10.2863 16.6158 10.5391 18.9204 10.6776 20.0896C10.6818 20.1253 10.6948 20.1604 10.7147 20.1911C10.7352 20.2224 10.7579 20.2413 10.7733 20.2503C10.7753 20.2515 10.777 20.2524 10.7785 20.2531C10.7821 20.2512 10.7869 20.2483 10.793 20.244C11.0351 20.0728 11.3161 19.902 11.5813 19.7409C11.6077 19.7248 11.634 19.7089 11.66 19.693C11.9588 19.5112 12.2446 19.3345 12.504 19.1466C13.0508 18.7503 13.319 18.4151 13.3769 18.1061C13.5234 17.3243 13.7372 15.9181 13.9837 13.1938C14.0289 12.6947 14.274 12.2188 14.6837 11.9127C17.1067 10.1025 18.8392 8.10571 19.7988 6.9657C19.9152 6.82745 19.9669 6.73831 19.9988 6.6616C20.0323 6.58102 20.0604 6.4744 20.0858 6.27712C20.1899 5.46812 20.2557 4.94212 20.2496 4.55249C20.244 4.18665 20.1745 4.07507 20.1169 4.01171C20.0558 3.94449 19.9401 3.85892 19.556 3.80655C19.1534 3.75165 18.6027 3.75 17.7657 3.75H6.23431C5.39726 3.75 4.8466 3.75165 4.44394 3.80655ZM10.7707 20.2566C10.7707 20.2566 10.7712 20.2564 10.7721 20.2562L10.7707 20.2566Z"
                            fill="#696C68"
                          />
                        </svg>
                      </button>
                      <div>
                        <CustomSelect
                          :items="['الأحدث', 'الأقدم']"
                          v-model="filter.dateOrder"
                          class="select-style"
                        />
                      </div>
                      <button
                        class="hidden sm:block p-2 rounded-xl border bg-[#f8f9f9] hover:border-[#ecb42b] transition"
                        :class="
                          displayMethod == 'card'
                            ? 'text-[#ecb42b]'
                            : 'text-[#667178]'
                        "
                        @click="handleDisplayMethod('card')"
                      >
                        <svg
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            d="M22 8.52V3.98C22 2.57 21.36 2 19.77 2H15.73C14.14 2 13.5 2.57 13.5 3.98V8.51C13.5 9.93 14.14 10.49 15.73 10.49H19.77C21.36 10.5 22 9.93 22 8.52Z"
                            fill="currentColor"
                          />
                          <path
                            d="M22 19.77V15.73C22 14.14 21.36 13.5 19.77 13.5H15.73C14.14 13.5 13.5 14.14 13.5 15.73V19.77C13.5 21.36 14.14 22 15.73 22H19.77C21.36 22 22 21.36 22 19.77Z"
                            fill="currentColor"
                          />
                          <path
                            d="M10.5 8.52V3.98C10.5 2.57 9.86 2 8.27 2H4.23C2.64 2 2 2.57 2 3.98V8.51C2 9.93 2.64 10.49 4.23 10.49H8.27C9.86 10.5 10.5 9.93 10.5 8.52Z"
                            fill="currentColor"
                          />
                          <path
                            d="M10.5 19.77V15.73C10.5 14.14 9.86 13.5 8.27 13.5H4.23C2.64 13.5 2 14.14 2 15.73V19.77C2 21.36 2.64 22 4.23 22H8.27C9.86 22 10.5 21.36 10.5 19.77Z"
                            fill="currentColor"
                          />
                        </svg>
                      </button>
                      <button
                        class="hidden sm:block p-2 rounded-xl border bg-[#f8f9f9] hover:border-[#ecb42b] transition"
                        :class="
                          displayMethod == 'list'
                            ? 'text-[#ecb42b]'
                            : 'text-[#667178]'
                        "
                        @click="handleDisplayMethod('list')"
                      >
                        <svg
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            d="M19.9 13.5H4.1C2.6 13.5 2 14.14 2 15.73V19.77C2 21.36 2.6 22 4.1 22H19.9C21.4 22 22 21.36 22 19.77V15.73C22 14.14 21.4 13.5 19.9 13.5Z"
                            fill="currentColor"
                          />
                          <path
                            d="M19.9 2H4.1C2.6 2 2 2.64 2 4.23V8.27C2 9.86 2.6 10.5 4.1 10.5H19.9C21.4 10.5 22 9.86 22 8.27V4.23C22 2.64 21.4 2 19.9 2Z"
                            fill="currentColor"
                          />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
              </template>

              <template #new>
                <div v-if="selectedApplicants.length">
                  <div class="flex flex-col sm:flex-row justify-between sm:items-center gap-6 mb-4">
                    <h2 class="text-sm lg:text-lg font-medium text-[#667178]">
                      تم تحديد {{ selectedApplicants.length }} متقدمين
                    </h2>
                    <div class="self-end flex gap-2">
                      <button class="btn-outline text-sm">
                        فحص عن طريق ATS
                      </button>
                      <button
                        class="btn-primary text-sm"
                        @click="setIntoShortList"
                      >
                        نقل إلى قائمة مختصرة
                      </button>
                    </div>
                  </div>
                </div>
                <div class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="(applicant, i) in applicants"
                    :key="i"
                    :applicant="applicant"
                    :action="true"
                    :select="true"
                    badge-text="جديد"
                    badge-style="bg-[#ecb42b]/10 text-[#ecb42b]"
                    card-style=" bg-[#fff]"
                    v-model="selectedApplicants"
                  />
                </div>
              </template>
              <template #shortlist>
                <JobShortlist
                  :shor-list="shorList"
                  :display-method="displayMethod"
                />
              </template>
              <template #interviews>
                <JobInterviews
                  :interview-list="interviewList"
                  :display-method="displayMethod"
                />
              </template>
              <template #contract>
                <div class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="(applicant, i) in applicantsRecievedContract"
                    :key="i"
                    :applicant="applicant"
                    :action="true"
                    badge-text="تم إرسال العقد"
                    badge-style="bg-[#35685F]/10 text-[#35685F]"
                    card-style=" bg-[#fff]"
                    v-model="selectedApplicants"
                  />
                </div>
              </template>
              <template #accepted>
                <div class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="(applicant, i) in applicantsAcceptContract"
                    :key="i"
                    :applicant="applicant"
                    :action="true"
                    badge-text="تم قبول العقد"
                    badge-style="bg-[#1E874C]/10 text-[#1E874C]"
                    card-style=" bg-[#fff]"
                    v-model="selectedApplicants"
                  />
                </div>
              </template>
              <template #refused>
                <div class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="(applicant, i) in applicantsRefuseContract"
                    :key="i"
                    :applicant="applicant"
                    :action="true"
                    badge-text="تم رفض العقد"
                    badge-style="bg-[#F53D6B]/10 text-[#F53D6B]"
                    card-style=" bg-[#fff]"
                    v-model="selectedApplicants"
                  />
                </div>
              </template>
            </ArrowTabs>
          </div>
        </template>
      </CustomTabs>
    </div>

    <!--  -->
    <FilterDrawer v-model="filterDrawer" />
  </div>
</template>
<style lang="scss" scoped>
.transition-height {
  transition: height 0.3s ease;
}

.select-style {
  background-color: rgb(255, 255, 255);
  border: 1px solid #f5f5f5;
  box-shadow: 0px 1px 2px rgba(18, 18, 23, 0.05);
  border-radius: 16px;
  min-width: 140px;
  height: 48px;
  width: 100%;
}
</style>
