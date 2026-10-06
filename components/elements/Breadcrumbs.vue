<template>
  <div class="bradcrumb flex">
    <div class="flex items-center gap-1" v-for="(item, i) in items" :key="i">
      <nuxt-link
        :to="item.active ? item.to : ''"
        class="text-sm"
        :class="{ 'text-gray-500': !item.active }"
        >{{ item.label }}</nuxt-link
      >
      <span v-if="i + 1 != items.length" class="me-1">
        <ChevronLeftIcon width="12" height="12" color="#BEC2C5" />
      </span>
    </div>
  </div>
</template>
<script setup>
/** @type {{ items: Array<{label: string, to: string, active: boolean}> }} */
defineProps({
  items: {
    type: Array,
    default: [],
  },
});
</script>
