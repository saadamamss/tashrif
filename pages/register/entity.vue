<script setup>
useHead({
  bodyAttrs: {
    class: "register-page",
  },
});

definePageMeta({
  middleware: "guest",
});

import { Field, Form, ErrorMessage, defineRule } from "vee-validate";
import CustomSelect from "~/components/elements/custom-select.vue";
import TextInput from "~/components/elements/text-input.vue";
import PhoneInput from "~/components/phone-input.vue";

const currentStep = ref(3);
const formData = ref({});
const phoneValid = ref(false);
//
defineRule("phonevalidation", (value) => {
  return phoneValid.value ? true : "الرقم غير صحيح";
});

const steps = ["المعلومات الأساسية", "مواقع التواصل", "بيانات الإتصال"];
const schema = computed(() => {
  // Dynamic schema based on current step
  const stepSchemas = {
    1: {
      companyLogo: "required",
      companyName: "required",
      fieldName: "required",
      sector: "required",
      country: "required",
      province: "required",
      companyDesc: "required",
    },
    2: {
      companyWebsite: "required",
      twitterAccount: "required",
      facebookAccount: "required",
      youtubeAccount: "required",
    },
    3: {
      name: "required|min:2",
      email: "required|email",
      phone: "required|phonevalidation",
      role: "required",
      nationality: "required",
    },
  };
  return stepSchemas[currentStep.value];
});

const imagePreview = ref(null);
const handleFileInputChange = (event) => {
  formData.value.companyLogo = event.target.files[0];
  const file = event.target.files[0];
  if (file && file.type.match("image.*")) {
    const reader = new FileReader();

    reader.onload = (event) => {
      imagePreview.value = event.target.result;
    };

    reader.readAsDataURL(file);
  } else {
    imagePreview.value = null;
  }
};
const router = useRouter();
const onSubmit = () => {
  if (currentStep.value < 3) {
    currentStep.value++;
  } else {
    // Final submission
    console.log("Form submitted", formData.value);

    router.push("/register/success");
    // Submit to API or show success message
  }
};

const prevStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--;
  }
};
//
const handlePhoneValidation = (validation) => {
  phoneValid.value = validation.valid;
};

const handleCountryChange = (country) => {
  console.log("Country changed:", country);
};

