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
            <svg
              width="17"
              height="16"
              viewBox="0 0 17 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M9.1252 9.66683C9.1252 10.012 8.84538 10.2918 8.5002 10.2918C8.15503 10.2918 7.8752 10.012 7.8752 9.66683V2.61522L7.87015 2.62118C7.69458 2.82838 7.52267 3.04735 7.36191 3.25212L7.32465 3.29958C7.16446 3.50353 6.99593 3.71779 6.86505 3.85245C6.62446 4.09997 6.22877 4.10559 5.98125 3.865C5.73373 3.62442 5.72811 3.22873 5.96869 2.98121C6.04261 2.90516 6.16397 2.75365 6.34163 2.52746L6.38082 2.47754C6.53921 2.27576 6.72504 2.03901 6.91648 1.81309C7.12165 1.57096 7.3513 1.31946 7.58086 1.12394C7.6959 1.02596 7.82556 0.929323 7.96506 0.854675C8.09959 0.782679 8.28497 0.708496 8.5002 0.708496C8.71544 0.708496 8.90082 0.782679 9.03535 0.854675C9.17484 0.929323 9.30451 1.02596 9.41955 1.12394C9.64911 1.31946 9.87875 1.57096 10.0839 1.81309C10.2754 2.039 10.4611 2.27569 10.6195 2.47747L10.6588 2.52746C10.8364 2.75365 10.9578 2.90516 11.0317 2.98121C11.2723 3.22873 11.2667 3.62442 11.0192 3.86501C10.7716 4.10559 10.3759 4.09997 10.1354 3.85245C10.0045 3.71779 9.83595 3.50353 9.67576 3.29958L9.63849 3.25211C9.47773 3.04735 9.30582 2.82837 9.13025 2.62118L9.1252 2.61522V9.66683Z"
                fill="#161614"
              />
              <path
                d="M16.5895 9.87483C16.7044 9.54933 16.5337 9.19233 16.2082 9.07745C15.8827 8.96257 15.5257 9.13331 15.4108 9.45881L15.2159 10.0109C14.832 11.0988 14.5604 11.8655 14.2816 12.4403C14.0097 13.0007 13.7568 13.3212 13.4413 13.5444C13.1259 13.7676 12.7395 13.8994 12.1205 13.9692C11.4857 14.0409 10.6723 14.0418 9.5187 14.0418H7.48164C6.32799 14.0418 5.5146 14.0409 4.87984 13.9692C4.26086 13.8994 3.87447 13.7676 3.559 13.5444C3.24353 13.3212 2.99069 13.0007 2.71879 12.4403C2.43995 11.8655 2.16837 11.0988 1.78441 10.0109L1.58954 9.45881C1.47466 9.13331 1.11766 8.96257 0.792158 9.07745C0.466658 9.19234 0.295919 9.54934 0.410801 9.87484L0.618125 10.4623C0.986782 11.5068 1.28096 12.3403 1.59416 12.9859C1.91781 13.653 2.28722 14.1758 2.83704 14.5648C3.38685 14.9538 4.00283 15.1282 4.73962 15.2114C5.45261 15.2918 6.33652 15.2918 7.44422 15.2918H9.55612C10.6638 15.2918 11.5477 15.2918 12.2607 15.2114C12.9975 15.1282 13.6135 14.9538 14.1633 14.5648C14.7131 14.1758 15.0825 13.653 15.4062 12.9859C15.7194 12.3403 16.0136 11.5068 16.3822 10.4622L16.5895 9.87483Z"
                fill="#161614"
              />
            </svg>
          </span>
          <h2 class="text-sm font-medium mb-2">
            اسحب وأفلِت أو اختر الملف الذي تريد تحميله
          </h2>
          <p class="text-xs">الحد الأقصى للحجم 5 ميجا بايت</p>
        </div>

        <div v-else class="flex flex-col items-center">
          <span class="flex justify-center w-8 h-8 mb-2">
            <svg
              v-if="['pdf', 'doc', 'docx'].some((i) => accept.includes(i))"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="#cdcdcd"
            >
              <path
                d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8l-6-6z"
              />
              <path d="M14 2v6h6" />
              <path d="M16 13H8" />
              <path d="M16 17H8" />
              <path d="M10 9H8" />
            </svg>
            <svg
              v-if="accept.includes('image')"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="#cdcdcd"
            >
              <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
              <circle cx="8.5" cy="8.5" r="1.5" />
              <polyline points="21 15 16 10 5 21" />
            </svg>
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
const emit = defineEmits(["change"]);
const props = defineProps({
  accept: { type: String, default: "" },
});

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
