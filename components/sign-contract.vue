<template>
  <Dialog v-model="model">
    <div
      class="max-w-4xl mx-auto bg-white rounded-2xl overflow-hidden relative"
    >
      <div class="bg-[#f5f5f5] p-6">
        <h2 class="text-lg lg:text-xl font-bold text-gray-800 mb-3">
          توقيع العقد إلكترونيً
        </h2>
        <p class="text-gray-600 text-sm">التوقيع محمي وآمن</p>
      </div>
      <!-- Contract Content -->
      <div class="contract-content p-8">
        <div class="border rounded-2xl relative overflow-hidden">
          <img src="~/assets/images/contract.png" />
        </div>
      </div>
      <!-- Signature Confirmation -->
      <div class="px-6 mb-4" v-if="!readonly">
        <div class="mb-6">
          <TextInput
            :label="`تأكيد التوقيع اكتب بالأسفل (${sequenceConfirm})`"
            class="bg-[#f5f5f5] w-full rounded-2xl h-12 text-sm px-2 placeholder:text-xs"
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
              class="w-4 h-4 text-primary accent-[#ecb42b] rounded border-gray-300 focus:ring-primary"
            />
            <span class="mr-2 text-sm font-bold">أوافق وأوقع العقد</span>
          </label>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex justify-between bg-[#f5f5f5] p-6">
        <button @click="cancelContract" class="btn-outline text-sm px-10">
          إلغاء
        </button>
        <button
          @click="submitContract"
          :disabled="!canSubmit"
          class="btn-primary text-sm px-10"
        >
          توثيق العقد
        </button>
      </div>

      <!-- Signature Pad Modal -->
      <!-- 
        <Modal v-model="showSignaturePad" title="التوقيع الإلكتروني">
          <SignaturePad @save="handleSignatureSave" @clear="handleSignatureClear" />
          <template #footer>
            <button
              @click="showSignaturePad = false"
              class="px-4 py-2 text-gray-700"
            >
              إغلاق
            </button>
          </template>
        </Modal> -->
    </div>
  </Dialog>
</template>

<script setup>
import { ref, computed } from "vue";
import Dialog from "./Dialog.vue";
import TextInput from "./elements/text-input.vue";

const props = defineProps({
  readonly: {
    type: Boolean,
    default: false,
  },
});
const model = defineModel();

const sequenceConfirm = ref("أوافق على كل بنود العقد");
const agreementConfirmed = ref(false);
const signatureAgreement = ref(null);
const showSignaturePad = ref(false);

const canSubmit = computed(
  () =>
    agreementConfirmed.value &&
    signatureAgreement.value == sequenceConfirm.value
);

const submitContract = () => {
  // Implement contract submission logic
  console.log("Contract submitted with signature");
  alert("تم توثيق العقد بنجاح");
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
