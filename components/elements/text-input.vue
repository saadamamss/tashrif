<template>
  <div>
    <label for="firstName" class="block text-sm mb-2">
      {{ label }}
      <span v-if="required" class="text-red-400">*</span>
    </label>
    <div class="relative">
      <Field
        :name="name"
        :id="id"
        :label="label"
        :rules="rules"
        v-model="model"
        class="relative"
      >
        <input
          :type="type"
          class="text-sm rounded-2xl w-full h-[38px] px-2 bg-[#f5f5f5] border border-[#ECB42B]/0 focus:border-[#ECB42B] outline-none placeholder:text-xs transition duration-300"
          v-model="model"
          :placeholder="placeholder"
          :class="[props.class, { invalid: error }]"
          :style="{ height: `${height}px` }"
        />
        <slot></slot>
      </Field>
    </div>
    <ErrorMessage :name="name" class="text-red-500 text-xs mt-1" />
  </div>
</template>
<script setup>
import { Field, Form, ErrorMessage, defineRule } from "vee-validate";
defineOptions({
  inheritAttrs: false,
});
const model = defineModel();
const props = defineProps({
  id: {
    type: String,
    default: "",
  },
  error: {
    type: String,
    default: "",
  },
  label: { type: String, default: "" },
  name: { type: String, default: "" },
  error: { type: String, default: "" },
  rules: { type: String, default: "" },
  placeholder: { type: String, default: "" },
  type: { type: String, default: "" },
  class: { type: String, default: "" },
  required: {
    type: Boolean,
    default: false,
  },
  height: { type: String, default: "40" },
});
</script>
<style scoped lang="scss">
input {
  line-height: 3;
}
input.invalid {
  border-color: rgb(249, 67, 67);
}
input::placeholder {
  padding: 6px;
}
</style>
