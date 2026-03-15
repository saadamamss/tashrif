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
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M13.25 8.75L9.75 12L13.25 15.25"
            stroke="#BEC2C5"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
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
