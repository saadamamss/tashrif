<template>
  <Transition name="modal">
    <div v-if="model" class="modal-mask">
      <div class="modal-container" @click.self="closeModal">
        <div class="modal-content">
          <slot></slot>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
const model = defineModel();
const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false,
  },
  title: {
    type: String,
    default: "Modal Title",
  },
  subtitle: {
    type: String,
    default: "Modal SubTitle",
  },
  showFooter: {
    type: Boolean,
    default: true,
  },
});

const emit = defineEmits(["close", "confirm"]);

const closeModal = () => {
  model.value = false;
};

const confirmAction = () => {
  emit("confirm");
};
</script>

<style scoped lang="scss">
/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

/* Modal Styles */
.modal-mask {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center;
  direction: rtl;
  z-index: 1000;
}

.modal-container {
  @apply fixed inset-0 flex items-center justify-center p-2 sm:p-4;
}

.modal-content {
  @apply max-w-[846px] w-full max-h-[90vh] overflow-y-auto;
  &::-webkit-scrollbar {
    width: 4px;
  }
  &::-webkit-scrollbar-thumb {
    width: 4px;
    background-color: #494949;
  }
  &::-webkit-scrollbar-track {
    width: 4px;
    background-color: #f5f5f5;
  }
}

.modal-header {
  @apply flex justify-between items-center p-4 border-b;
}

.modal-title {
  @apply text-lg font-semibold text-lg text-gray-800;
}

.modal-close-btn {
  @apply text-gray-500 hover:text-gray-700 text-2xl;
}

.modal-body {
  @apply p-4;
}

.modal-footer {
  @apply flex justify-end gap-3 p-4 border-t;
}

.modal-cancel-btn {
  @apply px-4 py-2 text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200;
}

.modal-confirm-btn {
  @apply px-4 py-2 text-white bg-blue-600 rounded-md hover:bg-blue-700;
}
</style>
