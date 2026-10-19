<script setup lang="ts">
import type { DailyCount } from '~/types/analytics'

const props = defineProps<{
  days: DailyCount[]
}>()

const max = computed(() => {
  if (!props.days.length) return 1
  return Math.max(...props.days.map((d) => d.count), 1)
})
</script>

<template>
  <div class="bg-white rounded-xl border p-5 sm:p-6">
    <h2 class="text-base font-semibold text-dark mb-4">الطلبات خلال 30 يوماً</h2>
    <div v-if="!days.length" class="text-center text-sm text-muted py-6">لا توجد بيانات</div>
    <template v-else>
      <div class="flex items-end gap-[2px] h-32" role="img" aria-label="الطلبات خلال 30 يوماً">
        <div
          v-for="day in days"
          :key="day.date"
          class="flex-1 rounded-t transition-all"
          :class="day.count > 0 ? 'bg-primary' : 'bg-bg-light'"
          :style="{ height: `${max > 0 ? Math.max((day.count / max) * 100, day.count > 0 ? 8 : 4) : 4}%` }"
          :title="`${day.date}: ${day.count}`"
        />
      </div>
      <div class="flex justify-between text-[10px] text-muted mt-1">
        <span>{{ days[0]?.date }}</span>
        <span>{{ days[days.length - 1]?.date }}</span>
      </div>
    </template>
  </div>
</template>
