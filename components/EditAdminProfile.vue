<script setup>
import { ErrorMessage, Field, Form } from "vee-validate";
import CustomSelect from "./elements/CustomSelect.vue";
import TextInput from "./elements/TextInput.vue";

const model = defineModel();
const emit = defineEmits(["saved"]);

const genderOptions = [
  { value: "male", label: "ذكر" },
  { value: "female", label: "أنثى" },
];

const formData = ref({
  name: "",
  email: "",
  phone: "",
  gender: "",
  nationality: "",
});
const isSubmitting = ref(false);
const { user } = useAuth();

watch(model, (open) => {
  if (!open) return;
  formData.value = {
    name: user.value?.name || "",
    email: user.value?.email || "",
    phone: user.value?.phone || "",
    gender: user.value?.gender || "",
    nationality: user.value?.nationality || "",
  };
});

const submit = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true;

  try {
    // Returns the flat UserDto — parent calls setUser() with it
    const { data, error } = await useApi().put("/admin/profile", {
      name: formData.value.name,
      email: formData.value.email,
      phone: formData.value.phone,
      gender: formData.value.gender,
      nationality: formData.value.nationality,
    });

    if (error) {
      useToast().show(error, "error");
      return;
    }

    useToast().show("تم تحديث الملف الشخصي بنجاح", "success");
    emit("saved", data);
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

<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          تعديل الملف الشخصي
        </h2>
        <p class="text-gray-600 text-sm text-right">
          حدّث معلوماتك الشخصية. رقم الهوية لا يمكن تعديله.
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
              <TextInput
                name="email"
                id="email"
                type="email"
                label="البريد الإلكتروني"
                required
                rules="required|email"
                placeholder="example@email.com"
                v-model="formData.email"
                :error="errors.email"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="phone"
                id="phone"
                label="رقم الجوال"
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
              <ErrorMessage name="gender" class="block text-red-500 text-xs mt-2" />
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
              <label class="block text-sm mb-2">رقم الهوية</label>
              <div
                class="text-sm rounded-2xl w-full h-[38px] px-2 bg-bg-light flex items-center text-gray-400"
              >
                {{ user?.nationalId }}
              </div>
            </div>
          </div>
        </div>
        <div class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light">
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
