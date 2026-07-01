<template>
  <div class="relative">
    <div
      class="p-4 md:p-6 bg-[#f5f5f5] overflow-hidden border border-dashed relative rounded-xl border-[#161614] min-h-32"
    >
      <input
        @change="handleFileChange"
        type="file"
        class="absolute w-full h-full top-0 left-0 appearance-none opacity-0 z-10"
        :accept="accept"
      />

      <div>
        <div v-if="!filePreview" class="flex flex-col items-center relative">
          <span class="mb-2 w-8 h-8">
            <UploadIcon width="17" height="16" />
          </span>
          <h2 class="text-sm font-medium mb-2">
            اسحب وأفلِت أو اختر الملف الذي تريد تحميله
          </h2>
          <p class="text-xs">الحد الأقصى للحجم 5 ميجا بايت</p>
        </div>

        <div v-else class="flex flex-col items-center">
          <span class="flex justify-center w-8 h-8 mb-2">
            <FileIcon v-if="['pdf', 'doc', 'docx'].some((i) => accept.includes(i))" />
            <ImageIcon v-if="accept.includes('image')" />
          </span>
          <span class="block text-sm font-medium mb-2">
            {{ filePreview.name }}
          </span>
          <span class="block text-xs text-[#667178]">
            {{ formattedFileSize }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import Pdf from "../icons/pdf.vue";

const filePreview = ref(null);

/** @type {{ accept: string }} */
const props = defineProps({
  accept: { type: String, default: "" },
});

/**
 * @type {import('vue').EmitsOptions}
 */
const emit = defineEmits(["change"]);

const checkAcceptance = (type) => {
  return props.accept.split(",").some((i) => type.match(i));
};

const typeError = ref(null);
const handleFileChange = (event) => {
  emit("change", event);

  const file = event.target.files[0];
  if (file && checkAcceptance(file.type)) {
    const reader = new FileReader();

    reader.onload = (event) => {
      filePreview.value = file;
    };

    reader.readAsDataURL(file);
  } else {
    filePreview.value = null;
  }
};

const formattedFileSize = computed(() => {
  if (filePreview.value) {
    return formatBytes(filePreview.value.size);
  }
  return "N/A";
});
const formatBytes = (bytes, decimals = 2) => {
  if (bytes === 0) return "0 Bytes";

  const k = 1024;
  const dm = decimals < 0 ? 0 : decimals;
  const sizes = ["بايت", "كيلو بايت", "ميجا بايت"];

  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
};
</script>
