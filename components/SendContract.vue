<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          إرسال العقد الوظيفي
        </h2>
        <p class="text-gray-600 text-sm">
          يمكنك الآن إرسال العقد إلكترونيًا لمراجعته وتوقيعه. تأكد من مراجعة
          التفاصيل قبل الإرسال
        </p>
      </div>

      <div class="p-4 ms:p-6">
        <div class="space-y-3 mb-6">
          <ApplicantCard
            v-for="(applicant, i) in applicants"
            :key="i"
            :applicant="applicant"
            :job-title="jobTitle"
            badge-text="القائمة المختصرة"
            badge-style="bg-badge-green/10 text-badge-green"
            card-style=" bg-bg-subtle"
          />
        </div>
      </div>
      <Form @submit="handleSubmit" v-slot="{ errors }" ref="form">
        <div class="px-4 sm:px-6 mb-10">
          <!-- Contaract send Form -->
          <div>
            <label for="contractFile" class="block text-sm mb-3">
              رفع نسخة العقد
            </label>
            <Field
              name="contractFile"
              id="contractFile"
              label="رفع نسخة العقد"
              rules="validateContractFile"
              v-model="formData.contractFile"
            >
              <FileInput
                accept=".pdf,.doc,.docx"
                @change="handleFileChange($event)"
              />
            </Field>
            <ErrorMessage
              name="contractFile"
              class="text-xs block text-red-500"
            />
          </div>

          <!-- Contract expiry date -->
          <div class="mt-5">
            <TextInput
              name="endDate"
              id="contract-end-date"
              type="date"
              label="تاريخ انتهاء صلاحية التوقيع"
              height="48"
              placeholder="تاريخ الانتهاء"
              v-model="formData.endDate"
              rules="validateEndDate"
              required
              :error="errors.endDate"
            />
          </div>

          <!-- Contract notes -->
          <div class="mt-5">
            <label for="notes" class="block text-sm mb-2">
              ملاحظات العقد
            </label>
            <Field
              name="notes"
              v-model="formData.notes"
              as="textarea"
              rows="3"
              placeholder="أضف أي ملاحظات أو شروط إضافية للعقد (اختياري)"
              class="text-sm rounded-2xl w-full px-3 py-3 bg-bg-light border border-primary/0 focus:border-primary outline-none placeholder:text-xs transition duration-300 resize-none"
            />
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="p-4 sm:p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button
            type="button"
            @click="cancelApplication"
            class="btn-outline text-sm"
          >
            إلغاء
          </button>
          <button
            type="submit"
            class="btn-primary text-sm"
            :disabled="isSubmitting"
          >
            <span v-if="isSubmitting">جاري الإرسال...</span>
            <span v-else> إرسال العقد</span>
          </button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { ref, reactive } from "vue";
import { defineRule, ErrorMessage, Field, Form } from "vee-validate";
import Dialog from "./Dialog.vue";
import ApplicantCard from "./ApplicantCard.vue";
import FileInput from "./elements/FileInput.vue";
import TextInput from "./elements/TextInput.vue";

const model = defineModel();

/** @type {{ applicants: Array<import('~/types/application').Application>, jobTitle?: string }} */
const props = defineProps(["applicants", "jobTitle"]);
const form = ref(null);
const isSubmitting = ref(false);
defineRule("validateContractFile", (value) => {
  if (!value) return "قم برفع ملف العقد أولا !";

  return true;
});

defineRule("validateEndDate", (value) => {
  if (!value) return "حدد تاريخ انتهاء صلاحية التوقيع";

  // The deadline is the end of the picked day and must leave a real signing
  // window: strictly more than 24h after sending (mirrors the backend rule).
  const endDate = new Date(`${value}T23:59:59`);
  if (endDate.getTime() - Date.now() <= 24 * 60 * 60 * 1000)
    return "يجب أن يكون تاريخ الانتهاء بعد 24 ساعة على الأقل من الإرسال";

  return true;
});

// Form data structure
const formData = reactive({
  contractFile: null,
  notes: "",
  endDate: "",
  applicantIds: props.applicants.map((applicant) => applicant.id), // Assuming applicants have IDs
});

// Form submission handler
const handleSubmit = async (values, { resetForm }) => {
  try {
    isSubmitting.value = true;

    emit("submit-contract", {
      file: formData.contractFile,
      notes: formData.notes,
      endDate: formData.endDate,
      applicantIds: [...formData.applicantIds],
    });

    resetForm();
    formData.notes = "";
    formData.endDate = "";
    model.value = false;
  } catch (error) {
    console.error("Submission error:", error);
    alert("حدث خطأ أثناء محاولة حفظ البيانات");
  } finally {
    isSubmitting.value = false;
  }
};

const emit = defineEmits(["submit-contract"]);
const handleFileChange = (event) => {
  formData.contractFile = event.target.files[0];
};
const cancelApplication = () => {
  form.value.resetForm();
  model.value = false;
};
</script>
