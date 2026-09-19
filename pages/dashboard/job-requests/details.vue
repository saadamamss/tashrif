<script setup>
import AddToCalendar from "~/components/AddToCalendar.vue";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomTabs from "~/components/elements/CustomTabs.vue";
import Pdf from "~/components/icons/pdf.vue";
import SignContract from "~/components/SignContract.vue";
import { formatDate } from "~/services/help";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "individual"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'تفاصيل الطلب',
})

const statusMeta = {
  new: { label: 'قيد المراجعة', badge: 'new', title: 'قيد المراجعة', msg: 'نحن بانتظار رد الجهة المعلنة. سيتم إشعارك فور تحديث الحالة.' },
  shortlisted: { label: 'قبول مبدئي', badge: 'preliminary', title: 'تم القبول المبدئي', msg: '🎉 تهانينا! لقد تم ترشيحك مبدئياً لهذه الوظيفة. وسيتم تحديد موعد للمقابلة قريباً.' },
  interview: { label: 'مقابلة', badge: 'interview', title: 'مقابلة شخصية', msg: 'تم تحديد موعد للمقابلة، يرجى مراجعة تفاصيل الموعد والالتزام بالحضور.' },
  contract_sent: { label: 'تم إرسال العقد', badge: 'contract_sent', title: 'تم إرسال العقد', msg: '🎉 تم قبولك! اطلع على العقد ووقّعه إلكترونياً لتأكيد انضمامك.' },
  accepted: { label: 'مقبول', badge: 'accepted', title: 'تم القبول', msg: '🎉 تهانينا! لقد تم قبولك نهائياً لهذه الوظيفة.' },
  refused: { label: 'مرفوض', badge: 'refused', title: 'لم يتم القبول', msg: 'نأسف لإعلامك بعدم قبول طلبك لهذه الوظيفة حالياً.' },
  withdrawn: { label: 'مسحوب', badge: 'withdrawn', title: 'تم سحب الطلب', msg: 'لقد سحبت طلبك لهذه الوظيفة بنجاح.' },
}

const canWithdraw = computed(() => {
  const s = application.value?.status
  return s === 'new' || s === 'shortlisted' || s === 'interview'
})

const showWithdrawConfirm = ref(false)
const withdrawLoading = ref(false)

async function withdrawApplication() {
  withdrawLoading.value = true
  try {
    const { data, error } = await useApi().put(`/applications/${application.value.id}/withdraw`)
    if (error) {
      useToast().show(error, 'error')
      return
    }
    if (data) {
      application.value = data
      useToast().show('تم سحب الطلب بنجاح', 'success')
      showWithdrawConfirm.value = false
    }
  } catch {
    useToast().show('حدث خطأ أثناء سحب الطلب', 'error')
  } finally {
    withdrawLoading.value = false
  }
}

const breadcrumbs = computed(() => [
  {
    label: "طلبات العمل",
    to: "/dashboard/job-requests",
    active: true,
  },
  {
    label: application.value?.job?.title || 'الوظيفة',
    active: false,
  },
]);

const signContractOpen = ref(false);
const application = ref(null);
const interview = ref(null);
const contract = ref(null);
const route = useRoute();
const loading = ref(true);
const statusHistory = ref([]);
const statusHistoryLoading = ref(false);

const currentStatus = computed(() => application.value?.status || '')
const meta = computed(() => statusMeta[currentStatus.value] || statusMeta.new)

onMounted(async () => {
  loading.value = true;
  const appId = route.query.id || route.params.id || 1
  try {
    const { data, error } = await useApi().get(`/applications/${appId}`);
    if (error) {
      useToast().show(error, "error");
      return;
    }
    if (data) {
      application.value = data
      const [iv, ct] = await Promise.all([
        useApi().get('/interviews?limit=100'),
        useApi().get('/contracts?limit=100'),
      ])
      if (iv.error) useToast().show(iv.error, "error")
      if (ct.error) useToast().show(ct.error, "error")
      interview.value = (iv.data?.items || []).find(i => i.applicationId === data.id) || null
      contract.value = (ct.data?.items || []).find(c => c.applicationId === data.id) || null

      // Load status history
      try {
        statusHistoryLoading.value = true
        const { data: historyData, error: historyError } = await useApi().get(`/applications/${data.id}/status-history`)
        if (historyError) {
          useToast().show(historyError, "error")
        } else if (historyData) {
          statusHistory.value = historyData
        }
      } catch {
        useToast().show("حدث خطأ أثناء تحميل سجل الحالات", "error")
      } finally {
        statusHistoryLoading.value = false
      }
    }
  } catch {
    useToast().show("حدث خطأ أثناء تحميل بيانات الطلب", "error");
  } finally {
    loading.value = false;
  }
});


