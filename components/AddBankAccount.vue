<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          {{ editing ? "تعديل الحساب البنكي" : "إضافة حساب بنكي" }}
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أضف بيانات حسابك البنكي لعرضها ضمن ملفك الشخصي. تساعد الجهات في تجهيز
          مستحقاتك المالية عند التعاقد.
        </p>
      </div>
      <Form @submit="submitBankAccount" v-slot="{ errors }">
        <div class="p-6">
          <div class="mb-6">
            <TextInput
              name="iban"
              id="iban"
              label="رقم الآيبان"
              required
              rules="required"
              placeholder="(مثال: SA0380000000608010167515)"
              v-model="formData.iban"
              :error="errors.iban"
            />
          </div>

          <div>
            <TextInput
              name="bankName"
              id="bankName"
              label="اسم البنك"
              required
              rules="required"
              placeholder="(مثال: البنك الأهلي السعودي)"
              v-model="formData.bankName"
              :error="errors.bankName"
            />
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
import { Form } from "vee-validate";
import TextInput from "./elements/TextInput.vue";

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
        iban: editing.iban || "",
        bankName: editing.bankName || "",
      };
    } else {
      formData.value = {};
    }
  }
);

const submitBankAccount = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true;

  const payload = {
    iban: formData.value.iban,
    bankName: formData.value.bankName,
  };

  try {
    const { error } = props.editing
      ? await useApi().put(`/bank-accounts/${props.editing.id}`, payload)
      : await useApi().post("/bank-accounts", payload);

    if (error) {
      useToast().show(error, "error");
      return;
    }

    useToast().show(
      props.editing ? "تم تعديل الحساب البنكي بنجاح" : "تم إضافة الحساب البنكي بنجاح",
      "success"
    );
    model.value = false;
    formData.value = {};
    emit("saved");
  } catch {
    useToast().show(
      props.editing ? "حدث خطأ أثناء تعديل الحساب البنكي" : "حدث خطأ أثناء إضافة الحساب البنكي",
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