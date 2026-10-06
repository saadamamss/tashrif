<template>
  <div class="custom-tabs">
    <!-- Tabs Navigation -->
    <div class="flex gap-4">
      <button
        v-for="(tab, index) in tabs"
        :key="index"
        @click="activeTab = tab.id"
        class="px-4 py-3 text-xs md:text-sm font-medium relative focus:outline-none rounded-full"
        :class="{
          'bg-[#FFF9EB] text-slate-900 border border-[#FFDA85]':
            activeTab === tab.id,
          'bg-[#FFFFFF] text-slate-900': activeTab !== tab.id,
        }"
      >
        {{ tab.title }}
        <!-- <span
          v-if="activeTab === tab.id"
          class="absolute bottom-0 left-0 right-0 h-0.5 bg-primary transition-all duration-300"
        ></span> -->
      </button>
    </div>

    <!-- Tabs Content with Transition -->
    <div class="mt-4 relative overflow-hidden min-h-[200px]">
      <transition name="fade-slide" mode="out-in">
        <div>
          <div v-for="tab in tabs" :key="tab.id">
            <div v-show="activeTab === tab.id">
              <slot :name="tab.id" :tab="tab">
                <!-- Default content if no slot provided -->
                <h3 class="text-lg font-bold mb-4 text-grey-800">
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
/** @type {{ tabs: Array<{id: string, title: string}>, initialTab: string|null }} */
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

<style>
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
