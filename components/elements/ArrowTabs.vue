<template>
  <div class="custom-tabs">
    <!-- Tabs Navigation -->
    <div class="bg-white p-4 sm:p-6 rounded-xl">
      <div class="flex mb-4 tabs-container overflow-x-auto pb-4">
        <ArrowButton
          v-for="(tab, index) in tabs"
          :key="index"
          @click="activeTab = tab.id"
          :class="{
            active: activeTab === tab.id,
          }"
        >
          {{ tab.title }} ({{ tab.number }})
        </ArrowButton>
      </div>

      <div class="block">
        <slot name="filter"></slot>
      </div>
    </div>

    <!-- Tabs Content with Transition -->
    <div class="mt-4 relative overflow-hidden min-h-[200px]">
      <transition name="fade-slide" mode="out-in">
        <div>
          <div v-for="tab in tabs" :key="tab.id">
            <div v-if="activeTab === tab.id">
              <slot :name="tab.id" :tab="tab">
                <!-- Default content if no slot provided -->
                <h3 class="text-lg font-bold mb-4 text-gray-800">
                  {{ tab.title }}
                </h3>
                <div class="text-gray-600">
                  {{ tab.defaultContent || "No content provided" }}
                </div>
              </slot>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import ArrowButton from "./ArrowButton.vue";

/** @type {{ tabs: Array<{id: string, title: string, number?: number}>, initialTab: string|null }} */
const props = defineProps({
  tabs: {
    type: Array,
    required: true,
    validator: (tabs) => tabs.every((tab) => tab.id && tab.title),
  },
  initialTab: {
    type: String,
    default: null,
  },
});

const activeTab = ref(props.initialTab || props.tabs[0]?.id);
</script>

<style scoped lang="scss">
.tabs-container {
  &::-webkit-scrollbar {
    height: 4px;
  }
  &::-webkit-scrollbar-thumb {
    background: rgba(236, 181, 43, 0.595);
  }
  &::-webkit-scrollbar-track {
    background: rgba(236, 181, 43, 0.199);
  }
}
/* Custom transition effects */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}

/* RTL support */
[dir="rtl"] .fade-slide-enter-from {
  transform: translateX(-20px);
}

[dir="rtl"] .fade-slide-leave-to {
  transform: translateX(20px);
}
</style>
