<template>
    <svg aria-hidden="true" focusable="false" :class="className" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
    </svg>
</template>

<script setup>
defineProps({
  className: { type: String, default: 'h-6 w-6' },
})
</script>
