<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-[#f5f5f5] p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          إضافة مؤهل علمي جديد
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أدخل بيانات مؤهلك العلمي بدقة لعرضها ضمن ملفك الشخصي. تساعد المؤهلات
          الجهات في تقييم مدى توافقك مع الوظائف المتاحة.
        </p>
      </div>

      <Form @submit="addQualification" v-slot="{ errors }">
        <div class="p-6">
          <!-- Qualification Type Section -->
          <div class="mb-6">
            <div class="">
              <label class="text-sm mb-2 block">
                نوع المؤهل <span class="text-red-500">*</span>
              </label>
              <Field
                id="qualificationType"
                name="qualificationType"
                label="نوع المؤهل"
                rules="required"
                v-model="formData.qualificationType"
              >
                <CustomSelect
                  :items="qualificationTypes"
                  placeholder="اختر"
                  :error="errors.qualificationType"
                  v-model="formData.qualificationType"
                />
              </Field>
              <ErrorMessage
                name="qualificationType"
                class="block text-red-500 text-xs mt-2"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <!-- Educational Institution Section -->
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                التخصص الدراسى <span class="text-red-500">*</span>
              </label>
              <Field
                id="specialization"
                name="specialization"
                label="التخصص الدراسى"
                rules="required"
                v-model="formData.specialization"
              >
                <CustomSelect
                  :items="specializations"
                  placeholder="(مثال: إدارة أعمال، هندسة، علوم الحاسب...)"
                  v-model="formData.specialization"
                  :error="errors.specialization"
                />
              </Field>
              <ErrorMessage
                name="specialization"
                class="block text-red-500 text-xs mt-2"
              />
            </div>
            <!-- Educational Institution Section -->
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                الجهة التعليمية <span class="text-red-500">*</span>
              </label>
              <Field
                id="institution"
                name="institution"
                label="الجهة التعليمية "
                rules="required"
                v-model="formData.institution"
              >
                <CustomSelect
                  :items="institutions"
                  placeholder="اختر   الجهة التعليمية "
                  v-model="formData.institution"
                  :error="errors.institution"
                />
              </Field>
              <ErrorMessage
                name="institution"
                class="block text-red-500 text-xs mt-2"
              />
            </div>
          </div>
          <div class="flex gap-6 mb-6">
            <!-- Grade Section -->
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                التقدير <span class="text-red-500">*</span>
              </label>
              <Field
                id="grade"
                name="grade"
                label="التقدير"
                rules="required"
                v-model="formData.grade"
              >
                <CustomSelect
                  :items="grades"
                  placeholder="اختر التقدير"
                  v-model="formData.grade"
                  :error="errors.grade"
                />
              </Field>
              <ErrorMessage
                name="grade"
                class="block text-red-500 text-xs mt-2"
              />
            </div>

            <!-- Graduation Year Section -->
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                سنة التخرج <span class="text-red-500">*</span>
              </label>
              <Field
                id="graduationYear"
                name="graduationYear"
                label=" سنة التخرج "
                rules="required"
                v-model="formData.graduationYear"
              >
                <CustomSelect
                  :items="graduationYears"
                  placeholder="اختر سنة التخرج "
                  v-model="formData.graduationYear"
                  :error="errors.graduationYear"
                />
              </Field>
              <ErrorMessage
                name="graduationYear"
                class="block text-red-500 text-xs mt-2"
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
            @click="cancelQualification"
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

const specializations = ref(["إدارة أعمال", "هندسة", "علوم حاسب"]);
const model = defineModel();
const qualificationTypes = ref([
  "بكلوريوس",
  "دبلوم",
  "ثانوية عامة",
  "شهادة مهنية",
]);

const institutions = ref([
  "جامعة الملك سعود",
  "جامعة الملك فهد للبترول والمعادن",
  "جامعة الملك عبدالعزيز",
  "جامعة الأميرة نورة",
]);

const grades = ref(["ممتاز", "جيد جداً", "جيد", "مقبول"]);

const graduationYears = ref(
  Array.from({ length: 30 }, (_, i) => new Date().getFullYear() - i)
);

const formData = ref({});

const addQualification = () => {
  alert("تم تقديم الطلب بنجاح");
};

const cancelQualification = () => {
  model.value = false;
};
</script>
