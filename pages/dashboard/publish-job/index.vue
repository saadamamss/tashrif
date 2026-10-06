<script setup>
import { Field, Form } from "vee-validate";
import Breadcrumbs from "~/components/elements/Breadcrumbs.vue";
import CustomSelect from "~/components/elements/CustomSelect.vue";
import TextInput from "~/components/elements/TextInput.vue";
import SuccessPublishing from "~/components/SuccessPublishing.vue";
import { cityOptions, workTypeOptions } from "~/services/jobLabels";

definePageMeta({
  layout: "dashboard",
  middleware: ["auth", "entity"],
  meta: { requiresAuth: true },
});

useHead({
  title: 'نشر وظيفة جديدة',
})

const breadcrumbs = [
  {
    label: "الرئيسية",
    to: "/dashboard",
    active: true,
  },
  {
    label: "نشر وظيفة جديدة",
    to: null,
    active: false,
  },
];
const succesDialog = ref(false);

const formData = ref({
  qualification: "",
  jobTitle: "",
  vacancies: "",
  jobPlace: "",
  jobType: "",
  targets: "",
  hours: "",
  duration: "",
  endDate: "",
  salary: "",
  jobDesc: "",
  benefits: "",
  responsibilities: "",
  conditions: "",
});

const { save: saveForm, restore: restoreForm, clear: clearForm } = useFormPersistence('publish-job')
const { enable: warnBeforeUnload, disable: disableUnloadWarning } = useBeforeUnload('لديك بيانات غير محفوظة في نموذج النشر')

watch(formData, () => saveForm(formData.value), { deep: true })

onMounted(() => {
  const saved = restoreForm()
  if (saved) formData.value = saved
})

warnBeforeUnload()

const isSubmitting = ref(false);

const splitLines = (value) =>
  (value || "")
    .split("\n")
    .map((s) => s.trim())
    .filter(Boolean);

const submitForm = async () => {
  isSubmitting.value = true
  try {
    const payload = {
      title: formData.value.jobTitle,
      description: formData.value.jobDesc,
      location: formData.value.jobPlace,
      type: formData.value.jobType,
      target: formData.value.targets,
      gender: formData.value.targets,
      hours: formData.value.hours,
      duration: formData.value.duration,
      endDate: formData.value.endDate ? new Date(formData.value.endDate).toISOString() : null,
      vacancies: Number(formData.value.vacancies) || 0,
      qualification: formData.value.qualification,
      salary: formData.value.salary,
      benefits: splitLines(formData.value.benefits),
      responsibilities: splitLines(formData.value.responsibilities),
      conditions: splitLines(formData.value.conditions),
    }
    const { error } = await useApi().post('/jobs/publish', payload)
    if (error) {
      useToast().show(error, "error")
      return
    }
    useToast().show("تم نشر الوظيفة بنجاح", "success")
    clearForm()
    disableUnloadWarning()
    succesDialog.value = true;
  } catch {
    useToast().show("حدث خطأ أثناء نشر الوظيفة", "error")
  } finally {
    isSubmitting.value = false
  }
};
</script>

