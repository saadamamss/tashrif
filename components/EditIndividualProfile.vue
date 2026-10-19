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
          <!-- Avatar Upload -->
          <div class="flex justify-center mb-6">
            <div class="relative group">
              <div class="w-24 h-24 rounded-full overflow-hidden border-2 border-gray-200">
                <img
                  :src="avatarPreview || defaultAvatar"
                  class="w-full h-full object-cover"
                  alt="الصورة الشخصية"
                />
              </div>
              <label
                class="absolute inset-0 flex items-center justify-center bg-black/40 rounded-full opacity-0 group-hover:opacity-100 cursor-pointer transition"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                  <path stroke-linecap="round" stroke-linejoin="round" d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <input
                  type="file"
                  accept="image/*"
                  class="hidden"
                  @change="handleAvatarChange"
                />
              </label>
            </div>
          </div>

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
import { toDateInputValue, buildImageUrl } from "~/services/help";

const model = defineModel();
const emit = defineEmits(["saved"]);

const defaultAvatar = "/images/avatar.png";

const genderOptions = [
  { value: "male", label: "ذكر" },
  { value: "female", label: "أنثى" },
];

const formData = ref({ email: "" });
const isSubmitting = ref(false);
const avatarFile = ref(null);
const avatarPreview = ref(null);

watch(model, async (open) => {
  if (!open) return;
  avatarFile.value = null;
  avatarPreview.value = null;
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
      if (data.avatarUrl) {
        avatarPreview.value = buildImageUrl(data.avatarUrl);
      }
    }
  } catch {
    // leave form empty on failure
  }
});

const handleAvatarChange = async (event) => {
  const file = event.target.files[0];
  if (!file) return;

  if (!file.type.startsWith("image/")) {
    useToast().show("يرجى اختيار صورة", "error");
    return;
  }

  if (file.size > 5 * 1024 * 1024) {
    useToast().show("حجم الصورة يتجاوز 5 ميجابايت", "error");
    return;
  }

  // Show preview immediately
  const reader = new FileReader();
  reader.onload = (e) => {
    avatarPreview.value = e.target.result;
  };
  reader.readAsDataURL(file);

  // Upload to server
  const formDataUpload = new FormData();
  formDataUpload.append("file", file);

  try {
    const { error } = await useApi().put("/individuals/profile/avatar", formDataUpload);
    if (error) {
      useToast().show(error, "error");
      // Revert preview on error
      const { data } = await useApi().get("/individuals/profile");
      if (data?.avatarUrl) {
        avatarPreview.value = buildImageUrl(data.avatarUrl);
      } else {
        avatarPreview.value = null;
      }
      return;
    }
    useToast().show("تم تحديث الصورة بنجاح", "success");
    avatarFile.value = file;
  } catch {
    useToast().show("حدث خطأ أثناء رفع الصورة", "error");
  }
};

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