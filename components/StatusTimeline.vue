<script setup lang="ts">
import type { StatusHistoryEntry } from '~/types/status-history'
import { statusLabels, statusBadgeStyles } from '~/services/statusLabels'

const props = defineProps<{
  history: StatusHistoryEntry[]
}>()

function formatDateTime(dateStr: string) {
  return new Date(dateStr).toLocaleString('ar-SA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function getLabel(status: string) {
  return statusLabels[status as keyof typeof statusLabels] || status
}

function getBadgeStyle(status: string) {
  return statusBadgeStyles[status as keyof typeof statusBadgeStyles] || 'bg-muted/10 text-muted'
}
</script>

<template>
  <div class="status-timeline">
    <div v-if="!history.length" class="text-center text-sm text-muted py-4">
      لا يوجد سجل حالات بعد
    </div>
    <div v-else class="relative">
      <!-- Connector line -->
      <div class="absolute top-0 bottom-0 right-[15px] w-0.25 bg-border-light" />

      <div
        v-for="(entry, index) in history"
        :key="entry.id"
        class="relative flex gap-4 pb-6 last:pb-0"
      >
        <!-- Circle marker -->
        <div class="relative z-10 flex-shrink-0">
          <div
            class="w-[31px] h-[31px] rounded-full flex items-center justify-center border-2 border-white shadow-sm"
            :class="index === history.length - 1 ? 'bg-primary' : 'bg-bg-subtle'"
          >
            <span
              v-if="index === history.length - 1"
              class="w-2.5 h-2.5 rounded-full bg-white"
            />
            <span
              v-else
              class="w-2.5 h-2.5 rounded-full bg-border-light"
            />
          </div>
        </div>

        <!-- Content -->
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 flex-wrap">
            <span
              class="text-xs px-2.5 py-1 rounded-lg font-medium"
              :class="getBadgeStyle(entry.newStatus)"
            >
              {{ getLabel(entry.newStatus) }}
            </span>
            <span v-if="entry.oldStatus" class="text-xs text-muted">
              ← {{ getLabel(entry.oldStatus) }}
            </span>
          </div>
          <div class="timeline-meta mt-1.5 text-xs text-muted leading-relaxed">
            <div v-if="entry.changedByName" class="timeline-meta-name font-medium truncate">{{ entry.changedByName }}</div>
            <div class="timeline-meta-date">{{ formatDateTime(entry.changedAt) }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.w-0\.25 {
  width: 1px;
}
</style>
