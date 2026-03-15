<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-[#f5f5f5] p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          إضافة الخبرات
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أضف خبراتك السابقة في العمل، سواء كانت موسمية أو دائمة. تساعد الخبرات
          الجهات في التعرف على مهاراتك العملية ومدى جاهزيتك للوظائف المعروضة.
        </p>
      </div>
      <Form @submit="submitApplication" v-slot="{ errors }">
        <div class="p-6">
          <!-- Qualification Type Section -->
          <div class="mb-6">
            <div class="">

              <TextInput
                name="jobTitle"
                id="jobTitle"
                label="المسمى الوظيفى"
                required
                rules="required"
                placeholder="(مثال: مشرف ميداني، مسؤول تنظيم، مندوب توجيه...)"
                v-model="formData.jobTitle"
                :error="errors.jobTitle"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <!-- Educational Institution Section -->
            <div class="flex-1">
              <TextInput
                name="entityName"
                id="entityName"
                label="اسم الجهة"
                required
                rules="required"
                placeholder="(مثال: شركة الإسناد الموسمي)"
                v-model="formData.entityName"
                :error="errors.entityName"
              />
            </div>
            <!-- Educational Institution Section -->
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                موقع العمل <span class="text-red-500">*</span>
              </label>
              <Field
                id="jobLocation"
                name="jobLocation"
                label="موقع العمل "
                rules="required"
                v-model="formData.jobLocation"
              >
                <CustomSelect
                  :items="jobLocation"
                  placeholder=" اختر موقع العمل"
                  v-model="formData.jobLocation"
                  :error="errors.jobLocation"
                />
              </Field>
              <ErrorMessage
                name="jobLocation"
                class="block text-red-500 text-xs mt-2"
              />
            </div>
          </div>
          <div class="flex gap-6 mb-6">
            <!-- Grade Section -->
            <div class="flex-1">
              <TextInput
                name="jobStart"
                id="jobStart"
                rules="required"
                label=" فترة العمل من "
                required
                placeholder=" فترة العمل من "
                v-model="formData.from"
                :error="errors.jobStart"
              />
            </div>

            <!-- Graduation Year Section -->
            <div class="flex-1">
              <TextInput
                name="jobEnd"
                id="jobEnd"
                rules="required"
                label=" فترة العمل إلى "
                required
                placeholder=" فترة العمل إلى "
                v-model="formData.to"
                :error="errors.jobEnd"
              />
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
          <button type="submit" class="btn-primary text-sm">إضافة</button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { ErrorMessage, Field, Form } from "vee-validate";
import CustomSelect from "./elements/custom-select.vue";
import TextInput from "./elements/text-input.vue";

const jobLocation = ref(["الرياض", "مكة", "المدينة", "جدة"]);
const selectedJobLocation = ref("");
const model = defineModel();

const formData = ref({});
const submitApplication = () => {
  alert("تم تقديم الطلب بنجاح");
};

const cancelApplication = () => {
  model.value = false;
};
</script>
