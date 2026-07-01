<script setup>
import ArrowTabs from "~/components/elements/ArrowTabs.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";
import Avatar1 from "~/assets/images/avatar-1.png";
import Avatar2 from "~/assets/images/avatar-2.png";
import JobShortlist from "~/components/JobShortlist.vue";
import JobInterviews from "~/components/JobInterviews.vue";
import ApplicantCard from "~/components/ApplicantCard.vue";
import FilterDrawer from "~/components/FilterDrawer.vue";

const loading = ref(false);
const error = ref(null);

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

const applicantsLoading = ref(false);

onMounted(async () => {
  applicantsLoading.value = true;
  try {
    await useApi().get('/jobs/1/applications');
  } finally {
    applicantsLoading.value = false;
  }
});
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
          <LoadingSkeleton v-if="applicantsLoading" :rows="3" :columns="2" height="160px" />
          <template v-else>
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
                        <Search />
                      </span>
                    </div>

                    <div class="flex flex-wrap gap-2 items-center">
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                      >
                        <InboxIcon />
                      </button>
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                      >
                        <OutboxIcon />
                      </button>
                      <button
                        class="p-2 rounded-xl border bg-[#fff] hover:border-[#ecb42b] transition"
                        @click="openFilterDrawer()"
                      >
                        <FilterIcon />
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
                        <GridIcon />
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
                        <ListIcon />
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
