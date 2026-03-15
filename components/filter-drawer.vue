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
                    class="w-8 h-8 flex items-center justify-center bg-[#f5f5f5] rounded-full"
                  >
                    <svg
                      width="15"
                      height="15"
                      viewBox="0 0 15 15"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <rect width="15" height="14.9826" fill="#F6F9F5" />
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M2.79354 2.79143C2.9766 2.60859 3.2734 2.60859 3.45646 2.79143L7.5 6.83027L11.5435 2.79143C11.7266 2.60859 12.0234 2.60859 12.2065 2.79143C12.3895 2.97428 12.3895 3.27073 12.2065 3.45357L8.16291 7.49241L12.2065 11.5313C12.3895 11.7141 12.3895 12.0106 12.2065 12.1934C12.0234 12.3762 11.7266 12.3762 11.5435 12.1934L7.5 8.15456L3.45646 12.1934C3.2734 12.3762 2.9766 12.3762 2.79354 12.1934C2.61049 12.0106 2.61049 11.7141 2.79354 11.5313L6.83709 7.49241L2.79354 3.45357C2.61049 3.27073 2.61049 2.97428 2.79354 2.79143Z"
                        fill="#B1B4B0"
                      />
                    </svg>
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
import CustomSelect from "./elements/custom-select.vue";

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
const filters = ref({});

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
