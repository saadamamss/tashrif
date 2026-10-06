<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          {{ title }}
        </h2>
        <p class="text-gray-600 text-sm text-right leading-relaxed">
          {{ message }}
        </p>
      </div>

      <div class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light">
        <button
          type="button"
          @click="cancel"
          class="btn-outline text-sm"
        >
          إلغاء
        </button>
        <button
          type="button"
          @click="confirmAction"
          class="text-sm py-3 px-4 xs:px-6 rounded-full text-white"
          :class="danger ? 'bg-danger' : 'bg-primary'"
          :disabled="isLoading"
        >
          {{
            isLoading
              ? "جارٍ الحذف..."
              : confirmText
          }}
        </button>
      </div>
    </div>
  </Dialog>
</template>

<script setup>
const model = defineModel();

const props = defineProps({
  title: { type: String, default: "تأكيد الحذف" },
  message: { type: String, default: "هل أنت متأكد من الحذف؟" },
  confirmText: { type: String, default: "حذف" },
  danger: { type: Boolean, default: true },
  isLoading: { type: Boolean, default: false },
});

const emit = defineEmits(["confirm", "cancel"]);

const cancel = () => {
  model.value = false;
  emit("cancel");
};

const confirmAction = () => {
  emit("confirm");
};
</script>