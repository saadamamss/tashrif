<script setup lang="ts">
import BarChart from '~/components/analytics/BarChart.vue'
import { statusLabels } from '~/services/analyticsLabels'
import { statusColors } from '~/services/analyticsColors'

const props = defineProps<{
  byStatus: Record<string, number>
}>()

const statusOrder = ['new', 'shortlisted', 'interview', 'contract_sent', 'accepted', 'refused', 'withdrawn'] as const

const funnelData = computed(() =>
  statusOrder.map((key) => ({
    label: (statusLabels as Record<string, string>)[key] || key,
    value: props.byStatus[key] ?? 0,
    color: (statusColors as Record<string, string>)[key],
  })),
)
</script>

<template>
  <div class="bg-white rounded-xl border p-5 sm:p-6">
    <h2 class="text-base font-semibold text-dark mb-4">مسار التوظيف</h2>
    <BarChart :data="funnelData" />
  </div>
</template>
