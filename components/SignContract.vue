<template>
  <Dialog v-model="model">
    <div
      class="max-w-4xl mx-auto bg-white rounded-2xl overflow-hidden relative"
    >
      <div class="bg-bg-light p-6">
        <h2 class="text-lg lg:text-xl font-bold text-gray-800 mb-3">
          توقيع العقد إلكترونيً
        </h2>
        <p class="text-gray-600 text-sm">التوقيع محمي وآمن</p>
      </div>
      <!-- Contract Content -->
      <div class="contract-content p-4 sm:p-6">
        <div class="border rounded-2xl overflow-hidden">
          <!-- file header -->
          <div class="flex items-center justify-between gap-3 bg-bg-light px-4 py-3 border-b">
            <div class="flex items-center gap-2 min-w-0">
              <span class="block p-2 bg-white rounded-xl shrink-0">
                <Pdf />
              </span>
              <div class="min-w-0">
                <span class="block text-sm text-slate-900 truncate">
                  {{ contract?.fileName || 'ملف العقد' }}
                </span>
                <span class="text-xs text-slate-400">{{ fileSizeText }}</span>
              </div>
            </div>
            <span class="text-xs text-muted shrink-0">
              ينتهي التوقيع
              {{ contract?.endDate ? formatDate(contract.endDate) : '-' }}
            </span>
          </div>

          <!-- file preview -->
          <iframe
            v-if="previewUrl"
            :src="previewUrl"
            title="معاينة العقد"
            class="w-full h-[60vh] min-h-[420px] bg-white"
          ></iframe>
          <div
            v-else
            class="h-[420px] flex items-center justify-center text-muted text-sm"
          >
            لا يمكن عرض ملف العقد
          </div>
        </div>
      </div>
      <!-- Signature Confirmation -->
      <div class="px-6 mb-4" v-if="!readonly">
        <div class="mb-6">
          <TextInput
            :label="`تأكيد التوقيع اكتب بالأسفل (${sequenceConfirm})`"
            class="bg-bg-light w-full rounded-2xl h-12 text-sm px-2 placeholder:text-xs"
            placeholder="أوافق علي كل بنود العقد"
            v-model="signatureAgreement"
          />
        </div>
        <!--  -->
        <div class="flex flex-col">
          <label class="flex items-center cursor-pointer mb-6">
            <input
              type="checkbox"
              v-model="agreementConfirmed"
              class="w-4 h-4 text-primary accent-primary rounded border-gray-300 focus:ring-primary"
            />
            <span class="mr-2 text-sm font-bold">أوافق وأوقع العقد</span>
          </label>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex justify-between bg-bg-light p-6">
        <button @click="cancelContract" class="btn-outline text-sm px-10">
          إلغاء
        </button>
        <button
          @click="submitContract"
          :disabled="!canSubmit || isSubmitting"
          class="btn-primary text-sm px-10"
        >
          {{ isSubmitting ? 'جاري التوقيع...' : 'توقيع العقد' }}
        </button>
      </div>

    </div>
  </Dialog>
</template>

<script setup>
import { ref, computed } from "vue";
import Dialog from "./Dialog.vue";
import TextInput from "./elements/TextInput.vue";
import { formatDate } from "~/services/help";

/** @type {{ readonly: boolean, contractName: string, contractId: number|null, contract: object|null }} */
const props = defineProps({
  readonly: {
    type: Boolean,
    default: false,
  },
  contractName: {
    type: String,
    default: 'العقد',
  },
  contractId: {
    type: [Number, String],
    default: null,
  },
  contract: {
    type: Object,
    default: null,
  },
});
const model = defineModel();
const emit = defineEmits(['signed']);

const sequenceConfirm = ref("أوافق على كل بنود العقد");
const agreementConfirmed = ref(false);
const signatureAgreement = ref(null);
const showSignaturePad = ref(false);

const fileSizeText = computed(() => {
  const bytes = Number(props.contract?.fileSize) || 0;
  if (!bytes) return "";
  if (bytes >= 1024 * 1024) return `${(bytes / 1024 / 1024).toFixed(2)} Mb`;
  if (bytes >= 1024) return `${(bytes / 1024).toFixed(1)} Kb`;
  return `${bytes} bytes`;
});

const config = useRuntimeConfig();
const previewUrl = computed(() => {
  const url = props.contract?.fileUrl;
  if (!url) return "";
  if (/^https?:\/\//.test(url)) return url;
  try {
    const apiOrigin = new URL(config.public.apiBaseUrl).origin;
    return apiOrigin + url;
  } catch {
    return url;
  }
});

  const canSubmit = computed(
    () => agreementConfirmed.value && signatureAgreement.value === sequenceConfirm.value
  );

const isSubmitting = ref(false);

const submitContract = async () => {
  if (!props.contractId) {
    useToast().show("تعذر تحديد العقد المراد توقيعه", "error");
    return;
  }
  isSubmitting.value = true;
  try {
    const { error } = await useApi().post(`/contracts/${props.contractId}/sign`);
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تم توقيع العقد وتوثيقه بنجاح", "success");
    agreementConfirmed.value = false;
    signatureAgreement.value = null;
    model.value = false;
    emit('signed');
  } catch {
    useToast().show("حدث خطأ أثناء توقيع العقد", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const cancelContract = () => {
  agreementConfirmed.value = false;
  signatureAgreement.value = null;
  model.value = false;
};
</script>

<style scoped>
.contract-content {
  line-height: 1.8;
  font-size: 1.05rem;
}

[dir="rtl"] .list-decimal {
  list-style-type: arabic-indic;
}
</style>
