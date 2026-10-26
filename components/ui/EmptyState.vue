<script setup>
defineProps({
  title: { type: String, default: 'لا توجد نتائج' },
  description: { type: String, default: 'لم يتم العثور على أي بيانات' },
  ctaText: { type: String, default: '' },
  ctaLink: { type: String, default: '' },
})

defineEmits(['cta-click'])
</script>

<template>
  <div class="flex flex-col items-center justify-center py-16 px-4">
    <div class="w-20 h-20 mb-6 text-gray-300">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" class="w-full h-full">
        <path stroke-linecap="round" stroke-linejoin="round" d="M20.25 7.5l-.625 10.632a2.25 2.25 0 01-2.247 2.118H6.622a2.25 2.25 0 01-2.247-2.118L3.75 7.5m6 4.125l2.25 2.25m0 0l2.25 2.25M12 11.625l2.25-2.25M12 11.625l-2.25 2.25M3.375 7.5h17.25c.621 0 1.125-.504 1.125-1.125v-1.5c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125z" />
      </svg>
    </div>
    <h3 class="text-lg font-semibold text-gray-700 mb-2">{{ title }}</h3>
    <p class="text-sm text-gray-500 text-center max-w-md mb-6">{{ description }}</p>
    <nuxt-link
      v-if="ctaText && ctaLink"
      :to="ctaLink"
      class="btn-primary text-sm px-6"
      @click="$emit('cta-click')"
    >
      {{ ctaText }}
    </nuxt-link>
    <button
      v-if="ctaText && !ctaLink"
      class="btn-primary text-sm px-6"
      @click="$emit('cta-click')"
    >
      {{ ctaText }}
    </button>
  </div>
</template>