const handleSubmit = () => {
  // Submit logic here
  console.log("Submitted phone:", form.value.phone);
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

      <!-- Step Indicator -->
      <div
        class="flex justify-between items-center py-8 border-b mb-8 relative overflow-hidden"
      >
        <div class="absolute left-0 right-0 border"></div>
        <div
          v-for="(step, stepIndex) in 3"
          :key="step"
          class="relative z-1 flex justify-between items-center border-4 border-[#fff]"
        >
          <div
            class="min-w-12 h-12 text-sm rounded-full flex items-center justify-center"
            :class="
              currentStep > step ? 'bg-[#ecb42b] text-black' : 'bg-[#f5f5f5]'
            "
          >
            <span v-if="currentStep == step" class="px-3">
              {{ steps[stepIndex] }}
            </span>

            <span v-else-if="currentStep > step">
              <svg
                width="32"
                height="32"
                viewBox="0 0 32 32"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M26.6668 8L12.0002 22.6667L5.3335 16"
                  stroke="#FFF5E8"
                  stroke-width="2.8"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </svg>
            </span>
            <span v-else>
              {{ step }}
            </span>
          </div>
        </div>
      </div>

      <Form @submit="onSubmit" :validation-schema="schema" v-slot="{ errors }">
        <!-- Step 1: Personal Info -->
        <div v-if="currentStep === 1">
          <div class="mb-6">
            <Field
              name="companyLogo"
              label="شعار الشركة"
              v-model="formData.companyLogo"
            >
              <label for="" class="block text-sm mb-3">
                شعار الشركة
                <span class="text-red-400">*</span>
              </label>
              <div
                class="relative w-24 h-24 border border-dashed border-[#ccc] bg-[#f5f5f5] rounded-full overflow-hidden hover:border-solid hover:bg-[#f9f9f9] transition"
              >
                <label
                  for="companylogo"
                  class="w-full h-full flex items-center justify-center cursor-pointer"
                >
                  <img
                    :src="imagePreview"
                    alt="company logo"
                    v-if="imagePreview"
                  />
                  <span v-else>
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        d="M10.6252 11.6663C10.6252 12.0115 10.3454 12.2913 10.0002 12.2913C9.65503 12.2913 9.3752 12.0115 9.3752 11.6663V4.61473L9.37015 4.62069C9.19458 4.82789 9.02267 5.04686 8.86191 5.25163L8.82465 5.29909C8.66446 5.50304 8.49593 5.7173 8.36505 5.85196C8.12446 6.09948 7.72877 6.1051 7.48125 5.86452C7.23373 5.62393 7.22811 5.22824 7.46869 4.98072C7.54261 4.90467 7.66397 4.75316 7.84163 4.52698L7.88082 4.47705C8.03921 4.27527 8.22504 4.03853 8.41648 3.8126C8.62165 3.57047 8.8513 3.31897 9.08086 3.12345C9.1959 3.02548 9.32556 2.92883 9.46506 2.85419C9.59959 2.78219 9.78497 2.70801 10.0002 2.70801C10.2154 2.70801 10.4008 2.78219 10.5354 2.85419C10.6748 2.92883 10.8045 3.02548 10.9195 3.12345C11.1491 3.31897 11.3788 3.57047 11.5839 3.8126C11.7754 4.03851 11.9611 4.27521 12.1195 4.47698L12.1588 4.52698C12.3364 4.75316 12.4578 4.90467 12.5317 4.98072C12.7723 5.22824 12.7667 5.62393 12.5192 5.86452C12.2716 6.1051 11.8759 6.09948 11.6354 5.85196C11.5045 5.7173 11.3359 5.50304 11.1758 5.2991L11.1385 5.25163C10.9777 5.04686 10.8058 4.82788 10.6303 4.62069L10.6252 4.61474V11.6663Z"
                        fill="#161614"
                      />
                      <path
                        d="M18.0895 11.8743C18.2044 11.5488 18.0337 11.1918 17.7082 11.077C17.3827 10.9621 17.0257 11.1328 16.9108 11.4583L16.7159 12.0105C16.332 13.0983 16.0604 13.865 15.7816 14.4398C15.5097 15.0002 15.2568 15.3207 14.9413 15.5439C14.6259 15.7671 14.2395 15.8989 13.6205 15.9688C12.9857 16.0404 12.1723 16.0413 11.0187 16.0413H8.98164C7.82799 16.0413 7.0146 16.0404 6.37984 15.9688C5.76086 15.8989 5.37447 15.7671 5.059 15.5439C4.74353 15.3207 4.49069 15.0002 4.21879 14.4398C3.93995 13.865 3.66837 13.0983 3.28441 12.0105L3.08954 11.4583C2.97466 11.1328 2.61766 10.9621 2.29216 11.077C1.96666 11.1918 1.79592 11.5488 1.9108 11.8743L2.11812 12.4618C2.48678 13.5063 2.78096 14.3398 3.09416 14.9854C3.41781 15.6525 3.78722 16.1753 4.33704 16.5643C4.88685 16.9533 5.50283 17.1277 6.23962 17.2109C6.95261 17.2914 7.83652 17.2913 8.94422 17.2913H11.0561C12.1638 17.2913 13.0477 17.2914 13.7607 17.2109C14.4975 17.1277 15.1135 16.9533 15.6633 16.5643C16.2131 16.1753 16.5825 15.6525 16.9062 14.9854C17.2194 14.3398 17.5136 13.5063 17.8822 12.4617L18.0895 11.8743Z"
                        fill="#161614"
                      />
                    </svg>
                  </span>
                </label>
                <input
                  type="file"
                  id="companylogo"
                  class="absolute top-0 left-0 right-0 w-full hidden"
                  accept="image/*"
                  @change="handleFileInputChange"
                />
              </div>
            </Field>
            <ErrorMessage
              name="companyLogo"
              class="text-red-500 text-xs mt-1"
            />
          </div>
          <div class="w-full flex gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="companyName"
                id="companyName"
                label="اسم الشركة"
                placeholder="اسم الشركة"
                v-model="formData.companyName"
                :error="errors.companyName"
                required
              />
            </div>
          </div>
          <div
            class="w-full flex flex-col sm:flex-row flex gap-4 md:gap-6 mb-6"
          >
            <div class="flex-1">
              <TextInput
                :error="errors.fieldName"
                v-model="formData.fieldName"
                name="fieldName"
                id="fieldName"
                type="text"
                label="المجال"
                placeholder="المجال"
                required
              />
            </div>
            <div class="flex-1">
              <TextInput
                label="القطاع"
                placeholder="القطاع"
                name="sector"
                id="sector"
                required
                :error="errors.sector"
                v-model="formData.sector"
              />
            </div>
          </div>

          <div class="w-full flex flex-col sm:flex-row gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <label for="role" class="block text-sm mb-2">
                الدولة <span class="text-red-400">*</span>
              </label>
              <Field v-model="formData.country" name="country" label="الدولة">
                <CustomSelect
                  :items="['السعودية', 'مصر']"
                  placeholder="الدولة"
                  :error="errors.country"
                  v-model="formData.country"
                />
              </Field>
              <ErrorMessage name="country" class="text-red-500 text-xs mt-1" />
            </div>
            <div class="flex-1">
              <label for="role" class="block text-sm mb-2">
                المنطقة <span class="text-red-400">*</span>
              </label>
              <Field
                v-model="formData.province"
                name="province"
                label="المنطقة"
              >
                <CustomSelect
                  :items="['منطقة 2', 'منطقة 1']"
                  placeholder="المنطقة"
                  :error="errors.province"
                  v-model="formData.province"
                />
              </Field>
              <ErrorMessage name="province" class="text-red-500 text-xs mt-1" />
            </div>
          </div>

          <div class="mb-6">
            <label for="role" class="block text-sm mb-2">
              وصف الشركة <span class="text-red-400">*</span>
            </label>
            <Field
              v-model="formData.companyDesc"
              name="companyDesc"
              type="text"
              label="وصف الشركة"
            >
              <textarea
                class="text-sm rounded-xl w-full py-2 px-2 bg-[#f5f5f5] border border-[#ECB42B]/0 focus:border-[#ECB42B] outline-none transition duration-300"
                placeholder="أدخل وصف الشركة"
                v-model="formData.companyDesc"
                rows="8"
              ></textarea>
            </Field>
            <ErrorMessage
              name="companyDesc"
              class="text-red-500 text-xs mt-1"
            />
          </div>
        </div>

        <!-- Step 2: Contact Info -->
        <div v-if="currentStep === 2">
          <div class="w-full flex-col sm:flex-row flex gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                label="موقعك الإلكترونى"
                name="companyWebsite"
                id="companyWebsite"
                required
                :error="errors.companyWebsite"
                v-model="formData.companyWebsite"
                placeholder="www.capitalx.com"
              />
            </div>
            <div class="flex-1">
              <TextInput
                label="تويتر"
                name="twitterAccount"
                id="twitterAccount"
                required
                :error="errors.twitterAccount"
                v-model="formData.twitterAccount"
                placeholder="twitter.com/CapitalX"
              />
            </div>
          </div>

          <div class="w-full flex-col sm:flex-row flex gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                label="فيسبوك"
                name="facebookAccount"
                id="facebookAccount"
                required
                :error="errors.facebookAccount"
                v-model="formData.facebookAccount"
                placeholder="facebook.com/CapitalX"
              />
            </div>
            <div class="flex-1">
              <TextInput
                label="يوتيوب"
                name="youtubeAccount"
                id="youtubeAccount"
                required
                :error="errors.youtubeAccount"
                v-model="formData.youtubeAccount"
                placeholder="youtube.com/CapitalX"
              />
            </div>
          </div>
        </div>

        <!-- Step 3: Documents -->
        <div v-if="currentStep === 3">
          <div class="mb-6">
            <TextInput
              label="الإسم "
              name="name"
              id="name"
              required
              :error="errors.name"
              v-model="formData.name"
              placeholder="أدخل الإسم"
            />
          </div>

          <div class="w-full flex flex-col sm:flex-row gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <label for="phone" class="block text-sm mb-2">
                رقم الجوال <span class="text-red-400">*</span>
              </label>

              <Field
                id="phone"
                name="phone"
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
                label="البريد الإلكترونى"
                :error="errors.email"
                required
                v-model="formData.email"
                placeholder="أدخل بريدك الإلكترونى"
              />
            </div>
          </div>
          <div class="w-full flex flex-col sm:flex-row gap-4 md:gap-6 mb-6">
            <div class="flex-1">
              <label for="role" class="block text-sm mb-2">
                المنصب <span class="text-red-400">*</span>
              </label>
              <Field v-model="formData.role" name="role" label="الدولة">
                <CustomSelect
                  :items="['مدير', 'نائب']"
                  placeholder="المنصب"
                  :error="errors.role"
                  v-model="formData.role"
                />
              </Field>
              <ErrorMessage name="role" class="text-red-500 text-xs mt-1" />
            </div>

            <div class="flex-1">
              <TextInput
                label="الجنسية"
                name="nationality"
                id="nationality"
                required
                :error="errors.nationality"
                v-model="formData.nationality"
                placeholder="الجنسية"
              />
            </div>
          </div>
        </div>

        <div class="mt-10 flex justify-between">
          <button
            v-if="currentStep > 1"
            @click.prevent="prevStep"
            type="button"
            class="btn-outline text-sm py-3 px-10"
          >
            السابق
          </button>
          <div v-else></div>
          <!-- Spacer -->

          <button type="submit" class="btn-primary text-sm py-3 px-10">
            {{ currentStep === 3 ? "إنشاء حساب" : "التالي" }}
          </button>
        </div>
      </Form>
    </div>
  </div>
</template>
