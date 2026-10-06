<script setup>
useHead({
  title: 'إنشاء حساب فرد',
  meta: [
    { name: "description", content: "إنشاء حساب فرد جديد في منصة تشريف للتوظيف الموسمي. قدم على الوظائف الموسمية في الحج والعمرة." },
  ],
  bodyAttrs: {
    class: "register-page",
  },
});

definePageMeta({
  middleware:["auth"],
  meta: { guest: true },
})

import Dialog from "~/components/Dialog.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import FileInput from "~/components/elements/FileInput.vue";
import TextInput from "~/components/elements/TextInput.vue";
import OTPDialog from "~/components/OtpDialog.vue";
import { Field, Form, ErrorMessage, defineRule } from "vee-validate";
import { required, email, min, numeric, confirmed, length } from "@vee-validate/rules";
import PhoneInput from "~/components/PhoneInput.vue";

// Define validation rules
defineRule("required", required);
defineRule("email", email);
defineRule("min", min);
defineRule("numeric", numeric);
defineRule("length", length);
defineRule("confirmed", confirmed);

const route = useRoute();
const showOTPDialog = ref(false);
if (route.query.otp) {
  showOTPDialog.value = true;
}

// Initialize formData with all fields
const formData = ref({
  firstName: "",
  lastName: "",
  nationalId: "",
  password: "",
  confirmPassword: "",
  phone: "",
  email: "",
  gender: "",
  nationality: "",
  idFile: null,
});

const handleFileChange = (field, event) => {
  formData.value[field] = event.target.files[0];
};
const phoneValid = ref(false);
//
defineRule("phonevalidation", (value) => {
  return phoneValid.value ? true : "الرقم غير صحيح";
});
const handlePhoneValidation = (validation) => {
  phoneValid.value = validation.valid;
};

const handleCountryChange = (country) => {
  console.log("Country changed:", country);
};
const router = useRouter();
const isSubmitting = ref(false);
const onSubmit = async (values) => {
  isSubmitting.value = true;
  try {
    const payload = new FormData()
    Object.entries(values).forEach(([key, val]) => {
      if (val) payload.append(key, val)
    })
    if (formData.value.idFile) payload.append('idFile', formData.value.idFile)
    const { error } = await useApi().post('/auth/register', payload)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم إنشاء الحساب بنجاح", "success")
    router.push("/register/success")
  } finally {
    isSubmitting.value = false
  }
};
</script>

