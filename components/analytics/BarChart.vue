<script setup lang="ts">
export interface BarItem {
  label: string
  value: number
  color?: string
}

const props = withDefaults(
  defineProps<{
    data: BarItem[]
    maxValue?: number
  }>(),
  { maxValue: undefined }
)

const computedMax = computed(() => {
  if (props.maxValue !== undefined) return props.maxValue
  const max = Math.max(...props.data.map((d) => d.value), 1)
  return max
})

function barWidth(value: number): string {
  if (computedMax.value === 0) return '0%'
  return `${Math.round((value / computedMax.value) * 100)}%`
}

function barColor(item: BarItem): string {
  if (item.color) return item.color
  return '#ECB42B'
}
</script>

<template>
  <div class="bar-chart space-y-3" role="img" aria-label="مخطط أعمدة">
    <div
      v-for="(item, index) in data"
      :key="index"
      class="flex items-center gap-3"
    >
      <span class="text-xs text-muted w-24 shrink-0 text-right truncate" :title="item.label">
        {{ item.label }}
      </span>
      <div class="flex-1 h-6 bg-bg-light rounded-full overflow-hidden">
        <div
          class="h-full rounded-full transition-all duration-500 ease-out flex items-center justify-end pe-2"
          :style="{ width: barWidth(item.value), backgroundColor: barColor(item) }"
          role="progressbar"
          :aria-valuenow="item.value"
          :aria-valuemin="0"
          :aria-valuemax="computedMax"
          :aria-label="`${item.label}: ${item.value}`"
        >
          <span v-if="item.value > 0" class="text-[11px] font-bold text-white leading-none">
            {{ item.value }}
          </span>
        </div>
      </div>
      <span class="text-xs font-semibold text-dark w-7 text-left">
        {{ item.value }}
      </span>
    </div>
    <div v-if="!data.length" class="text-center text-sm text-muted py-4">
      لا توجد بيانات للعرض
    </div>
  </div>
</template>
