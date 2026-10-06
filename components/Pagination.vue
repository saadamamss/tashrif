<template>
  <div
    class="bg-white p-3 border shadow rounded-xl flex flex-col md:flex-row gap-6 justify-between items-center"
  >
    <div class="text-sm text-muted">
      <div class="flex items-center gap-3">
        <CustomSelect v-model="perPageSelect" :items="[9, 12, 15, 21]" :clearable="false" style="min-width: 80px; width: 80px;"/>
        <p>
          تم عرض من {{ (currentPage - 1) * perPage + 1 }} إلى
          {{ Math.min(currentPage * perPage, perPage * totalPages) }} من أصل
          {{ perPage * totalPages }}
        </p>
        <span class="text-sm text-muted">صفحة {{ currentPage }} من {{ totalPages }}</span>
      </div>
    </div>
    <nav class="pagination" aria-label="Pagination">
      <ul class="pagination-list">
        <!-- Previous Button -->
        <li>
          <button
            class="pagination-previous"
            :disabled="currentPage === 1"
            @click="changePage(currentPage - 1)"
            aria-label="Go to previous page"
          >
            <ChevronRightIcon color="#373A36" />
          </button>
        </li>

        <!-- First Page -->
        <li v-if="showFirstPage">
          <button
            class="pagination-link"
            @click="changePage(1)"
            :aria-current="currentPage === 1 ? 'page' : null"
          >
            1
          </button>
        </li>

        <!-- Ellipsis before -->
        <li v-if="showBeforeEllipsis">
          <span class="pagination-ellipsis">...</span>
        </li>

        <!-- Page Numbers -->
        <li v-for="page in visiblePages" :key="page">
          <button
            class="pagination-link"
            :class="{ 'is-active': page === currentPage }"
            @click="changePage(page)"
            :aria-current="page === currentPage ? 'page' : null"
          >
            {{ page }}
          </button>
        </li>

        <!-- Ellipsis after -->
        <li v-if="showAfterEllipsis">
          <span class="pagination-ellipsis">...</span>
        </li>

        <!-- Last Page -->
        <li v-if="showLastPage">
          <button
            class="pagination-link"
            @click="changePage(totalPages)"
            :aria-current="currentPage === totalPages ? 'page' : null"
          >
            {{ totalPages }}
          </button>
        </li>

        <!-- Next Button -->
        <li>
          <button
            class="pagination-next"
            :disabled="currentPage === totalPages"
            @click="changePage(currentPage + 1)"
            aria-label="Go to next page"
          >
            <ChevronLeftIcon color="#667178" />
          </button>
        </li>
      </ul>
    </nav>
  </div>
</template>

<script setup>
import CustomSelect from "./elements/CustomSelect.vue";

/** @type {import('vue').EmitsOptions} */
const emit = defineEmits(["page-changed", "per-page-change"]);

/** @type {{ currentPage: number, perPage: number, totalPages: number, maxVisibleButtons: number }} */
const props = defineProps({
  currentPage: {
    type: Number,
    required: true,
    validator: (value) => value >= 1,
  },
  perPage: {
    type: Number,
    required: true,
    validator: (value) => value >= 1,
  },
  totalPages: {
    type: Number,
    required: true,
    validator: (value) => value >= 1,
  },
  maxVisibleButtons: {
    type: Number,
    default: 5,
    validator: (value) => value >= 3 && value <= 10,
  },
});

const perPageSelect = ref(props.perPage);

watch(perPageSelect, (val) => {
  emit("per-page-change", val);
});

const showFirstPage = computed(() => {
  return props.currentPage > Math.floor(props.maxVisibleButtons / 2) + 1;
});
const showLastPage = computed(() => {
  return (
    props.currentPage <
    props.totalPages - Math.floor(props.maxVisibleButtons / 2)
  );
});
const showBeforeEllipsis = computed(() => {
  return (
    props.currentPage > Math.floor(props.maxVisibleButtons / 2) + 1 &&
    props.totalPages > props.maxVisibleButtons
  );
});
const showAfterEllipsis = computed(() => {
  return (
    props.currentPage <
      props.totalPages - Math.floor(props.maxVisibleButtons / 2) &&
    props.totalPages > props.maxVisibleButtons
  );
});
const visiblePages = computed(() => {
  const half = Math.floor(props.maxVisibleButtons / 2);
  let start = Math.max(1, props.currentPage - half);
  let end = Math.min(props.totalPages, start + props.maxVisibleButtons - 1);

  // Adjust if we're at the end
  if (end === props.totalPages) {
    start = Math.max(1, end - props.maxVisibleButtons + 1);
  }

  return Array.from({ length: end - start + 1 }, (_, i) => start + i);
});

const changePage = (page) => {
  if (page >= 1 && page <= props.totalPages && page !== props.currentPage) {
    emit("page-changed", page);
  }
};
</script>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
}

.pagination-list {
  display: flex;
  list-style: none;
  padding: 0;
  margin: 0;
  gap: 0.5rem;
}

.pagination-link,
.pagination-previous,
.pagination-next {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 2.5rem;
  height: 2.5rem;
  padding: 0 0.5rem;
  border-radius: 0.375rem;
  background-color: white;
  color: #25343e;
  font-weight: 500;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination-link:hover,
.pagination-previous:not(:disabled):hover,
.pagination-next:not(:disabled):hover {
  background-color: #ddd;
  border-color: #ddd;
}

.pagination-link.is-active {
  background-color: #ecb42b;
  border-color: #ecb42b;
}

.pagination-previous,
.pagination-next {
  padding: 0 1rem;
}

.pagination-previous:disabled,
.pagination-next:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-ellipsis {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 2.5rem;
  height: 2.5rem;
  color: #a0aec0;
}

@media (max-width: 640px) {
  .pagination-link:not(.is-active) {
    display: none;
  }

  .pagination-previous,
  .pagination-next {
    padding: 0 0.5rem;
  }
}
</style>
