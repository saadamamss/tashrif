<template>
  <div class="phone-input-wrapper">
    <VueTelInput
      v-model="phone"
      :dropdown-options="dropdownOptions"
      :input-options="inputOptions"
      :valid-characters-only="true"
      @validate="handleValidation"
      @country-changed="handleCountryChange"
      defaultCountry="SA"
      dir="rtl"
    />
    <!-- <div
      v-if="validation"
      class="validation-message text-xs text-red-500 mt-2"
      :class="{ valid: validation.valid }"
    >
      {{ validation.message }}
    </div> -->
  </div>
</template>

<script setup>
import { ref } from "vue";
import { VueTelInput } from "vue-tel-input";
import "vue-tel-input/vue-tel-input.css";

/** @type {{ modelValue: string }} */
const props = defineProps({
  modelValue: {
    type: String,
    default: "",
  },
});

/** @type {import('vue').EmitsOptions} */
const emit = defineEmits(["update:modelValue", "validation", "country-change"]);

const phone = ref(props.modelValue);
const validation = ref(null);

const dropdownOptions = {
  showDialCodeInList: true,
  showDialCodeInInput: true,
  showSearchBox: false,
  showDialCodeInSelection: true,
  showFlags: false,
};

const inputOptions = {
  placeholder: "00",
  required: true,
  autocomplete: "tel",
  styleClasses: "phone-input-field",
};

const handleValidation = (payload) => {
  if (!props.modelValue) return;
  validation.value = {
    valid: payload.valid,
    message: payload.valid ? "" : "رقم غير صحيح",
  };
  emit("validation", payload);
};

const handleCountryChange = (country) => {
  emit("country-change", country);
};

watch(phone, (newVal) => {
  emit("update:modelValue", newVal);
});

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== phone.value) {
      phone.value = newVal;
    }
  }
);
</script>

<style lang="scss" scoped>
.phone-input-wrapper {
  :deep(.vue-tel-input) {
    flex-direction: row-reverse;
    border: none;
    gap: 10px;
    box-shadow: none !important;
    position: relative;
    .vti__dropdown {
      position: static;
      background-color: #f5f5f5;
      border-radius: 16px;
      min-width: 100px;
      padding: 5px 14px;
      transition: box-shadow 0.3s ease;
      &.open {
        box-shadow: 0 0 0 1px rgb(236, 180, 43, 0.4);
      }
      .vti__selection {
        flex-direction: row-reverse;
        justify-content: space-between;
      }
      .vti__dropdown-list {
        background-color: #f5f5f5;
        border-radius: 8px;
        overflow-y: auto;
        direction: ltr;
        width: 100%;
        min-width: 250px;
        max-width: auto;
        &.below {
          top: calc(100% + 4px);
        }
        &::-webkit-scrollbar {
          width: 6px;
        }
        &::-webkit-scrollbar-track {
          background-color: rgb(236, 180, 43, 0.1);
        }
        &::-webkit-scrollbar-thumb {
          background-color: #ecb42b;
        }
        li {
          font-size: 14px;
          padding: 8px 15px;
          font-weight: 100;
          &:hover {
            background-color: #fff;
          }
        }
      }
    }

    .phone-input-field {
      font-size: 14px;
      border: solid 1px transparent;
      border-radius: 16px;
      padding: 5px 10px;
      height: 40px;
      background-color: #f5f5f5;
      direction: ltr;
      text-align: right;
      transition: box-shadow 0.3s ease;
      &:focus {
        box-shadow: 0 0 0 1px rgb(236, 180, 43, 1);
      }
    }
  }
}

.phone-input-field {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.validation-message.valid {
  color: #38a169;
}

.vti__dropdown {
  border-radius: 4px 0 0 4px;
}

.vti__input {
  border-radius: 0 4px 4px 0;
}
</style>
