<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          اضافة السيرة الذاتية
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أرفق سيرتك الذاتية لتُعرض ضمن ملفك الشخصي، مما يساعد الجهات في تقييم
          خبراتك ومهاراتك بشكل احترافي. يُفضل رفع الملف بصيغة PDF وبحجم لا
          يتجاوز 5 ميغابايت.
        </p>
      </div>

      <Form @submit="submitApplication" v-slot="{ errors }" ref="form">
        <div class="px-6 mb-10">
          <!-- Contaract send Form -->
          <div>
            <label for="contractFile" class="block text-sm mb-3">
              السيرة الذاتية
              <span class="text-red-500">*</span>
            </label>
            <Field
              name="contractFile"
              id="contractFile"
              label="السيرة الذاتية"
              rules="validateContractFile"
              v-model="cvFile"
            >
              <FileInput
                accept=".pdf,.doc,.docx"
                @change="handleFileChange($event)"
              />
            </Field>
            <ErrorMessage
              name="contractFile"
              class="text-xs block text-red-500 mt-2"
            />
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
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
            <span v-if="isSubmitting">جاري التحميل ...</span>
            <span v-else> إضافة </span>
          </button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { defineRule, ErrorMessage, Field, Form } from "vee-validate";
import FileInput from "./elements/FileInput.vue";

const form = ref(null);
const model = defineModel();
const cvFile = ref();
defineRule("validateContractFile", (value) => {
  if (!value || !(value instanceof File)) return "قم برفع ملف العقد أولا !";

  return true;
});

const handleFileChange = (event) => {
  cvFile.value = event.target.files[0];
};
const isSubmitting = ref(false);
const submitApplication = async () => {
  try {
    isSubmitting.value = true;
    await new Promise((resolve) =>
      setTimeout(() => {
        resolve(true);
      }, 1000)
    );

    form.value.reset();
  } catch (error) {
    alert("done!");
  } finally {
    isSubmitting.value = false;
  }
};
const cancelApplication = () => {
  model.value = false;
};
</script>
