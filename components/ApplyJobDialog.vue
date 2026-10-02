<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <!-- Form Title -->
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          التقديم على وظيفة
        </h2>
        <p class="text-gray-600 text-sm">
          أنت على بعد خطوة واحدة من التقديم على هذه الوظيفة.
        </p>
      </div>

      <Form @submit="submitApplication">
        <div class="p-6">
          <div class="mb-8">
            <label class="text-sm block mb-2"> المؤهل </label>
            <CustomSelect
              v-model="formData.qualificationId"
              :items="qualificationItems"
              placeholder="اختر المؤهل"
            />
          </div>

          <div class="mb-8">
            <label class="text-sm block mb-2"> السيرة الذاتية </label>
            <CustomSelect
              v-if="!hasNoCvs"
              v-model="formData.cvId"
              :items="cvItems"
              placeholder="اختر السيرة الذاتية"
            />
            <div v-else class="text-sm bg-bg-light p-4 rounded-lg">
              <p class="text-gray-600 mb-2">لا توجد سيرة ذاتية في ملفك — أضف واحدة أولاً لتتمكن من التقديم.</p>
              <NuxtLink to="/dashboard/profile" class="text-primary font-medium">الذهاب إلى الملف الشخصي</NuxtLink>
            </div>
          </div>

          <div class="mb-8">
            <label class="text-sm block mb-2"> نبذة عن خبراتك </label>
            <textarea
              v-model="formData.experience"
              placeholder="اكتب ملخصاً عن خبراتك السابقة ومؤهلاتك لهذه الوظيفة..."
              rows="5"
              class="text-sm w-full placeholder:text-xs bg-bg-light p-4 border border-[#fff]/0 rounded-lg focus:outline-none focus:border-primary transition"
            ></textarea>
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button
            @click="cancelApplication"
            type="button"
            class="btn-outline text-sm"
          >
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm" :disabled="hasNoCvs">التقديم</button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import Dialog from "./Dialog.vue";
import CustomSelect from "./elements/CustomSelect.vue";
import { Form } from "vee-validate";
const model = defineModel();

const props = defineProps({
  jobId: {
    type: [Number, String],
    required: false,
    default: null,
  },
});

const formData = ref({
  experience: "",
  qualificationId: null,
  cvId: null,
});

const qualifications = ref([]);
const qualificationItems = computed(() =>
  qualifications.value.map((q) => ({ value: q.id, label: q.type }))
);

const cvs = ref([]);
const cvsLoaded = ref(false);
const cvItems = computed(() =>
  cvs.value.map((c) => ({ value: c.id, label: c.fileName || "السيرة الذاتية" }))
);
const hasNoCvs = computed(() => cvsLoaded.value && cvs.value.length === 0);

const isSubmitting = ref(false);

const emit = defineEmits(["applied"]);

// Fetch qualifications + CVs when dialog opens (fresh each time so a CV
// added from the profile mid-session is picked up)
watch(model, async (val) => {
  if (!val) return;
  const [qualsRes, cvsRes] = await Promise.allSettled([
    useApi().get("/qualifications"),
    useApi().get("/cvs"),
  ]);
  if (qualsRes.status === "fulfilled" && qualsRes.value.data?.items) {
    qualifications.value = qualsRes.value.data.items;
  }
  if (cvsRes.status === "fulfilled") {
    const items = cvsRes.value.data?.items;
    cvs.value = Array.isArray(items) ? items : cvsRes.value.data || [];
    cvsLoaded.value = true;
  }
});

const submitApplication = async () => {
  if (!formData.value.qualificationId) {
    useToast().show("يجب اختيار المؤهل", "error");
    return;
  }
  if (!formData.value.cvId) {
    useToast().show("يجب اختيار السيرة الذاتية", "error");
    return;
  }
  isSubmitting.value = true;
  try {
    const { error } = await useApi().post("/applications/apply", {
      jobId: Number(props.jobId),
      qualificationId: Number(formData.value.qualificationId),
      cvId: Number(formData.value.cvId),
      experience: formData.value.experience,
    });
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تم إرسال طلب التقديم بنجاح", "success");
    emit("applied", Number(props.jobId));
    model.value = false;
    resetForm();
  } catch {
    useToast().show("حدث خطأ غير متوقع", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const resetForm = () => {
  formData.value.experience = "";
  formData.value.qualificationId = null;
  formData.value.cvId = null;
};

const cancelApplication = () => {
  resetForm();
  model.value = false;
};
</script>

<style scoped></style>
