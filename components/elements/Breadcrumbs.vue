<template>
  <div class="bradcrumb flex">
    <div class="flex" v-for="(item, i) in items" :key="i">
      <nuxt-link
        :to="item.active ? item.to : ''"
        class="text-sm"
        :class="{ 'text-gray-500': !item.active }"
        >{{ item.label }}</nuxt-link
      >
      <span v-if="i + 1 != items.length">
        <ChevronLeftIcon width="24" height="24" color="#BEC2C5" />
      </span>
    </div>
  </div>
</template>
<script setup>
defineProps({
  items: {
    type: Array,
    default: [],
  },
});
</script>
