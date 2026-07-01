<template>
  <Transition name="modal">
    <div v-if="model" class="modal-mask">
      <div class="modal-container" @click.self="closeModal">
        <div class="modal-content">
          <div class="bg-white drawer sm:rounded-l-3xl p-6">
            <div class="h-full relative">
              <div class="mb-6 flex items-center justify-between">
                <h1 class="text-lg font-bold">فلترة المتقدمين</h1>
                <span>
                  <button
                    @click="closeModal"
                    class="w-8 h-8 flex items-center justify-center bg-bg-light rounded-full"
                  >
                    <Close />
                  </button>
                </span>
              </div>
              <div>
                <div class="flex gap-4 mb-6">
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2"
                      >هل الموظف سعودى؟</label
                    >
                    <CustomSelect
                      :items="['نعم', 'لا']"
                      placeholder="هل الموظف سعودى؟"
                      v-model="filters.isSaudi"
                    />
                  </div>
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2">الجنسية</label>
                    <CustomSelect
                      :items="['سعودى', 'مصرى']"
                      placeholder="الجنسية"
                      v-model="filters.nationality"
                    />
                  </div>
                </div>
                <div class="flex gap-4 mb-6">
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2"
                      >المؤهل العلمى</label
                    >
                    <CustomSelect
                      :items="['نعم', 'لا']"
                      placeholder="المؤهل العلمى"
                      v-model="filters.qualification"
                    />
                  </div>
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2">التخصص</label>
                    <CustomSelect
                      :items="['علوم حاسب', 'هندسة']"
                      placeholder="التخصص"
                      v-model="filters.specialization"
                    />
                  </div>
                </div>
                <div class="flex gap-4">
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2">
                      تاريخ التقديم
                    </label>
                    <CustomSelect
                      :items="['2024', '2023', '2022']"
                      placeholder="تاريخ التقديم"
                      v-model="filters.inrollDate"
                    />
                  </div>
                  <div class="flex-1">
                    <label for="" class="text-sm block mb-2"
                      >هل الموظف قديم؟</label
                    >
                    <CustomSelect
                      :items="['نعم', 'لا']"
                      placeholder="هل الموظف قديم؟"
                      v-model="filters.isOld"
                    />
                  </div>
                </div>
              </div>
              <!--  -->
              <div class="absolute w-full bottom-0">
                <div class="flex items-center justify-between">
                  <button class="btn-outline text-sm" @click="closeModal">
                    مسح الفلاتر
                  </button>
                  <button class="btn-primary text-sm" @click="closeModal">
                    تطبيق الفلتر (24 مشروع)
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import CustomSelect from "./elements/CustomSelect.vue";

const model = defineModel();

/** @type {{ isOpen: boolean, title: string, subtitle: string, showFooter: boolean }} */
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
const filters = ref({});

/** @type {import('vue').EmitsOptions} */
const emit = defineEmits(["close", "confirm"]);
const closeModal = () => {
  model.value = false;
};

const confirmAction = () => {
  emit("confirm");
};
watch(model, () => {
  if (model.value) {
    document.body.classList.add("no-scroll");
  } else {
    document.body.classList.remove("no-scroll");
  }
});
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
  @apply fixed inset-0 flex items-center justify-center p-4;
}

.modal-content {
  @apply max-w-[540px] w-full max-h-[100vh] overflow-y-auto absolute right-0;
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
.drawer {
  height: 100vh;
  height: 100dvh;
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
