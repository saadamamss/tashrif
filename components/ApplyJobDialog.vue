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
            <label class="text-sm block mb-2"> نبذة عن خبراتك </label>
            <textarea
              v-model="formData.coverLetter"
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
          <button type="submit" class="btn-primary text-sm">التقديم</button>
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
  coverLetter: "",
  qualificationId: null,
});

const qualifications = ref([]);
const qualificationItems = computed(() =>
  qualifications.value.map((q) => ({ value: q.id, label: q.type }))
);

const isSubmitting = ref(false);

const emit = defineEmits(["applied"]);

// Fetch qualifications when dialog opens
watch(model, async (val) => {
  if (val && qualifications.value.length === 0) {
    const { data } = await useApi().get("/qualifications");
    if (data?.items) qualifications.value = data.items;
  }
});

const submitApplication = async () => {
  isSubmitting.value = true;
  try {
    const { error } = await useApi().post("/applications/apply", {
      jobId: Number(props.jobId),
      qualificationId: Number(formData.value.qualificationId),
      experience: formData.value.coverLetter,
    });
    if (error) {
      useToast().show(error, "error");
      return;
    }
    useToast().show("تم إرسال طلب التقديم بنجاح", "success");
    emit("applied", Number(props.jobId));
    model.value = false;
    formData.value.coverLetter = "";
    formData.value.qualificationId = null;
  } catch {
    useToast().show("حدث خطأ غير متوقع", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const cancelApplication = () => {
  formData.value.coverLetter = "";
  formData.value.qualificationId = null;
  model.value = false;
};
</script>

<style scoped></style>