<template>
  <div class="py-24 px-4">
    <div
      class="card p-6 md:p-8 lg:p-10 max-w-[846px] mx-auto bg-white rounded-2xl"
    >
      <div class="header">
        <h1 class="text-xl lg:text-2xl font-bold text-dark mb-3">
          إنشاء حساب جديد
        </h1>
        <p class="text-sm lg:text-base text-dark">
          املأ النموذج أدناه ليتم تسجيل حساب جديد والانضمام إلى منصة تشريف.
        </p>
      </div>
      <div class="py-4">
        <hr />
      </div>
      <div class="form">
        <Form @submit="onSubmit" v-slot="{ errors }">
          <div>
            <div class="w-full flex gap-4 md:gap-6 mb-6">
              <div class="flex-1">
                <TextInput
                  id="firstName"
                  name="firstName"
                  :error="errors.firstName"
                  placeholder="الإسم الأول"
                  label="الإسم الأول"
                  required
                  rules="required|min:2"
                  v-model="formData.firstName"
                />
              </div>

              <div class="flex-1">
                <TextInput
                  id="lastName"
                  name="lastName"
                  required
                  rules="required|min:2"
                  label="الإسم الثانى"
                  placeholder="الإسم الثانى"
                  v-model="formData.lastName"
                  :error="errors.lastName"
                />
              </div>
            </div>
            <div class="mb-6">
              <TextInput
                name="nationalId"
                id="nationalId"
                required
                rules="required|numeric|length:10"
                label="رقم الهوية الوطنية أو الإقامة"
                placeholder="رقم الهوية الوطنية أو الإقامة"
                v-model="formData.nationalId"
                :error="errors.nationalId"
              />
            </div>
            <div class="w-full flex gap-4 md:gap-6 mb-6">
              <div class="flex-1">
                <TextInput
                  name="password"
                  id="password"
                  type="password"
                  required
                  rules="required|min:8"
                  label="كلمة المرور"
                  placeholder="أدخل كلمة مرور قوية (8 أحرف على الأقل)"
                  v-model="formData.password"
                  :error="errors.password"
                />
              </div>
              <div class="flex-1">
                <TextInput
                  name="confirmPassword"
                  id="confirmPassword"
                  type="password"
                  required
                  rules="required|confirmed:@password"
                  label="تأكيد كلمة المرور"
                  placeholder="أعد إدخال كلمة المرور"
                  v-model="formData.confirmPassword"
                  :error="errors.confirmPassword"
                />
              </div>
            </div>
            <div class="w-full flex-col sm:flex-row flex gap-4 md:gap-6 mb-6">
              <div class="flex-1">
                <label for="phone" class="block text-sm mb-2">
                  رقم الجوال <span class="text-red-400">*</span>
                </label>
            
                <Field
                  id="phone"
                  name="phone"
                  rules="required|phonevalidation"
                  v-model="formData.phone"
                  label="رقم الجوال"
                >
                  <PhoneInput
                    v-model="formData.phone"
                    @validation="handlePhoneValidation"
                    @country-change="handleCountryChange"
                  />
                </Field>
                <ErrorMessage
                  name="phone"
                  class="block text-xs text-red-500 mt-1"
                />
              </div>

              <div class="flex-1">
                <TextInput
                  name="email"
                  id="email"
                  required
                  rules="required|email"
                  v-model="formData.email"
                  label="البريد الإلكترونى"
                  placeholder="البريد الإلكترونى"
                  :error="errors.email"
                />
              </div>
            </div>
            <div class="w-full flex gap-4 md:gap-6 mb-6">
              <div class="flex-1">
                <label for="gender" class="block text-sm mb-2">
                  الجنس <span class="text-red-400">*</span>
                </label>
                <Field
                  name="gender"
                  id="gender"
                  required
                  rules="required"
                  label="الجنس"
                  class="border"
                  v-model="formData.gender"
                >
                  <CustomSelect
                    name="gender"
                    placeholder="الجنس"
                    :items="[{ value: 'male', label: 'ذكر' }, { value: 'female', label: 'أنثى' }]"
                    :error="errors.gender"
                    v-model="formData.gender"
                  />
                </Field>

                <ErrorMessage name="gender" class="text-red-500 text-xs mt-1" />
              </div>

              <div class="flex-1">
                <TextInput
                  name="nationality"
                  id="nationality"
                  required
                  rules="required"
                  v-model="formData.nationality"
                  label="الجنسية"
                  placeholder="الجنسية"
                  :error="errors.nationality"
                />
              </div>
            </div>
            <div class="w-full flex flex-col md:flex-row gap-4 md:gap-6 mb-6">
              <div class="flex-1">
                <label for="idFile" class="block text-sm mb-2">
                  صورة الهوية الوطنية/الإقامة
                  <span class="text-red-400">*</span>
                </label>

                <Field
                  name="idFile"
                  required
                  rules="required"
                  label="صورة الهوية الوطنية/الإقامة"
                  v-model="formData.idFile"
                  id="idFile"
                >
                  <FileInput
                    required
                    accept="image/*"
                    :error="errors.idFile"
                    @change="handleFileChange('idFile', $event)"
                  />
                </Field>
                <ErrorMessage name="idFile" class="text-red-500 text-xs mt-1" />
              </div>
            </div>

            <div class="mt-10 flex justify-end">
              <button type="submit" class="btn-primary text-sm py-3 px-10" :disabled="isSubmitting">
                {{ isSubmitting ? 'جاري إنشاء الحساب...' : 'إنشاء حساب' }}
              </button>
            </div>
          </div>
        </Form>
      </div>
    </div>

    <Dialog v-if="showOTPDialog" v-model="showOTPDialog">
      <OTPDialog @cancel="showOTPDialog = false" />
    </Dialog>
  </div>
</template>
