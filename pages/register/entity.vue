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
import CustomSelect from "~/components/elements/CustomSelect.vue";
import TextInput from "~/components/elements/TextInput.vue";
import PhoneInput from "~/components/PhoneInput.vue";

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
              <StepCheckIcon />
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
                    <UploadIcon />
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
