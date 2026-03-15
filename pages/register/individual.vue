<script setup>
useHead({
  bodyAttrs: {
    class: "register-page",
  },
});

definePageMeta({
  middleware:"guest"
})

import Dialog from "~/components/Dialog.vue";
import CustomSelect from "~/components/elements/custom-select.vue";
import FileInput from "~/components/elements/file-input.vue";
import TextInput from "~/components/elements/text-input.vue";
import OTPDialog from "~/components/OTP-dialog.vue";
import { Field, Form, ErrorMessage, defineRule } from "vee-validate";
import { required, email, min, numeric } from "@vee-validate/rules";
import PhoneInput from "~/components/phone-input.vue";

// Define validation rules
defineRule("required", required);
defineRule("email", email);
defineRule("min", min);
defineRule("numeric", numeric);

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
  phone: "",
  email: "",
  gender: "",
  nationality: "",
  cvFile: null,
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
const onSubmit = (values) => {
  console.log("Form submitted", {
    ...values,
    cvFile: formData.value.cvFile,
    idFile: formData.value.idFile,
  });

  router.push("/register/success");

  // Handle form submission here
  // showOTPDialog.value = true; // Uncomment to show OTP dialog after submission
};
</script>

<template>
  <div class="py-24 px-4">
    <div
      class="card p-6 md:p-8 lg:p-10 max-w-[846px] mx-auto bg-white rounded-2xl"
    >
      <div class="header">
        <h1 class="text-xl lg:text-2xl font-bold text-[#161614] mb-3">
          إنشاء حساب جديد
        </h1>
        <p class="text-sm lg:text-base text-[#161614]">
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
                rules="required|numeric|min:10"
                label="رقم الهوية الوطنية أو الإقامة"
                placeholder="رقم الهوية الوطنية أو الإقامة"
                v-model="formData.nationalId"
                :error="errors.nationalId"
              />
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
                    :items="['ذكر', 'أنثى']"
                    :error="errors.gender"
                    v-model="formData.gender"
                  />
                </Field>

                <!-- </Field> -->
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
                <label for="cvFile" class="block text-sm mb-2">
                  السيرة الذاتية
                  <span class="text-gray-300 text-xs">(اختيارى)</span>
                </label>
                <Field
                  name="cvFile"
                  id="cvFile"
                  label="السيرة الذاتية"
                  v-model="formData.cvFile"
                >
                  <FileInput
                    accept=".pdf,.doc,.docx"
                    @change="handleFileChange('cvFile', $event)"
                  />
                </Field>
              </div>

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
              <button type="submit" class="btn-primary text-sm py-3 px-10">
                إنشاء حساب
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
