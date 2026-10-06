<template>
  <div
    class="custom-select h-[40px]"
    ref="selectContainer"
    :class="{ 'is-open': isOpen, error: error }"
    role="combobox"
    :aria-expanded="isOpen"
    aria-haspopup="listbox"
    tabindex="0"
    @keydown.enter.prevent="toggleDropdown"
    @keydown.space.prevent="toggleDropdown"
    @keydown.esc="isOpen = false"
    @keydown.down.prevent="navigateOptions(1)"
    @keydown.up.prevent="navigateOptions(-1)"
  >
    <div class="select-field text-sm" @click="toggleDropdown" ref="selectField">
      <span :class="{ 'text-gray-400 text-xs': !selectedOption }">
        {{ selectedOption || placeholder }}
      </span>
      <Close
        v-if="clearable && hasValue"
        :width="14"
        :height="14"
        bgColor="transparent"
        class="dropdown-icon cursor-pointer"
        role="button"
        aria-label="مسح الاختيار"
        @click.stop="clearSelection"
      />
      <ArrowDownIcon v-else className="dropdown-icon" />
    </div>

    <transition name="slide-fade">
      <div
        class="dropdown-options"
        v-if="isOpen"
        :style="dropdownPosition"
        ref="optionsBox"
        role="listbox"
      >
        <div
          v-for="(option, index) in options"
          :key="option.value"
          @click="selectOption(option)"
          class="option text-sm"
          :class="{ 'is-highlighted': highlightedIndex === index }"
          role="option"
          :aria-selected="modelValue === option.value"
        >
          {{ option.label }}
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";

/**
 * @type {{ items: Array<{value: string|number, label: string}>, placeholder: string, error: string, modelValue: string|number }}
 */
const props = defineProps({
  items: Array,
  placeholder: String,
  error: String,
  modelValue: [String, Number],
  clearable: { type: Boolean, default: true },
});

/**
 * @type {import('vue').EmitsOptions}
 */
const emit = defineEmits(["update:modelValue"]);

const selectContainer = ref(null);
const selectField = ref(null);
const optionsBox = ref(null);
const isOpen = ref(false);
const dropdownDirection = ref("down");
const highlightedIndex = ref(-1);

const options = computed(() => {
  if (!props.items) return [];
  const typeFirst = typeof props.items[0];
  if (typeFirst === "string" || typeFirst === "number") {
    return props.items.map((i) => ({ label: i, value: i }));
  }
  return props.items;
});

const selectedOption = computed(() => {
  const option = options.value.find((opt) => opt.value === props.modelValue);
  return option ? option.label : "";
});

const hasValue = computed(() => {
  return props.modelValue !== "" && props.modelValue !== null && props.modelValue !== undefined;
});

const clearSelection = () => {
  emit("update:modelValue", "");
  isOpen.value = false;
};

const dropdownPosition = computed(() => {
  if (!selectField.value || !isOpen.value) return {};
  const fieldRect = selectField.value.getBoundingClientRect();
  return {
    position: "fixed",
    left: `${fieldRect.left}px`,
    width: `${fieldRect.width}px`,
    [dropdownDirection.value === "down" ? "top" : "bottom"]:
      dropdownDirection.value === "down"
        ? `${fieldRect.bottom}px`
        : `${window.innerHeight - fieldRect.top}px`,
  };
});

const toggleDropdown = () => {
  isOpen.value = !isOpen.value;
  if (isOpen.value) {
    highlightedIndex.value = options.value.findIndex(opt => opt.value === props.modelValue);
    calculatePosition();
  }
};

const calculatePosition = () => {
  if (!selectField.value) return;
  const fieldRect = selectField.value.getBoundingClientRect();
  const spaceBelow = window.innerHeight - fieldRect.bottom;
  dropdownDirection.value = spaceBelow > 200 ? "down" : "up";
};

const selectOption = (option) => {
  emit("update:modelValue", option.value);
  isOpen.value = false;
};

const navigateOptions = (direction) => {
  if (!isOpen.value) {
    toggleDropdown();
    return;
  }
  const newIndex = highlightedIndex.value + direction;
  if (newIndex >= 0 && newIndex < options.value.length) {
    highlightedIndex.value = newIndex;
  }
  if (direction === 0 && highlightedIndex.value !== -1) {
    selectOption(options.value[highlightedIndex.value]);
  }
};

const handleClickOutside = (event) => {
  if (selectContainer.value && !selectContainer.value.contains(event.target)) {
    isOpen.value = false;
  }
};

onMounted(() => {
  document.addEventListener("click", handleClickOutside);
  window.addEventListener("scroll", handleClickOutside, { passive: true });
});

onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
  window.removeEventListener("scroll", handleClickOutside);
});
</script>

<style scoped lang="scss">
@use "~/assets/scss/abstracts/variables" as vars;

.custom-select {
  position: relative;
  width: 100%;
  min-width: 140px;
  max-width: vars.$select-max-width;
  border: 1px solid vars.$select-border-color;
  border-radius: 1rem;
  background-color: vars.$select-bg-color;
  transition: border 0.3s ease;
  cursor: pointer;
  user-select: none;
  &:hover {
    border-color: vars.$select-hover-border-color;
  }

  &:focus {
    border-color: vars.$select-active-border-color;
    outline: none !important;
  }

  &.error {
    border-color: rgb(249, 67, 67);
  }

  .select-field {
    display: flex;
    justify-content: space-between;
    align-items: center;
    color: vars.$select-text-color;
    transition: all 0.3s ease;
    min-height: 100%;
    padding-inline: 12px;
  }

  .dropdown-icon {
    width: 16px;
    height: 16px;
    fill: vars.$select-icon-color;
    transition: transform 0.3s ease;
  }

  &.is-open {
    .dropdown-icon {
      transform: rotate(180deg);
    }

    .select-field {
      &:not(.error) {
        border-color: vars.$select-active-border-color;
      }
    }
  }

  .dropdown-options {
    position: fixed;
    // top: 100%;
    // left: 0;
    // right: 0;
    z-index: 50;
    margin-top: 4px;
    border: 1px solid vars.$select-border-color;
    border-radius: 4px;
    background-color: vars.$select-bg-color;
    box-shadow: vars.$select-dropdown-shadow;
    max-height: 200px;
    overflow-y: auto;
  }

  .option {
    padding: vars.$select-option-padding;
    color: vars.$select-text-color;
    transition: background-color 0.2s ease;

    &:hover {
      background-color: vars.$select-option-hover-bg;
      color: vars.$select-option-hover-color;
    }
  }
}

/* Transition effects */
.slide-fade-enter-active {
  transition: all 0.2s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.15s ease-in;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  transform: translateY(-5px);
  opacity: 0;
}
</style>


<!-- <style scoped lang="scss">
.custom-select {
  position: relative;
  outline: none;
  &:focus-visible {
    border-color: #ECB42B;
    box-shadow: 0 0 0 2px rgba(236, 180, 43, 0.2);
  }
}
.dropdown-options {
  z-index: 9999;
  background: white;
  border: 1px solid #eff2ee;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  max-height: 200px;
  overflow-y: auto;
}
.option {
  padding: 10px 15px;
  cursor: pointer;
  &.is-highlighted, &:hover {
    background-color: #f5f5f5;
  }
}
.dropdown-icon {
  transition: transform 0.3s ease;
}
.is-open .dropdown-icon {
  transform: rotate(180deg);
}
</style> -->