<template>
  <div class="px-4 lg:px-0 py-4">
    <div class="mb-3">
      <Breadcrumbs :items="breadcrumbs" />
    </div>
    <div class="bg-white rounded-2xl shadow-sm p-4 sm:p-6">
      <div>
        <h1 class="text-lg lg:text-xl font-bold mb-3">نشر وظيفة جديدة</h1>
        <p class="text-sm text-muted">
          أضف تفاصيل الوظيفة لعرضها في المنصة، ليتمكن الباحثون عن العمل من
          الاطلاع عليها والتقديم.
        </p>
      </div>
      <div class="py-6">
        <hr />
      </div>
      <!--  -->
      <div class="mb-6">
        <Form @submit="submitForm" v-slot="{ errors }">
          <div class="mb-6">
            <TextInput
              name="job-title"
              id="job-title"
              label="المسمى الوظيفى"
              placeholder="المسمى الوظيفى"
              v-model="formData.jobTitle"
              :error="errors.jobTitle"
              required
            />
          </div>

          <div class="grid sm:grid-cols-2 gap-6 mb-6">
            <div>
              <TextInput
                name="vacant-no"
                id="vacant-no"
                label="عدد الشواغر"
                placeholder="0000"
                v-model="formData.vacancies"
                :error="errors.vacancies"
                required
              />
            </div>

            <div>
              <label for="job-place" class="text-sm mb-2 block">
                مكان العمل
                <span class="text-red-400">*</span>
              </label>
              <Field
                name="job-place"
                id="job-place"
                v-model="formData.jobPlace"
              >
                <CustomSelect
                  placeholder="مكان العمل"
                  :items="cityOptions"
                  required
                  v-model="formData.jobPlace"
                  :error="errors.jobPlace"
                />
              </Field>
            </div>
          </div>

          <div class="grid sm:grid-cols-2 gap-6 mb-6">
            <div>
              <label for="job-type" class="text-sm mb-2 block">
                نوع العمل
                <span class="text-red-400">*</span>
              </label>
              <Field name="job-type" id="job-type" v-model="formData.jobType">
                <CustomSelect
                  placeholder="نوع العمل"
                  :items="workTypeOptions"
                  required
                  v-model="formData.jobType"
                  :error="errors.jobType"
                />
              </Field>
            </div>
            <div>
              <label for="targets" class="text-sm mb-2 block">
                الفئة المستهدفة
                <span class="text-red-400">*</span>
              </label>
              <Field name="targets" id="targets" v-model="formData.targets">
                <CustomSelect
                  placeholder="الفئة المستهدفة"
                  :items="[
                    { value: 'both', label: 'رجال ونساء' },
                    { value: 'male', label: 'رجال' },
                    { value: 'female', label: 'نساء' },
                  ]"
                  required
                  v-model="formData.targets"
                  :error="errors.targets"
                />
              </Field>
            </div>
          </div>
          <!--  -->
          <div class="grid sm:grid-cols-2 gap-6 mb-6">
            <div>
              <label for="job-qualification" class="text-sm mb-2 block">
                المؤهل المطلوب
                <span class="text-red-400">*</span>
              </label>
              <Field
                name="qualification"
                id="qualification"
                v-model="formData.qualification"
              >
                <CustomSelect
                  placeholder="المؤهل المطلوب"
                  :items="['ثانوية عامة', 'دبلوم', 'بكالوريوس', 'ماجستير']"
                  required
                  v-model="formData.qualification"
                  :error="errors.qualification"
                />
              </Field>
            </div>
            <div class="relative">
              <TextInput
                name="salary"
                id="salary"
                label="الراتب"
                placeholder="000"
                v-model="formData.salary"
                :error="errors.salary"
                rules="required"
                required
              >
                <span
                  class="block absolute h-[38px] flex items-center left-[1px] bottom-[1px] px-3 bg-white text-sm rounded-2xl"
                  style="z-index: 1"
                >
                  ريال سعودى
                </span>
              </TextInput>
            </div>
          </div>
          <!--  -->
          <div class="grid sm:grid-cols-3 gap-6 mb-6">
            <div>
              <label for="job-hours" class="text-sm mb-2 block">
                ساعات العمل
                <span class="text-red-400">*</span>
              </label>
              <Field name="job-hours" id="job-hours" v-model="formData.hours">
                <CustomSelect
                  placeholder="ساعات العمل"
                  :items="[
                    { value: '8', label: '8 ساعات' },
                    { value: '10', label: '10 ساعات' },
                    { value: '12', label: '12 ساعة' },
                  ]"
                  required
                  v-model="formData.hours"
                  :error="errors.hours"
                />
              </Field>
            </div>
            <div>
              <label for="job-duration" class="text-sm mb-2 block">
                المدة
                <span class="text-red-400">*</span>
              </label>
              <Field name="job-duration" id="job-duration" v-model="formData.duration">
                <CustomSelect
                  placeholder="المدة"
                  :items="[
                    { value: 'month', label: 'شهر واحد' },
                    { value: '2months', label: 'شهرين' },
                    { value: '3months', label: '3 أشهر' },
                    { value: '6months', label: '6 أشهر' },
                    { value: 'year', label: 'سنة' },
                  ]"
                  required
                  v-model="formData.duration"
                  :error="errors.duration"
                />
              </Field>
            </div>
            <div>
              <TextInput
                name="end-date"
                id="end-date"
                label="تاريخ الانتهاء"
                type="date"
                v-model="formData.endDate"
                :error="errors.endDate"
              />
            </div>
          </div>
          <!--  -->
          <div class="grid sm:grid-cols-2 gap-6 mb-6">
            <div>
              <label for="job-desc" class="text-sm mb-2 block">
                وصف الوظيفة
                <span class="text-red-400">*</span>
              </label>
              <Field name="job-desc" id="job-desc" v-model="formData.jobDesc">
                <textarea
                  rows="6"
                  class="text-sm placeholder:text-xs p-3 bg-bg-light border border-[#fff]/0 focus:border-primary focus:outline-none rounded-xl w-full transition"
                  placeholder="وصف الوظيفة"
                  v-model="formData.jobDesc"
                  :error="errors.jobDesc"
                ></textarea>
              </Field>
            </div>
            <div>
              <label for="job-benefits" class="text-sm mb-2 block">
                المزايا والمكافأة
                <span class="text-red-400">*</span>
              </label>
              <Field
                name="job-benefits"
                id="job-benefits"
                v-model="formData.benefits"
              >
                <textarea
                  rows="6"
                  class="text-sm placeholder:text-xs p-3 bg-bg-light border border-[#fff]/0 focus:border-primary focus:outline-none rounded-xl w-full transition"
                  placeholder="المزايا والمكافأة "
                  v-model="formData.benefits"
                  :error="errors.benefits"
                ></textarea>
              </Field>
            </div>
          </div>
          <!--  -->
          <div class="grid sm:grid-cols-2 gap-6 mb-6">
            <div>
              <label for="job-responsibilities" class="text-sm mb-2 block">
                المهام والمسؤوليات
                <span class="text-red-400">*</span>
              </label>
              <Field
                name="job-responsibilities"
                id="job-responsibilities"
                v-model="formData.responsibilities"
              >
                <textarea
                  rows="6"
                  class="text-sm placeholder:text-xs p-3 bg-bg-light border border-[#fff]/0 focus:border-primary focus:outline-none rounded-xl w-full transition"
                  placeholder="المهام والمسؤوليات"
                  v-model="formData.responsibilities"
                  :error="errors.responsibilities"
                ></textarea>
              </Field>
            </div>
            <div>
              <label for="job-conditions" class="text-sm mb-2 block">
                شروط القبول
                <span class="text-red-400">*</span>
              </label>
              <Field
                name="job-conditions"
                id="job-conditions"
                v-model="formData.conditions"
              >
                <textarea
                  rows="6"
                  class="text-sm placeholder:text-xs p-3 bg-bg-light border border-[#fff]/0 focus:border-primary focus:outline-none rounded-xl w-full transition"
                  placeholder="شروط القبول"
                  v-model="formData.conditions"
                  :error="errors.conditions"
                ></textarea>
              </Field>
            </div>
          </div>

          <div class="pt-4 flex justify-between">
            <button
              type="button"
              class="btn-outline text-sm"
              @click="() => $router.push('/dashboard')"
            >
              إلغاء
            </button>

            <button type="submit" class="btn-primary text-sm">
              نشر وظيفة جديدة
            </button>
          </div>
        </Form>
      </div>
    </div>

    <!--  -->
    <SuccessPublishing v-model="succesDialog" />
  </div>
</template>
