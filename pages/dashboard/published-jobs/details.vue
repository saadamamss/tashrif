<script setup>
import ArrowTabs from "~/components/elements/ArrowTabs.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";

import JobShortlist from "~/components/JobShortlist.vue";
import JobInterviews from "~/components/JobInterviews.vue";
import ApplicantCard from "~/components/ApplicantCard.vue";
import FilterDrawer from "~/components/FilterDrawer.vue";
import SendInterview from "~/components/SendInterview.vue";
import SendContract from "~/components/SendContract.vue";

const error = ref(null);
const applicantsLoading = ref(false);
const route = useRoute();

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "entity"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'المتقدمون للوظيفة',
})

const breadcrumbs = computed(() => [
  {
    label: "وظائفى المنشورة",
    to: "/dashboard/published-jobs",
    active: true,
  },
  {
    label: jobDetails.value?.title || 'الوظيفة',
    active: false,
  },
]);

const applicants = ref([])
const jobDetails = ref(null)
const selectedApplicants = ref([]);
const filter = ref({
  search: "",
  dateOrder: "الأحدث",
});
const displayMethod = ref("card");
const handleDisplayMethod = (method) => {
  displayMethod.value = method;
};

const activeTab = ref('new')

const filteredApplicants = computed(() => {
  const statusMap = {
    new: 'new',
    shortlist: 'shortlisted',
    interviews: 'interview',
    contract: 'contract_sent',
    accepted: 'accepted',
    refused: 'refused',
  }
  const status = statusMap[activeTab.value]
  return status ? applicants.value.filter(a => a.status === status) : applicants.value
})

const tabs = computed(() => [
  { id: 'new', title: 'جديد', number: applicants.value.filter(a => a.status === 'new' || !a.status).length.toString() },
  { id: 'shortlist', title: 'المرشحين', number: applicants.value.filter(a => a.status === 'shortlisted').length.toString() },
  { id: 'interviews', title: 'المقابلات', number: applicants.value.filter(a => a.status === 'interview').length.toString() },
  { id: 'contract', title: 'العقد', number: applicants.value.filter(a => a.status === 'contract_sent').length.toString() },
  { id: 'accepted', title: 'وافق', number: applicants.value.filter(a => a.status === 'accepted').length.toString() },
  { id: 'refused', title: 'رفض', number: applicants.value.filter(a => a.status === 'refused').length.toString() },
])

const interviewDialogVisible = ref(false);
const interviewTargetApplicants = ref([]);
const contractDialogVisible = ref(false);
const contractTargetApplicants = ref([]);
const applicantModalOpen = ref(false);
const selectedApplicant = ref(null);

const loadApplicants = async ({ initial = false } = {}) => {
  if (initial) applicantsLoading.value = true;
  error.value = null;
  const jobId = route.query.id || route.params.id || 1;
  try {
    const [jobRes, appsRes] = await Promise.all([
      useApi().get(`/jobs/${jobId}`),
      useApi().get(`/jobs/${jobId}/applications`)
    ]);
    if (jobRes.error) useToast().show(jobRes.error, "error");
    if (appsRes.error) useToast().show(appsRes.error, "error");
    if (jobRes.data) jobDetails.value = jobRes.data;
    if (appsRes.data) {
      applicants.value = appsRes.data.items || appsRes.data;
    }
  } catch (e) {
    error.value = e;
    useToast().show("حدث خطأ أثناء تحميل البيانات", "error");
  }
  finally { if (initial) applicantsLoading.value = false; }
};

const moveToShortlist = async (app) => {
  try {
    const { error } = await useApi().put(`/applications/${app.id}/status`, { status: 'shortlisted' });
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تمت إضافة المتقدم إلى المرشحين", "success");
    await loadApplicants();
    selectedApplicants.value = selectedApplicants.value.filter(a => a.id !== app.id);
  } catch {
    useToast().show("حدث خطأ أثناء التحديث", "error");
  }
};