function downloadContract() {
  if (!contract.value?.fileUrl) return
  const link = document.createElement('a')
  link.href = contract.value.fileUrl
  link.download = 'contract.pdf'
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  link.remove()
}
</script>
<template>
  <div class="px-4 lg:px-0 mb-8">
    <Breadcrumbs :items="breadcrumbs" />
    <UiLoadingSkeleton v-if="loading" :count="1" height="420px" rounded="2xl" />
    <template v-else-if="application">
    <!--  -->
    <div class="grid grid-cols-7 gap-6 items-start mt-4">
      <div class="col-span-7 lg:col-span-4 xl:col-span-5">
        <div
          class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8 mb-8 relative overflow-hidden"
        >
          <div
            class="badge absolute top-0 rounded-r-3xl rounded-t-[0px] left-0 px-3 py-2 min-w-24 text-center bg-blue-100"
            :x-status="meta.badge"
          >
            <span class="text-xs">
              {{ meta.label }}
            </span>
          </div>
          <div class="flex flex-col gap-6">
            <h1 class="job-title text-lg lg:text-xl font-bold text-dark">
              {{ application?.job?.title }}
            </h1>
            <p class="job-desc text-sm text-dark/70 leading-[2]">
              {{ application?.job?.description || application?.description }}
            </p>

            <div class="flex items-center gap-2">
              <span class="company-logo border rounded-md overflow-hidden py-1 px-2">
                <img :src="application?.job?.entityLogo || '/images/partner-3.svg'" class="w-10 h-6 object-cover" />
              </span>
              <span class="company-name text-sm text-dark">
                {{ application?.job?.entityName || application?.entityName }}
              </span>
            </div>
          </div>
        </div>

        <!--  -->
        <CustomTabs
          :tabs="[
            { id: 'benefits', title: 'المزايا والمكافأة' },
            { id: 'conditions', title: 'شروط القبول' },
            { id: 'tasks', title: 'المهام والمسؤوليات' },
          ]"
          initial-tab="benefits"
        >
          <!-- Named slots for each tab content -->
          <template #benefits>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3 class="text-base font-semibold text-black pb-5 border-b mb-4">
                مميزات خاصة
              </h3>
              <ul v-if="application?.job?.benefits?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="benefit in application.job.benefits" :key="benefit" class="text-sm text-muted mb-4">
                  {{ benefit }}
                </li>
              </ul>
            </div>
          </template>

          <template #conditions>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3 class="text-base font-semibold text-black pb-5 border-b mb-4">
                شروط القبول
              </h3>
              <ul v-if="application?.job?.conditions?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="condition in application.job.conditions" :key="condition" class="text-sm text-muted mb-4">
                  {{ condition }}
                </li>
              </ul>
            </div>
          </template>

          <template #tasks>
            <div class="p-5 bg-white rounded-2xl border-2">
              <h3 class="text-base font-semibold text-black pb-5 border-b mb-4">
                المهام والمسؤوليات
              </h3>
              <ul v-if="application?.job?.responsibilities?.length" class="ps-2 mt-2 list-disc list-inside">
                <li v-for="resp in application.job.responsibilities" :key="resp" class="text-sm text-muted mb-4">
                  {{ resp }}
                </li>
              </ul>
            </div>
          </template>
        </CustomTabs>
      </div>

      <div class="col-span-7 lg:col-span-3 xl:col-span-2 space-y-6">
        <!-- status -->
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">حالة الطلب</h3>
          </div>
          <div class="space-y-4 pt-4">
            <div class="p-4 rounded-xl badge" :x-status="meta.badge">
              <h3 class="text-sm mb-3">{{ meta.title }}</h3>
              <p class="text-xs text-muted">{{ meta.msg }}</p>
            </div>
            <p class="text-xs text-muted">
              تم التقديم في تاريخ: {{ formatDate(application.createdAt) }}
            </p>
            <button
              v-if="canWithdraw"
              @click="showWithdrawConfirm = true"
              class="w-full text-center btn-outline text-sm text-red-600 border-red-300 hover:bg-red-50"
            >
              سحب الطلب
            </button>
          </div>
        </div>

        <!-- status history -->
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">سجل الحالات</h3>
          </div>
          <div class="pt-4">
            <UiLoadingSkeleton v-if="statusHistoryLoading" :count="3" height="40px" />
            <StatusTimeline v-else :history="statusHistory" />
          </div>
        </div>

        <!-- interview -->
        <div
          v-if="interview"
          class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6"
        >
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">موعد المقابلة الشخصية</h3>
          </div>
          <div class="details py-6 flex flex-col gap-4">
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="18" height="18" />
              </span>
              <span class="text-icon-muted text-xs">
                {{ formatDate(interview.date) }}
              </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <Clock />
              </span>
              <span class="text-icon-muted text-xs"> الساعة {{ interview.time }} </span>
            </div>
            <div class="flex gap-2 items-center" v-if="interview.location">
              <span>
                <Location />
              </span>
              <span class="text-icon-muted text-xs">
                {{ interview.location }}
              </span>
            </div>
            <div class="flex gap-2 items-center" v-else-if="interview.link">
              <span>
                <Location />
              </span>
              <span class="text-icon-muted text-xs">
                مقابلة عن بعد — {{ interview.link }}
              </span>
            </div>
          </div>

          <div class="p-4 rounded-xl bg-bg-subtle mb-4" v-if="interview.notes">
            <h3 class="text-sm mb-3 text-surface">📌 ملاحظات</h3>
            <div class="text-xs text-muted">
              <p>{{ interview.notes }}</p>
            </div>
          </div>

          <div class="flex gap-4">
            <AddToCalendar
              :title="`مقابلة شخصية لوظيفة ${application?.job?.title || ''}`"
              :start="interview.date"
              :start-time="interview.time"
              :location="interview.location || interview.link"
            />
          </div>
        </div>

        <!-- contract -->
        <div
          v-if="contract"
          class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6"
        >
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">العقد الوظيفى</h3>
          </div>
          <div class="details py-6 flex flex-col gap-4">
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="18" height="18" />
              </span>
              <span class="text-icon-muted text-xs">
                {{ contract.status === 'signed' ? 'تم توقيع العقد' : 'العقد بانتظار التوقيع' }}
              </span>
            </div>
          </div>

          <div class="p-4 rounded-xl bg-bg-subtle mb-2">
            <div class="flex justify-between gap-4 items-center">
              <div class="flex flex-wrap items-center gap-2">
                <span class="block p-2 bg-white rounded-xl">
                  <Pdf />
                </span>
                <div>
                  <span class="block text-slate-900 text-sm mb-1">
                    عقد العمل
                  </span>
                  <span class="text-xs block text-slate-400">
                    {{ contract.status === 'signed' ? 'تم التوقيع' : 'PDF' }}
                  </span>
                </div>
              </div>
              <div>
                <button
                  class="block shadow-sm p-2 bg-white rounded-lg"
                  @click="downloadContract"
                >
                  <Download />
                </button>
              </div>
            </div>
          </div>

          <div class="flex gap-4">
            <button
              v-if="contract.status === 'sent'"
              @click="signContractOpen = true"
              class="flex-1 text-center btn-primary text-sm"
            >
              توقيع العقد الإلكترونى
            </button>
            <button
              v-else
              disabled
              class="flex-1 text-center btn-outline text-sm"
            >
              ✅ تم التوقيع
            </button>
          </div>
        </div>

        <!-- job details -->
        <div class="bg-white rounded-xl p-6 shadow-md">
          <div class="pb-4 border-b-2">
            <h3 class="text-lg font-medium">تفاصيل الوظيفة</h3>
          </div>
          <div class="space-y-4 pt-4">
            <div class="flex gap-2 items-center">
              <span>
                <Location width="20" height="20" />
              </span>
              <span class="text-xs text-icon-muted">
                {{ application?.job?.location || application?.location }}
              </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="21" height="20" />
              </span>
              <span class="text-icon-muted text-xs"> {{ application?.job?.hours || application?.hours }} </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <CalenderIcon width="21" height="20" />
              </span>
              <span class="text-icon-muted text-xs">
                {{ application?.job?.duration || application?.duration }}
              </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <MoneyIcon />
              </span>
              <span class="text-xs text-icon-muted"> {{ application?.job?.salary || application?.salary }} </span>
            </div>
            <div class="flex gap-2 items-center">
              <span>
                <PersonIcon width="20" height="20" color="#696C68" />
              </span>
              <span class="text-xs text-icon-muted">
                {{ (application?.job?.gender || application?.gender) === 'male' ? 'الذكور فقط' : (application?.job?.gender || application?.gender) === 'female' ? 'الإناث فقط' : 'رجال ونساء' }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    </template>
    <!--  -->
    <SignContract v-model="signContractOpen" />

    <!-- Withdraw Confirmation Dialog -->
    <Teleport to="body">
      <div
        v-if="showWithdrawConfirm"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
        @click.self="showWithdrawConfirm = false"
      >
        <div class="bg-white rounded-2xl p-6 max-w-sm mx-4 shadow-xl">
          <h3 class="text-lg font-bold text-dark mb-2">تأكيد سحب الطلب</h3>
          <p class="text-sm text-muted mb-6">
            هل أنت متأكد من سحب طلبك لهذه الوظيفة؟ لا يمكن التراجع عن هذا الإجراء.
          </p>
          <div class="flex gap-3">
            <button
              @click="showWithdrawConfirm = false"
              class="flex-1 btn-outline text-sm"
              :disabled="withdrawLoading"
            >
              إلغاء
            </button>
            <button
              @click="withdrawApplication"
              class="flex-1 btn-primary text-sm bg-red-600 hover:bg-red-700"
              :disabled="withdrawLoading"
            >
              {{ withdrawLoading ? 'جاري السحب...' : 'نعم، سحب الطلب' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style>
.transition-height {
  transition: height 0.3s ease;
}
</style>
