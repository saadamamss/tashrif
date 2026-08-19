<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          تعديل الملف الشخصي
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أكمل معلوماتك الشخصية لمساعدتنا في ترشيح الوظائف الأنسب لك وزيادة فرص
          قبولك لدى الجهات.
        </p>
      </div>
      <Form @submit="submit" v-slot="{ errors }">
        <div class="p-6">
          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="name"
                id="name"
                label="الاسم الكامل"
                required
                rules="required"
                placeholder="الاسم الكامل"
                v-model="formData.name"
                :error="errors.name"
              />
            </div>
            <div class="flex-1">
              <label class="block text-sm mb-2">البريد الإلكتروني</label>
              <div
                class="text-sm rounded-2xl w-full h-[38px] px-2 bg-bg-light flex items-center text-gray-400"
              >
                {{ formData.email }}
              </div>
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="phone"
                id="phone"
                label="رقم الجوال"
                required
                rules="required"
                placeholder="05xxxxxxxx"
                v-model="formData.phone"
                :error="errors.phone"
              />
            </div>
            <div class="flex-1">
              <label class="text-sm mb-2 block">
                الجنس <span class="text-red-500">*</span>
              </label>
              <Field
                id="gender"
                name="gender"
                label="الجنس"
                rules="required"
                v-model="formData.gender"
              >
                <CustomSelect
                  name="gender"
                  :items="genderOptions"
                  placeholder="الجنس"
                  v-model="formData.gender"
                  :error="errors.gender"
                />
              </Field>
              <ErrorMessage
                name="gender"
                class="block text-red-500 text-xs mt-2"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="nationality"
                id="nationality"
                label="الجنسية"
                placeholder="الجنسية"
                v-model="formData.nationality"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="birthDate"
                id="birthDate"
                type="date"
                label="تاريخ الميلاد"
                v-model="formData.birthDate"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="city"
                id="city"
                label="المدينة"
                placeholder="المدينة"
                v-model="formData.city"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="zone"
                id="zone"
                label="المنطقة"
                placeholder="المنطقة"
                v-model="formData.zone"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="district"
                id="district"
                label="الحي"
                placeholder="الحي"
                v-model="formData.district"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="street"
                id="street"
                label="الشارع"
                placeholder="الشارع"
                v-model="formData.street"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="zipcode"
                id="zipcode"
                label="الرمز البريدي"
                placeholder="الرمز البريدي"
                v-model="formData.zipcode"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="jobTitle"
                id="jobTitle"
                label="المسمى الوظيفي"
                placeholder="(مثال: مشرف ميداني، مسؤول تنظيم...)"
                v-model="formData.jobTitle"
              />
            </div>
          </div>
        </div>
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button type="button" @click="cancel" class="btn-outline text-sm">
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm" :disabled="isSubmitting">
            {{ isSubmitting ? "جارٍ الحفظ..." : "حفظ" }}
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

const model = defineModel();
const emit = defineEmits(["saved"]);

const genderOptions = [
  { value: "male", label: "ذكر" },
  { value: "female", label: "أنثى" },
];

const formData = ref({ email: "" });
const isSubmitting = ref(false);

watch(model, async (open) => {
  if (!open) return;
  try {
    const { data, error } = await useApi().get("/individuals/profile");
    if (!error && data) {
      formData.value = {
        name: data.name || "",
        email: data.email || "",
        phone: data.phone || "",
        gender: data.gender || "",
        nationality: data.nationality || "",
        birthDate: toDateInputValue(data.birthDate),
        city: data.city || "",
        zone: data.zone || "",
        district: data.district || "",
        street: data.street || "",
        zipcode: data.zipcode || "",
        jobTitle: data.jobTitle || "",
      };
    }
  } catch {
    // leave form empty on failure
  }
});

const submit = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true;

  const payload = {
    name: formData.value.name,
    phone: formData.value.phone,
    gender: formData.value.gender,
    nationality: formData.value.nationality,
    birthDate: formData.value.birthDate || null,
    city: formData.value.city,
    zone: formData.value.zone,
    district: formData.value.district,
    street: formData.value.street,
    zipcode: formData.value.zipcode,
    jobTitle: formData.value.jobTitle,
  };

  try {
    const { error } = await useApi().put("/individuals/profile", payload);

    if (error) {
      useToast().show(error, "error");
      return;
    }

    useToast().show("تم تحديث الملف الشخصي بنجاح", "success");
    model.value = false;
    emit("saved");
  } catch {
    useToast().show("حدث خطأ أثناء تحديث الملف الشخصي", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const cancel = () => {
  model.value = false;
};
</script>