const bulkAction = async (action) => {
  if (!selectedApplicants.value.length) return;

  const ids = selectedApplicants.value.map(a => a.id);
  const { data, error } = await useApi().post('/applications/bulk-action', {
    applicationIds: ids,
    action,
  });

  if (error) {
    useToast().show(error, "error");
    return;
  }

  const succeeded = data?.filter(r => r.success).length || 0;
  const failed = data?.filter(r => !r.success).length || 0;

  if (succeeded > 0) {
    const label = action === 'shortlist' ? 'ترشيح' : 'رفض';
    useToast().show(`تم ${label} ${succeeded} متقدم بنجاح`, "success");
  }
  if (failed > 0) {
    useToast().show(`فشل ${failed} طلب`, "error");
  }

  selectedApplicants.value = [];
  await loadApplicants();
};

const showBulkRefuseConfirm = ref(false);
const bulkRefuseTargets = ref([]);

const confirmBulkRefuse = () => {
  bulkRefuseTargets.value = [...selectedApplicants.value];
  showBulkRefuseConfirm.value = true;
};

const confirmBulkRefuseFromChild = (apps) => {
  bulkRefuseTargets.value = apps;
  showBulkRefuseConfirm.value = true;
};

const executeBulkRefuse = async () => {
  showBulkRefuseConfirm.value = false;
  const targets = bulkRefuseTargets.value;
  if (!targets.length) return;

  const ids = targets.map(a => a.id);
  const { data, error } = await useApi().post('/applications/bulk-action', {
    applicationIds: ids,
    action: 'refuse',
  });

  if (error) {
    useToast().show(error, "error");
    return;
  }

  const succeeded = data?.filter(r => r.success).length || 0;
  const failed = data?.filter(r => !r.success).length || 0;

  if (succeeded > 0) {
    useToast().show(`تم رفض ${succeeded} متقدم بنجاح`, "success");
  }
  if (failed > 0) {
    useToast().show(`فشل ${failed} طلب`, "error");
  }

  bulkRefuseTargets.value = [];
  selectedApplicants.value = [];
  await loadApplicants();
};

const bulkActionFor = async (app, action) => {
  const { data, error } = await useApi().post('/applications/bulk-action', {
    applicationIds: [app.id],
    action,
  });

  if (error) {
    useToast().show(error, "error");
    return;
  }

  const succeeded = data?.filter(r => r.success).length || 0;
  if (succeeded > 0) {
    const labels = { refuse: 'تم رفض المتقدم', restore: 'تمت إعادة المتقدم للمتقدمين الجدد' };
    useToast().show(labels[action] || 'تمت العملية بنجاح', "success");
  }

  await loadApplicants();
};

const deleteApplicant = async (app) => {
  try {
    const { error } = await useApi().delete(`/applications/${app.id}`);
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تم حذف الطلب", "success");
    await loadApplicants();
  } catch {
    useToast().show("حدث خطأ أثناء الحذف", "error");
  }
};

const openInterview = (app) => {
  interviewTargetApplicants.value = Array.isArray(app) ? app : [app];
  interviewDialogVisible.value = true;
};

const submitInterview = async (payload) => {
  let ok = 0;
  for (const app of interviewTargetApplicants.value) {
    try {
      const { error } = await useApi().post('/interviews/schedule', {
        applicationId: app.id,
        method: payload.get ? payload.get('method') : payload.method,
        date: payload.get ? payload.get('date') : payload.date,
        time: payload.get ? payload.get('time') : payload.time,
        location: payload.get ? payload.get('location') : payload.location,
        link: payload.get ? payload.get('link') : payload.link,
        notes: payload.get ? payload.get('notes') : payload.notes,
      });
      if (!error) ok++;
      else useToast().show(error, "error");
    } catch {}
  }
  if (ok > 0) useToast().show(`تم جدولة ${ok} مقابلة بنجاح`, "success");
  interviewDialogVisible.value = false;
  await loadApplicants();
};

