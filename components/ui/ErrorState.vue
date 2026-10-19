<script setup>
defineProps({
  title: { type: String, default: 'حدث خطأ' },
  message: { type: String, default: 'تعذر تحميل البيانات. يرجى المحاولة مرة أخرى.' },
  retryText: { type: String, default: 'إعادة المحاولة' },
})

defineEmits(['retry'])
</script>

<template>
  <div class="flex flex-col items-center justify-center py-16 px-4">
    <div class="w-20 h-20 mb-6 text-red-400">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" class="w-full h-full">
        <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
      </svg>
    </div>
    <h3 class="text-lg font-semibold text-gray-700 mb-2">{{ title }}</h3>
    <p class="text-sm text-gray-500 text-center max-w-md mb-6">{{ message }}</p>
    <button
      class="btn-primary text-sm px-6"
      @click="$emit('retry')"
    >
      {{ retryText }}
    </button>
  </div>
</template>
