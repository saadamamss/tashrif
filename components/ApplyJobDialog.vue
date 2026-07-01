<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          التقديم على وظيفة
        </h2>
        <p class="text-gray-600 text-sm">
          أنت على بعد خطوة واحدة من التقديم على هذه الوظيفة.
        </p>
      </div>

      <Form @submit="submitApplication" v-slot="{ errors }">
        <div class="p-6">
          <!-- CV Selection Section -->
          <div class="mb-8">
            <h3 class="text-sm font-semibold text-gray-700 mb-4 text-right">
              اختر السيرة الذاتية
            </h3>

            <div class="space-y-3">
              <label
                v-for="(cv, index) in cvs"
                :key="index"
                :for="'cv-' + index"
                class="cursor-pointer mb-3 block bg-bg-light rounded-lg border border-[#fff]/0 hover:border-primary active:bg-bg-subtle transition"
              >
                <div class="flex items-center p-4">
                  <input
                    type="radio"
                    :id="'cv-' + index"
                    :value="index"
                    v-model="formData.selectedCv"
                    class="ml-3 h-[12px] w-[12px] text-primary accent-danger focus:bg-primary active:bg-primary checked:bg-primary"
                  />
                  <div class="flex gap-2">
                    <span class="block p-2 bg-white rounded-xl">
                      <Pdf />
                    </span>
                    <div>
                      <span class="block text-slate-900 text-sm mb-1">
                        pdf السيرة الذاتية
                      </span>
                      <span class="text-xs block text-slate-400">
                        {{ cv.size }}
                      </span>
                    </div>
                  </div>
                </div>
              </label>
            </div>
          </div>

          <!-- Cover Letter Section -->
          <div class="mb-8">
            <label class="text-sm block mb-2"> خطاب تعريفي </label>
            <Field
              name="coverletter"
              label=" الخطاب التعريفي "
              id="coverletter"
              rules="required|min:50"
              v-model="formData.coverLetter"
            >
              <textarea
                v-model="formData.coverLetter"
                placeholder="أدخل خطاب تعريفي..."
                rows="5"
                class="text-sm w-full placeholder:text-xs bg-bg-light p-4 border border-[#fff]/0 rounded-lg focus:outline-none focus:border-primary transition"
              ></textarea>
            </Field>
            <ErrorMessage
              name="coverletter"
              class="text-xs text-red-500 block"
            />
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button
            @click="cancelApplication"
            type="button"
            class="btn-outline text-sm"
          >
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm">التقديم</button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { ErrorMessage, Field, Form } from "vee-validate";
import Dialog from "./Dialog.vue";
import Pdf from "./icons/pdf.vue";
const model = defineModel();

/** @type {{ cvs: Array<{id: number, name: string, size: string}> }} */
const props = defineProps({
  cvs: {
    type: Array,
    default: () => [],
  },
});

const formData = ref({
  selectedCv: 0,
  coverLetter: "",
});

const submitApplication = () => {
  // Handle form submission
  alert("تم تقديم الطلب بنجاح");
};

const cancelApplication = () => {
  // Reset form
  formData.value.selectedCv = 0;
  formData.value.coverLetter = "";
  model.value = false;
};
</script>

<style scoped></style>