const openContract = (apps) => {
  contractTargetApplicants.value = apps;
  contractDialogVisible.value = true;
};

const openApplicantDetail = (app) => {
  selectedApplicant.value = { ...app, jobTitle: jobDetails.value?.title };
  applicantModalOpen.value = true;
};

const submitContract = async (payload) => {
  const ids = payload?.applicantIds?.length
    ? payload.applicantIds
    : contractTargetApplicants.value.map((a) => a.id);
  let ok = 0;
  for (const id of ids) {
    try {
      const formData = new FormData();
      formData.append("applicationId", String(id));
      if (payload?.file) formData.append("contractFile", payload.file);
      if (payload?.notes) formData.append("notes", payload.notes);
      if (payload?.endDate) formData.append("endDate", payload.endDate);
      const { error } = await useApi().post("/contracts/send", formData);
      if (!error) ok++;
      else useToast().show(error, "error");
    } catch {}
  }
  if (ok > 0) useToast().show(`تم إرسال ${ok} عقد بنجاح`, "success");
  else useToast().show("لم يتم إرسال أي عقد", "error");
  contractDialogVisible.value = false;
  await loadApplicants();
};

const openFilterDrawer = () => {};
const filterDrawer = ref(false);

onMounted(() => loadApplicants({ initial: true }));
</script>
<template>
  <div class="px-4 lg:px-0">
    <Breadcrumbs :items="breadcrumbs" />

    <div class="mt-3">
      <CustomTabs
        :tabs="[
          { id: 'about', title: 'عن الوظيفة' },
          { id: 'applicants', title: 'المتقدمون' },
        ]"
        initial-tab="applicants"
      >
        <template #about>
          <div>
            <div class="bg-white rounded-2xl shadow-sm p-4 sm:p-6 md:p-8 mb-4">
              <div class="flex flex-col gap-6">
                <h1 class="job-title text-lg lg:text-xl font-bold text-dark">
                  {{ jobDetails?.title }}
                </h1>
                <p class="job-desc text-sm text-dark/70 leading-[2]">
                  {{ jobDetails?.description }}
                </p>
                <div class="flex items-center gap-2">
                  <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
                    <img :src="jobDetails?.entityLogo || '/images/partner-3.svg'" class="w-10 h-6 object-cover" />
                  </span>
                  <span class="company-name text-sm text-dark">
                    {{ jobDetails?.entityName }}
                  </span>
                </div>
              </div>
            </div>
            <div v-if="jobDetails?.benefits?.length" class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                مميزات خاصة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li v-for="benefit in jobDetails.benefits" :key="benefit" class="text-sm text-muted mb-4">
                  {{ benefit }}
                </li>
              </ul>
            </div>
            <div v-if="jobDetails?.conditions?.length" class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                شروط القبول
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li v-for="condition in jobDetails.conditions" :key="condition" class="text-sm text-muted mb-4">
                  {{ condition }}
                </li>
              </ul>
            </div>
            <div v-if="jobDetails?.responsibilities?.length" class="p-5 bg-white rounded-2xl border-1 mb-4">
              <h3 class="text-base font-semibold text-primary pb-5 border-b mb-4">
                المزايا والمكافأة
              </h3>
              <ul class="ps-2 mt-2 list-disc list-inside">
                <li v-for="resp in jobDetails.responsibilities" :key="resp" class="text-sm text-muted mb-4">
                  {{ resp }}
                </li>
              </ul>
            </div>
          </div>
        </template>
        <template #applicants>
          <UiErrorState v-if="error" :message="error?.message || 'حدث خطأ' " @retry="() => loadApplicants({ initial: true })" />
          <UiLoadingSkeleton v-else-if="applicantsLoading" :count="6" :columns="2" height="160px" />
          <template v-else>
          <div :class="displayMethod">
            <ArrowTabs
              :tabs="tabs"
              v-model="activeTab"
            >
              <template #filter>
                <div class="filters">
                  <div class="flex flex-wrap items-center gap-2">
                    <div
                      class="flex-1 min-w-[250px] flex search relative items-center"
                    >
                      <input
                        class="w-full ps-10 h-[48px] border bg-bg-light py-2 text-sm rounded-xl focus:outline-none focus:border-primary transition"
                        type="text"
                        v-model="filter.search"
                      />
                      <span
                        class="flex items-center justify-center bg-white shadow-sm block h-[28px] w-[28px] rounded-lg absolute right-[10px]"
                      >
                        <Search />
                      </span>
                    </div>

                    <div class="flex flex-wrap gap-2 items-center">
                      <div>
                        <CustomSelect
                          :items="['الأحدث', 'الأقدم']"
                          v-model="filter.dateOrder"
                          class="select-style"
                        />
                      </div>
                      <button
                        class="hidden sm:block p-2 rounded-xl border bg-bg-subtle hover:border-primary transition"
                        :class="displayMethod == 'card' ? 'text-primary' : 'text-muted'"
                        @click="handleDisplayMethod('card')"
                      >
                        <GridIcon />
                      </button>
                      <button
                        class="hidden sm:block p-2 rounded-xl border bg-bg-subtle hover:border-primary transition"
                        :class="displayMethod == 'list' ? 'text-primary' : 'text-muted'"
                        @click="handleDisplayMethod('list')"
                      >
                        <ListIcon />
                      </button>
                    </div>
                  </div>
                </div>
              </template>

              <template #new>
                <UiEmptyState v-if="!filteredApplicants.length && !applicantsLoading" title="لا يوجد متقدمون جدد" description="لم يتقدم أحد لهذه الوظيفة بعد" />
                <div v-else-if="selectedApplicants.length">
                  <div class="flex flex-col sm:flex-row justify-between sm:items-center gap-6 mb-4">
                    <h2 class="text-sm lg:text-lg font-medium text-muted">
                      تم تحديد {{ selectedApplicants.length }} متقدمين
                    </h2>
                    <div class="self-end flex gap-2">
                      <button
                        class="btn-primary text-sm"
                        @click="bulkAction('shortlist')"
                      >
                        نقل إلى المرشحين
                      </button>
                      <button
                        class="text-sm py-3 px-4 xs:px-6 rounded-full bg-danger text-white hover:bg-danger/85 transition"
                        @click="confirmBulkRefuse"
                      >
                        رفض المحدد
                      </button>
                    </div>
                  </div>
                </div>
                <div class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="applicant in filteredApplicants"
                    :key="applicant.id"
                    :applicant="applicant"
                    :action="true"
                    :select="true"
                    :status="applicant.status || 'new'"
                    :job-title="jobDetails?.title"
                    badge-text="جديد"
                    badge-style="bg-primary/10 text-primary"
                    card-style=" bg-[#fff]"
                    v-model="selectedApplicants"
                    @shortlist="moveToShortlist(applicant)"
                    @interview="openInterview(applicant)"
                    @refuse="bulkActionFor(applicant, 'refuse')"
                    @view-details="openApplicantDetail(applicant)"
                  />
                </div>
              </template>
              <template #shortlist>
                <UiEmptyState v-if="!filteredApplicants.length" title="لا يوجد مرشحين" description="لم يتم ترشيح أي متقدم بعد" />
                <JobShortlist v-else
                  :shor-list="applicants.filter(a => a.status === 'shortlisted')"
                  :job-title="jobDetails?.title"
                  :display-method="displayMethod"
                  @shortlist="moveToShortlist"
                  @interview="openInterview"
                  @schedule-interview="openInterview"
                  @bulk-refuse="confirmBulkRefuseFromChild"
                  @view-details="openApplicantDetail"
                />
              </template>
              <template #interviews>
                <UiEmptyState v-if="!filteredApplicants.length" title="لا توجد مقابلات" description="لم يتم جدولة أي مقابلات بعد" />
                <JobInterviews v-else
                  :interview-list="applicants.filter(a => a.status === 'interview')"
                  :job-title="jobDetails?.title"
                  :display-method="displayMethod"
                  @shortlist="moveToShortlist"
                  @interview="openInterview"
                  @send-contract="openContract"
                  @bulk-refuse="confirmBulkRefuseFromChild"
                  @view-details="openApplicantDetail"
                />
              </template>
              <template #contract>
                <UiEmptyState v-if="!applicants.filter(a => a.status === 'contract_sent').length" title="لم يتم إرسال عقود" description="لم يتم إرسال أي عقد بعد" />
                <div v-else class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="applicant in applicants.filter(a => a.status === 'contract_sent')"
                    :key="applicant.id"
                    :applicant="applicant"
                    :action="true"
                    :status="applicant.status"
                    :job-title="jobDetails?.title"
                    badge-text="تم إرسال العقد"
                    badge-style="bg-badge-green/10 text-badge-green"
                    card-style=" bg-[#fff]"
                    @view-details="openApplicantDetail(applicant)"
                  />
                </div>
              </template>
              <template #accepted>
                <UiEmptyState v-if="!applicants.filter(a => a.status === 'accepted').length" title="لم يقبل أحد" description="لم يقبل أي مرشح العقد بعد" />
                <div v-else class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="applicant in applicants.filter(a => a.status === 'accepted')"
                    :key="applicant.id"
                    :applicant="applicant"
                    :action="true"
                    :status="applicant.status"
                    :job-title="jobDetails?.title"
                    badge-text="تم قبول العقد"
                    badge-style="bg-success/10 text-success"
                    card-style=" bg-[#fff]"
                    @view-details="openApplicantDetail(applicant)"
                  />
                </div>
              </template>
              <template #refused>
                <UiEmptyState v-if="!applicants.filter(a => a.status === 'refused').length" title="لا يوجد رافضين" description="لم يرفض أي مرشح العقد" />
                <div v-else class="grid applicant-grid gap-4">
                  <ApplicantCard
                    v-for="applicant in applicants.filter(a => a.status === 'refused')"
                    :key="applicant.id"
                    :applicant="applicant"
                    :action="true"
                    :status="applicant.status"
                    :job-title="jobDetails?.title"
                    badge-text="تم رفض العقد"
                    badge-style="bg-danger/10 text-danger"
                    card-style=" bg-[#fff]"
                    @delete="deleteApplicant(applicant)"
                    @restore="bulkActionFor(applicant, 'restore')"
                    @view-details="openApplicantDetail(applicant)"
                  />
                </div>
              </template>
            </ArrowTabs>
          </div>
        </template>
        </template>
      </CustomTabs>
    </div>

    <SendInterview
      v-model="interviewDialogVisible"
      :applicants="interviewTargetApplicants"
      :job-title="jobDetails?.title"
      @submit-interview="submitInterview"
    />
    <SendContract
      v-model="contractDialogVisible"
      :applicants="contractTargetApplicants"
      :job-title="jobDetails?.title"
      @submit-contract="submitContract"
    />
    <FilterDrawer v-model="filterDrawer" />
    <ConfirmDialog
      v-model="showBulkRefuseConfirm"
      title="رفض المتقدمين"
      :message="`هل أنت متأكد من رفض ${bulkRefuseTargets.length} متقدمين؟`"
      confirm-text="رفض"
      :danger="true"
      @confirm="executeBulkRefuse"
    />
    <ApplicantDetailModal
      v-model="applicantModalOpen"
      :applicant="selectedApplicant"
    />
  </div>
</template>
<style lang="scss" scoped>
.transition-height {
  transition: height 0.3s ease;
}

.select-style {
  background-color: rgb(255, 255, 255);
  border: 1px solid theme('colors.bg-light');
  box-shadow: 0px 1px 2px rgba(18, 18, 23, 0.05);
  border-radius: 16px;
  min-width: 140px;
  height: 48px;
  width: 100%;
}
</style>
