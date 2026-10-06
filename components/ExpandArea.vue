<template>
  <div
    class="expand-area overflow-hidden"
    :style="{ '--height': `${contentHeight}px` }"
    :class="{ 'is-expanded': expand }"
  >
    <div ref="contentRef">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";

/** @type {{ expand: boolean }} */
const props = defineProps({
  expand: Boolean,
});

const contentRef = ref(null);
const contentHeight = ref(0);

// Only calculate height when needed
function updateHeight() {
  if (contentRef.value) {
    contentHeight.value = contentRef.value.scrollHeight;
  }
}

// Initial calculation and watch for content changes
// onMounted(updateHeight);
watch(() => props.expand, updateHeight);
</script>

<style scoped>
.expand-area {
  height: var(--height);
  animation: collapsed 0.6s cubic-bezier(0.18, 0.89, 0.32, 1.28) forwards;
}
.expand-area.is-expanded {
  height: 0px;
  animation: expands 0.6s cubic-bezier(0.18, 0.89, 0.32, 1.28) forwards;
}
@keyframes expands {
  99% {
    height: var(--height);
  }
  100% {
    height: auto;
  }
}

@keyframes collapsed {
  100% {
    height: 0px;
  }
}
</style>
