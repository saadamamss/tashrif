<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          {{ editing ? "تعديل خبرة" : "إضافة الخبرات" }}
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أضف خبراتك السابقة في العمل، سواء كانت موسمية أو دائمة. تساعد الخبرات
          الجهات في التعرف على مهاراتك العملية ومدى جاهزيتك للوظائف المعروضة.
        </p>
      </div>
      <Form @submit="submitExperience" v-slot="{ errors }">
        <div class="p-6">
          <div class="mb-6">
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

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="employer"
                id="employer"
                label="اسم الجهة"
                required
                rules="required"
                placeholder="(مثال: شركة الإسناد الموسمي)"
                v-model="formData.employer"
                :error="errors.employer"
              />
            </div>
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
                  :items="jobLocations"
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
            <div class="flex-1">
              <TextInput
                name="jobStart"
                id="jobStart"
                type="date"
                rules="required"
                label=" فترة العمل من "
                required
                placeholder=" فترة العمل من "
                v-model="formData.startDate"
                :error="errors.jobStart"
              />
            </div>

            <div class="flex-1">
              <TextInput
                name="jobEnd"
                id="jobEnd"
                type="date"
                label=" فترة العمل إلى "
                placeholder=" فترة العمل إلى "
                v-model="formData.endDate"
              />
            </div>
          </div>
        </div>
        <!-- Action Buttons -->
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button
            type="button"
            @click="cancel"
            class="btn-outline text-sm"
          >
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm" :disabled="isSubmitting">
            {{
              editing
                ? isSubmitting
                  ? "جارٍ الحفظ..."
                  : "حفظ"
                : isSubmitting
                  ? "جارٍ الإضافة..."
                  : "إضافة"
            }}
          </button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { ErrorMessage, Field, Form } from "vee-validate";
import CustomSelect from "./elements/CustomSelect.vue";
import TextInput from "./elements/TextInput.vue";
import { toDateInputValue } from "~/services/help";

const jobLocations = ref(["الرياض", "مكة", "المدينة", "جدة"]);
const model = defineModel();
const props = defineProps({
  editing: { type: Object, default: null },
});
const emit = defineEmits(["saved"]);

const formData = ref({});
const isSubmitting = ref(false);

watch(
  () => props.editing,
  (editing) => {
    if (editing) {
      formData.value = {
        jobTitle: editing.jobTitle,
        employer: editing.employer,
        jobLocation: editing.location,
        startDate: toDateInputValue(editing.startDate),
        endDate: toDateInputValue(editing.endDate),
      };
    } else {
      formData.value = {};
    }
  }
);

const submitExperience = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true;

  const isCurrent = !formData.value.endDate;
  const duration = isCurrent
    ? `من ${formData.value.startDate || ""} حتى الآن`
    : `من ${formData.value.startDate || ""} إلى ${formData.value.endDate || ""}`;

  const payload = {
    jobTitle: formData.value.jobTitle,
    employer: formData.value.employer,
    location: formData.value.jobLocation,
    duration,
    isCurrent,
    startDate: formData.value.startDate,
    endDate: formData.value.endDate,
  };

  try {
    const { error } = props.editing
      ? await useApi().put(`/experiences/${props.editing.id}`, payload)
      : await useApi().post("/experiences", payload);

    if (error) {
      useToast().show(error, "error");
      return;
    }

    useToast().show(
      props.editing ? "تم تعديل الخبرة بنجاح" : "تم إضافة الخبرة بنجاح",
      "success"
    );
    model.value = false;
    formData.value = {};
    emit("saved");
  } catch {
    useToast().show(
      props.editing ? "حدث خطأ أثناء تعديل الخبرة" : "حدث خطأ أثناء إضافة الخبرة",
      "error"
    );
  } finally {
    isSubmitting.value = false;
  }
};

const cancel = () => {
  formData.value = {};
  model.value = false;
};
</script>