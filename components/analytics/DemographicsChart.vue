<script setup lang="ts">
import { genderLabel, pct } from '~/services/analyticsLabels'
import { genderColor } from '~/services/analyticsColors'

const props = defineProps<{
  title: string
  data: Record<string, number>
  kind: 'gender' | 'nationality'
}>()

const total = computed(() => Object.values(props.data).reduce((a, b) => a + b, 0))

function labelFor(key: string): string {
  return props.kind === 'gender' ? genderLabel(key) : key
}

function barColor(key: string): string {
  return props.kind === 'gender' ? genderColor(key) : '#ECB42B'
}
</script>

<template>
  <div class="bg-white rounded-xl border p-5 sm:p-6">
    <h2 class="text-base font-semibold text-dark mb-4">{{ title }}</h2>
    <div v-if="!Object.keys(data).length" class="text-center text-sm text-muted py-6">لا توجد بيانات</div>
    <div v-else class="space-y-3">
      <div v-for="(count, key) in data" :key="String(key)" class="flex items-center gap-3">
        <span class="text-xs text-muted w-20 shrink-0 text-right truncate" :title="String(key)">
          {{ labelFor(String(key)) }}
        </span>
        <div class="flex-1 h-5 bg-bg-light rounded-full overflow-hidden">
          <div
            class="h-full rounded-full flex items-center justify-end pe-2 transition-all"
            :style="{ width: pct(count as number, total), backgroundColor: barColor(String(key)) }"
          >
            <span v-if="(count as number) > 0" class="text-[11px] font-bold text-white">{{ count }}</span>
          </div>
        </div>
        <span class="text-xs font-semibold text-dark w-12 text-left">{{ pct(count as number, total) }}</span>
      </div>
    </div>
  </div>
</template>
