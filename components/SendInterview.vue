<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-[#f5f5f5] p-4 sm:p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          إجراء المقابلة الشخصية
        </h2>
        <p class="text-gray-600 text-sm">
          قم بتحديد مواعيد المقابلة مع المتقدم.
        </p>
      </div>

      <div class="p-4 sm:p-6">
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
        <div class="px-4 sm:px-6 mb-6">
          <!-- Interview Form -->
          <div class="space-y-6">
            <div>
              <label class="text-sm mb-2 block">طريقة المقابلة</label>
              <Field
                name="method"
                v-slot="{ field }"
                v-model="formData.method"
                rules="validateMethod"
              >
                <div class="flex gap-4">
                  <div class="flex-1">
                    <label
                      class="flex gap-2 items-center rounded-xl px-3 py-3 h-[48px] bg-[#f5f5f5] text-sm active:bg-[#f8f9f9] transition cursor-pointer"
                    >
                      <input
                        type="radio"
                        v-bind="field"
                        value="inperson"
                        v-model="formData.method"
                        @change="clearLocationFields"
                      />
                      <span class="text-sm text-gray-700">حضوريا</span>
                    </label>
                  </div>
                  <div class="flex-1">
                    <label
                      class="flex gap-2 items-center rounded-xl px-3 py-3 h-[48px] bg-[#f5f5f5] text-sm active:bg-[#f8f9f9] transition cursor-pointer"
                    >
                      <input
                        type="radio"
                        v-bind="field"
                        value="remote"
                        v-model="formData.method"
                        @change="clearLocationFields"
                      />

                      <span class="text-sm text-gray-700">عن بعد</span>
                    </label>
                  </div>
                </div>
              </Field>
              <ErrorMessage
                name="method"
                class="text-xs block text-red-500 mt-1"
              />
            </div>
            <div class="flex gap-4">
              <div class="flex-1">
                <TextInput
                  name="date"
                  id="interview-date"
                  type="date"
                  label="التاريخ"
                  height="48"
                  placeholder="التاريخ"
                  v-model="formData.date"
                  rules="required"
                  :error="errors.date"
                />
              </div>
              <div class="flex-1">
                <TextInput
                  name="time"
                  id="interview-time"
                  type="time"
                  height="48"
                  label="الوقت"
                  placeholder="الوقت"
                  v-model="formData.time"
                  rules="required"
                  :error="errors.time"
                />
              </div>
            </div>

            <div class="">
              <TextInput
                v-if="formData.method === 'inperson'"
                name="location"
                id="interview-place"
                type="text"
                height="48"
                required
                label="الموقع"
                placeholder="مثال: مكتب الجهة – العزيزية"
                v-model="formData.location"
                rules="validateLocation"
                :error="errors.location"
              />

              <TextInput
                v-if="formData.method === 'remote'"
                name="link"
                id="interview-link"
                required
                type="text"
                height="48"
                label="رابط المقابلة"
                placeholder="https://example.com/meeting"
                v-model="formData.link"
                rules="validateLink"
                :error="errors.link"
              />
            </div>

            <div>
              <label for="notes" class="text-sm mb-2 block">
                ملاحظات للمتقدم
              </label>
              <textarea
                name="notes"
                id="notes"
                v-model="formData.notes"
                placeholder="أدخل ملاحظاتك للمتقدم ..."
                rows="5"
                class="text-sm w-full p-4 placeholder:text-xs border border-[#fff]/0 bg-[#f5f5f5] rounded-lg focus:outline-none focus:border-[#ecb42b] transition"
              ></textarea>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-[#f5f5f5]"
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
            <span v-else>إرسال</span>
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
import TextInput from "./elements/TextInput.vue";
import ApplicantCard from "./ApplicantCard.vue";

const model = defineModel();

/** @type {{ applicants: Array<import('~/types/application').Application> }} */
const props = defineProps(["applicants"]);
const form = ref(null);
const isSubmitting = ref(false);
defineRule("validateMethod", (value) => {
  if (!value) return "يلزم تحديد طريقة المقابلة ";
  return true;
});
defineRule("validateLocation", (value) => {
  if (!value && formData.method == "inperson")
    return "الموقع مطلوب للمقابلة الحضورية";
  return true;
});
defineRule("validateLink", (value) => {
  if (formData.method === "remote") {
    if (!value) return "رابط المقابلة مطلوب";
    if (!/^https?:\/\/.+\..+/.test(value)) return "يجب أن يكون الرابط صحيحاً";
  }
  return true;
});

// Form data structure
const formData = reactive({
  method: "",
  date: "",
  time: "",
  location: "",
  link: "",
  notes: "",
  applicantIds: props.applicants.map((applicant) => applicant.id), // Assuming applicants have IDs
});

// Clear location fields when method changes
const clearLocationFields = () => {
  formData.location = "";
  formData.link = "";
};

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

    emit("submit-interview", formPayload);
    resetForm();
    model.value = false;
  } catch (error) {
    console.error("Submission error:", error);
    alert("حدث خطأ أثناء محاولة حفظ البيانات");
  } finally {
    isSubmitting.value = false;
  }
};

const emit = defineEmits(["submit-interview"]);

const cancelApplication = () => {
  form.value.resetForm();
  model.value = false;
};
</script>
