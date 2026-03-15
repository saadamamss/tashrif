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
      <svg
        width="18"
        height="17"
        viewBox="0 0 25 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        class="dropdown-icon"
      >
        <path
          d="M7.10372 8.55529C7.2256 8.71666 7.58951 9.1984 7.80624 9.47611C8.24031 10.0323 8.83342 10.7714 9.47323 11.5083C10.1163 12.2489 10.7918 12.9702 11.3919 13.5005C11.6928 13.7663 11.9571 13.9684 12.1753 14.0999C12.3804 14.2235 12.5016 14.2493 12.5016 14.2493C12.5016 14.2493 12.6192 14.2235 12.8244 14.0999C13.0425 13.9684 13.3068 13.7663 13.6078 13.5005C14.2079 12.9702 14.8834 12.2489 15.5264 11.5083C16.1662 10.7714 16.7593 10.0323 17.1934 9.47608C17.4101 9.19837 17.7735 8.7173 17.8954 8.55593C18.1411 8.22241 18.6111 8.15047 18.9446 8.3961C19.2781 8.64173 19.3494 9.11123 19.1037 9.44476L19.1018 9.4473C18.974 9.61655 18.5971 10.1156 18.3759 10.3989C17.9321 10.9677 17.3216 11.7286 16.6591 12.4917C15.9997 13.2511 15.2741 14.0298 14.601 14.6245C14.2653 14.9212 13.925 15.1879 13.5988 15.3845C13.2932 15.5687 12.9063 15.75 12.4998 15.75C12.0933 15.75 11.7064 15.5687 11.4008 15.3845C11.0747 15.1879 10.7344 14.9212 10.3987 14.6245C9.72557 14.0298 8.9999 13.2511 8.34058 12.4917C7.67803 11.7286 7.0676 10.9677 6.62372 10.399C6.40243 10.1154 6.02551 9.61637 5.89793 9.44747L5.89628 9.44528C5.65064 9.11176 5.72154 8.64179 6.05506 8.39615C6.38857 8.15053 6.85808 8.22179 7.10372 8.55529Z"
          fill="#B1B4B0"
        />
      </svg>
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

const props = defineProps({
  items: Array,
  placeholder: String,
  error: String,
  modelValue: [String, Number],
});

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