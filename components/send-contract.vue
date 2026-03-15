<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-[#f5f5f5] p-6">
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
            badge-text="القائمة المختصرة"
            badge-style="bg-[#35685F]/10 text-[#35685F]"
            card-style=" bg-[#f8f9f9]"
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
        </div>

        <!-- Action Buttons -->
        <div
          class="p-4 sm:p-6 flex justify-between space-x-3 space-x-reverse bg-[#f5f5f5]"
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
import ApplicantCard from "./applicant-card.vue";
import FileInput from "./elements/file-input.vue";

const model = defineModel();
const props = defineProps(["applicants"]);
const form = ref(null);
const isSubmitting = ref(false);
defineRule("validateContractFile", (value) => {
  if (!value) return "قم برفع ملف العقد أولا !";

  return true;
});

// Form data structure
const formData = reactive({
  contractFile: null,
  applicantIds: props.applicants.map((applicant) => applicant.id), // Assuming applicants have IDs
});

// Form submission handler
const handleSubmit = async (values, { resetForm }) => {
  try {
    isSubmitting.value = true;

    // Prepare FormData for submission
    const formPayload = new FormData();
    Object.entries(formData).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formPayload.append(key, value);
      }
    });

    // Here you would typically make an API call
    const response = await submitInterviewData(formPayload);

    // On success
    alert("تم إرسال العقد بنجاح");
    resetForm();
    model.value = false;
  } catch (error) {
    console.error("Submission error:", error);
    alert("حدث خطأ أثناء محاولة حفظ البيانات");
  } finally {
    isSubmitting.value = false;
  }
};

// Mock API submission function
const submitInterviewData = async (formData) => {
  // Replace with actual API call
  console.log("Submitting:", Object.fromEntries(formData));
  return new Promise((resolve) => setTimeout(resolve, 1500));
};
const handleFileChange = (event) => {
  formData.contractFile = event.target.files[0];
};
const cancelApplication = () => {
  form.value.resetForm();
  model.value = false;
};
</script>
