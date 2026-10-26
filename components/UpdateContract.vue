<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          تعديل العقد الوظيفي
        </h2>
        <p class="text-gray-600 text-sm" v-if="step === 'form'">
          حدّل ملف العقد أو مهلة التوقيع أو الملاحظات. سيتم إشعار الطرف الآخر بالتعديل.
        </p>
        <p class="text-gray-600 text-sm" v-else>
          راجع التعديلات قبل التأكيد — سيصبح العقد قابلاً للتوقيع مجدداً وسيُشعر الفرد.
        </p>
      </div>

      <!-- Step 1: form -->
      <Form @submit="goConfirm" v-slot="{ errors }" ref="form" v-if="step === 'form'">
        <div class="px-4 sm:px-6 mb-10">
          <div>
            <label for="contractFile" class="block text-sm mb-3">
              ملف العقد (اتركه فارغاً للإبقاء على الحالي: {{ contract?.fileName || 'عقد العمل' }})
            </label>
            <Field
              name="contractFile"
              id="contractFile"
              label="ملف العقد"
              v-model="formData.contractFile"
            >
              <FileInput
                accept=".pdf,.doc,.docx"
                @change="handleFileChange($event)"
              />
            </Field>
          </div>

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

          <div class="mt-5">
            <label for="notes" class="block text-sm mb-2">
              ملاحظات العقد
            </label>
            <Field
              name="notes"
              v-model="formData.notes"
              as="textarea"
              rows="3"
              placeholder="ملاحظات أو شروط إضافية (اختياري)"
              class="text-sm rounded-2xl w-full px-3 py-3 bg-bg-light border border-primary/0 focus:border-primary outline-none placeholder:text-xs transition duration-300 resize-none"
            />
          </div>
        </div>

        <div class="p-4 sm:p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light">
          <button type="button" @click="close" class="btn-outline text-sm">
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm">
            مراجعة التعديل
          </button>
        </div>
      </Form>

      <!-- Step 2: confirm -->
      <div v-else>
        <div class="px-4 sm:px-6 py-6 space-y-3 text-sm">
          <div class="flex justify-between gap-4">
            <span class="text-muted">المهلة الجديدة</span>
            <span class="font-bold">{{ formData.endDate }}</span>
          </div>
          <div class="flex justify-between gap-4">
            <span class="text-muted">الملف</span>
            <span class="font-bold">{{ formData.contractFile ? formData.contractFile.name : 'الإبقاء على الحالي' }}</span>
          </div>
          <div class="flex justify-between gap-4" v-if="formData.notes">
            <span class="text-muted">ملاحظات</span>
            <span class="font-bold text-left max-w-[60%]">{{ formData.notes }}</span>
          </div>
        </div>
        <div class="p-4 sm:p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light">
          <button type="button" @click="step = 'form'" class="btn-outline text-sm" :disabled="isSubmitting">
            رجوع
          </button>
          <button type="button" @click="confirmUpdate" class="btn-primary text-sm" :disabled="isSubmitting">
            <span v-if="isSubmitting">جاري الحفظ...</span>
            <span v-else>تأكيد التعديل</span>
          </button>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script setup>
import { ref, reactive, watch } from "vue";
import { defineRule, Field, Form } from "vee-validate";
import Dialog from "./Dialog.vue";
import FileInput from "./elements/FileInput.vue";
import TextInput from "./elements/TextInput.vue";
import { toDateInputValue } from "~/services/help";

const model = defineModel();
const props = defineProps({ contract: { type: Object, default: () => ({}) } });
const emit = defineEmits(["updated"]);

const form = ref(null);
const step = ref("form");
const isSubmitting = ref(false);

defineRule("validateEndDate", (value) => {
  if (!value) return "حدد تاريخ انتهاء صلاحية التوقيع";

  // Same 24h rule as send (mirrors the backend): end of picked day, strictly > 24h out.
  const endDate = new Date(`${value}T23:59:59`);
  if (endDate.getTime() - Date.now() <= 24 * 60 * 60 * 1000)
    return "يجب أن يكون تاريخ الانتهاء بعد 24 ساعة على الأقل";

  return true;
});

const formData = reactive({
  contractFile: null,
  notes: "",
  endDate: "",
});

watch(() => props.contract, (c) => {
  step.value = "form";
  formData.contractFile = null;
  formData.notes = c?.notes || "";
  formData.endDate = c?.endDate ? toDateInputValue(c.endDate) : "";
}, { immediate: true });

const handleFileChange = (event) => {
  formData.contractFile = event.target.files[0];
};

const close = () => {
  form.value?.resetForm();
  model.value = false;
};

const goConfirm = () => {
  step.value = "confirm";
};

const confirmUpdate = async () => {
  isSubmitting.value = true;
  try {
    const payload = new FormData();
    if (formData.contractFile) payload.append("contractFile", formData.contractFile);
    if (formData.notes) payload.append("notes", formData.notes);
    if (formData.endDate) payload.append("endDate", new Date(formData.endDate).toISOString());

    const { error } = await useApi().put(`/contracts/${props.contract.id}`, payload);
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تم تحديث العقد بنجاح", "success");
    emit("updated");
    model.value = false;
  } catch {
    useToast().show("حدث خطأ أثناء تحديث العقد", "error");
  } finally {
    isSubmitting.value = false;
  }
};
</script>
