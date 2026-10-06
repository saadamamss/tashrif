<template>
  <div class="dropdown-container" ref="dropdownContainer">
    <!-- Trigger Button -->
    <button
      class="dropdown-trigger"
      :class="triggerStyle"
      @click="toggleMenu"
      @blur="onBlur"
      :aria-expanded="isOpen"
      aria-haspopup="true"
      :aria-label="label"
    >
      <slot name="trigger"></slot>
    </button>

    <!-- Dropdown Menu -->
    <transition name="slide-fade">
      <div
        v-show="isOpen"
        class="dropdown-menu"
        ref="dropdownMenu"
        :style="menuPosition"
      >
        <slot name="list" class="list-item"></slot>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";

const props = defineProps({
  label: { type: String, default: undefined },
  prevent: { type: Boolean, default: false },
  items: {
    type: Array,
    default: () => [],
  },
  placeholder: {
    type: String,
    default: "Select an option",
  },
  triggerStyle: {
    type: String,
    default: "",
  },
  modelValue: {
    type: [String, Number, Boolean],
    default: null,
  },
});

/** @type {import('vue').EmitsOptions} */
const emit = defineEmits(["update:modelValue", "change"]);

const isOpen = ref(false);
const dropdownContainer = ref(null);
const dropdownMenu = ref(null);

// Calculate menu position (right-aligned by default)
const menuPosition = computed(() => {
  if (!dropdownContainer.value) return {};

  const containerRect = dropdownContainer.value.getBoundingClientRect();
  const windowWidth = window.innerWidth;
  const menuWidth = 240; // Default menu width

  // Check if menu would overflow on the right
  const rightSpace = windowWidth - containerRect.right;
  const leftSpace = containerRect.left;

  return {
    // width: `${menuWidth}px`,
    left: rightSpace >= menuWidth || rightSpace >= leftSpace ? "0" : "auto",
    right: rightSpace >= menuWidth || rightSpace >= leftSpace ? "auto" : "0",
  };
});

const toggleMenu = () => {
  isOpen.value = !isOpen.value;
};

const selectItem = (item) => {
  if (item.disabled) return;

  emit("update:modelValue", item.value);
  emit("change", item);
  isOpen.value = false;
};

const onBlur = (e) => {
  // Don't close if focus moved to menu
  const relatedTarget = e.relatedTarget;
  if (props.prevent || !dropdownMenu.value?.contains(relatedTarget)) return;

  // isOpen.value = false;
  // relatedTarget.click();
};

const handleClickOutside = (e) => {
  if (!dropdownContainer.value?.contains(e.target)) {
    isOpen.value = false;
  }
  if (!props.prevent && dropdownMenu.value?.contains(e.target)) {
    isOpen.value = false;
  }
};

const handleEscape = (e) => {
  if (e.key === "Escape") {
    isOpen.value = false;
  }
};

// Event listeners
onMounted(() => {
  document.addEventListener("click", handleClickOutside);
  document.addEventListener("keydown", handleEscape);
});

onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
  document.removeEventListener("keydown", handleEscape);
});
</script>

<style scoped>
.dropdown-container {
  position: relative;
  display: inline-block;
}

.dropdown-trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  width: auto;
  height: auto;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.dropdown-trigger:focus {
  outline: none;
}

.trigger-content {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.dropdown-icon {
  display: flex;
  color: #6b7280;
  transition: transform 0.2s ease;
}

.dropdown-icon.open {
  transform: rotate(180deg);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  z-index: 50;
  margin: 0;
  padding: 4px 0;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1),
    0 2px 4px -1px rgba(0, 0, 0, 0.06);
  list-style: none;
  max-height: 300px;
  overflow-y: auto;
  width: max-content;
  
}
.dropdown-menu::-webkit-scrollbar {
  display: none;
}
.dropdown-item {
  padding: 8px 16px;
  font-size: 14px;
  color: #111827;
  cursor: pointer;
  transition: all 0.1s ease;
  user-select: none;
}

.dropdown-item:hover {
  background-color: #f3f4f6;
}

.dropdown-item.selected {
  background-color: #eff6ff;
  color: #1d4ed8;
}

.dropdown-item.disabled {
  color: #9ca3af;
  cursor: not-allowed;
  background-color: transparent;
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
