<script setup lang="ts">
import type { StatusHistoryEntry } from '~/types/status-history'
import { formatDate, buildImageUrl } from '~/services/help'

const props = defineProps<{
  applicant: any
}>()

const model = defineModel<boolean>()

const history = ref<StatusHistoryEntry[]>([])
const historyLoading = ref(false)
const historyError = ref('')

watch(model, async (open) => {
  if (open && props.applicant?.id) {
    await loadHistory()
  }
})

async function loadHistory() {
  historyLoading.value = true
  historyError.value = ''
  try {
    const { data, error } = await useApi().get(`/applications/${props.applicant.id}/status-history`)
    if (error) {
      historyError.value = error
    } else if (data) {
      history.value = data
    }
  } catch {
    historyError.value = 'حدث خطأ أثناء تحميل سجل الحالات'
  } finally {
    historyLoading.value = false
  }
}

const genderLabel = computed(() => {
  const value = (props.applicant?.userGender || props.applicant?.gender || '').toLowerCase()
  if (value === 'male' || value === 'ذكر') return 'ذكر'
  if (value === 'female' || value === 'أنثى' || value === 'انثى') return 'أنثى'
  return value
})

const avatarSrc = computed(() => buildImageUrl(props.applicant?.avatarUrl || props.applicant?.avatar))
</script>

<template>
  <Teleport to="body">
    <div
      v-if="model"
      class="fixed inset-0 z-[100] flex items-center justify-center bg-black/50 p-4"
      @click.self="model = false"
    >
      <div class="bg-white rounded-2xl max-w-lg w-full max-h-[85vh] overflow-y-auto shadow-xl">
        <!-- Header -->
        <div class="flex items-center justify-between p-5 border-b">
          <h3 class="text-lg font-bold text-dark">تفاصيل الطلب</h3>
          <button
            @click="model = false"
            class="p-1.5 rounded-lg hover:bg-bg-subtle transition"
          >
            <Close :width="20" :height="20" color="#696C68" bg-color="transparent" />
          </button>
        </div>

        <!-- Applicant info -->
        <div class="p-5 border-b">
          <div class="flex items-center gap-3 mb-4">
            <span class="w-12 h-12 rounded-full bg-bg-light flex items-center justify-center text-primary font-bold text-lg overflow-hidden">
              <img v-if="avatarSrc" :src="avatarSrc" class="w-full h-full object-cover" />
              <span v-else>{{ (applicant?.userName || applicant?.name || '').charAt(0) }}</span>
            </span>
            <div>
              <p class="text-sm font-medium text-dark">{{ applicant?.userName || applicant?.name }}</p>
              <p v-if="applicant?.jobTitle" class="text-xs text-muted">{{ applicant.jobTitle }}</p>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-3 text-xs">
            <div>
              <span class="text-muted block mb-1">المؤهل العلمي</span>
              <span class="font-medium">{{ applicant?.qualification }}</span>
            </div>
            <div>
              <span class="text-muted block mb-1">الجنس</span>
              <span class="font-medium">{{ genderLabel }}</span>
            </div>
            <div>
              <span class="text-muted block mb-1">المدينة</span>
              <span class="font-medium">{{ applicant?.userCity || applicant?.city }}</span>
            </div>
            <div>
              <span class="text-muted block mb-1">تاريخ التقديم</span>
              <span class="font-medium">{{ formatDate(applicant?.createdAt) }}</span>
            </div>
            <div v-if="applicant?.experience" class="col-span-2">
              <span class="text-muted block mb-1">نبذة عن الخبرات</span>
              <span class="font-medium">{{ applicant.experience }}</span>
            </div>
            <div class="col-span-2">
              <span class="text-muted block mb-1">السيرة الذاتية</span>
              <a
                v-if="applicant?.cvFilePath"
                :href="buildImageUrl(applicant.cvFilePath)"
                target="_blank"
                class="font-medium text-primary"
              >
                {{ applicant.cvFileName || 'عرض السيرة الذاتية' }}
              </a>
              <span v-else class="font-medium text-muted">لا توجد سيرة</span>
            </div>
          </div>
        </div>

        <!-- Status History -->
        <div class="p-5">
          <h4 class="text-sm font-semibold text-dark mb-4">سجل الحالات</h4>
          <UiLoadingSkeleton v-if="historyLoading" :count="3" height="40px" />
          <UiErrorState v-else-if="historyError" :message="historyError" @retry="loadHistory" />
          <StatusTimeline v-else :history="history" />
        </div>
      </div>
    </div>
  </Teleport>
</template>
