<script setup>
// Searchable user picker for admin filters — replaces raw "user id" number inputs.
// Searches via GET /admin/users?search=..., emits the selected user's id via v-model.
// NOTE: buildImageUrl comes from ~/services/help — NOT auto-imported by Nuxt.
import { buildImageUrl } from "~/services/help";

const props = defineProps({
  placeholder: { type: String, default: "المستخدم" },
});

const model = defineModel({ type: Number, default: null });

const container = ref(null);
const open = ref(false);
const query = ref("");
const results = ref([]);
const searching = ref(false);
const highlightedIndex = ref(-1);
let debounceTimer = null;

const typeLabel = { individual: "فرد", entity: "جهة", admin: "مدير" };

const selectedUser = computed(() =>
  model.value == null ? null : results.value.find((u) => u.id === model.value) || null
);

const search = async () => {
  searching.value = true;
  try {
    const params = { limit: 10 };
    if (query.value.trim()) params.search = query.value.trim();
    const { data } = await useApi().get("/admin/users", params);
    results.value = data?.items || [];
  } catch {
    results.value = [];
  } finally {
    searching.value = false;
  }
};

const onInput = () => {
  model.value = null; // typing clears the previous selection
  open.value = true;
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(search, 300);
};

const selectUser = (user) => {
  model.value = user.id;
  query.value = user.name;
  open.value = false;
};

const clear = () => {
  model.value = null;
  query.value = "";
  results.value = [];
};

const onKeydown = (e) => {
  if (e.key === "Escape") {
    open.value = false;
    return;
  }
  if (e.key === "ArrowDown" || e.key === "ArrowUp") {
    e.preventDefault();
    const dir = e.key === "ArrowDown" ? 1 : -1;
    const next = highlightedIndex.value + dir;
    if (next >= 0 && next < results.value.length) highlightedIndex.value = next;
    return;
  }
  if (e.key === "Enter") {
    if (highlightedIndex.value >= 0 && results.value[highlightedIndex.value]) {
      selectUser(results.value[highlightedIndex.value]);
    }
  }
};

const handleClickOutside = (e) => {
  if (container.value && !container.value.contains(e.target)) open.value = false;
};

onMounted(() => document.addEventListener("click", handleClickOutside));
onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
  clearTimeout(debounceTimer);
});

defineExpose({ clear });
</script>

<template>
  <div class="user-search relative" ref="container">
    <div class="relative">
      <input
        v-model="query"
        type="text"
        :placeholder="placeholder"
        class="h-[40px] w-full border rounded-2xl px-4 pe-10 text-sm bg-bg-light outline-none focus:border-primary transition"
        @input="onInput"
        @focus="open = true"
        @keydown="onKeydown"
      />
      <button
        v-if="model != null"
        type="button"
        class="absolute end-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500 transition"
        aria-label="مسح"
        @click.stop="clear"
      >
        ✕
      </button>
      <span v-else class="absolute end-3 top-1/2 -translate-y-1/2 pointer-events-none">
        <Search />
      </span>
    </div>

    <!-- selected summary -->
    <p v-if="selectedUser" class="text-xs text-green-700 mt-1">
      تم الاختيار: {{ selectedUser.name }} ({{ typeLabel[selectedUser.type] || selectedUser.type }})
    </p>

    <transition name="slide-fade">
      <div v-if="open" class="dropdown absolute z-50 mt-1 w-full bg-white border rounded-xl shadow-lg max-h-[240px] overflow-y-auto">
        <p v-if="searching" class="p-3 text-xs text-muted text-center">جارٍ البحث...</p>
        <template v-else-if="results.length">
          <button
            v-for="(user, index) in results"
            :key="user.id"
            type="button"
            class="w-full text-right p-3 border-b last:border-0 hover:bg-bg-light transition flex items-center gap-2"
            :class="{ 'bg-bg-light': highlightedIndex === index }"
            @click="selectUser(user)"
          >
            <img
              :src="buildImageUrl(user.avatarUrl, '/images/avatar.png')"
              class="w-8 h-8 rounded-full object-cover"
              alt=""
            />
            <span class="flex-1 min-w-0">
              <span class="block text-sm font-semibold truncate">{{ user.name }}</span>
              <span class="block text-xs text-muted truncate">{{ user.email }}</span>
            </span>
            <span class="text-xs px-2 py-0.5 rounded-full bg-bg-light text-muted whitespace-nowrap">
              {{ typeLabel[user.type] || user.type }}
            </span>
          </button>
        </template>
        <p v-else class="p-3 text-xs text-muted text-center">لا توجد نتائج</p>
      </div>
    </transition>
  </div>
</template>

<style scoped>
.slide-fade-enter-active { transition: all 0.15s ease-out; }
.slide-fade-leave-active { transition: all 0.1s ease-in; }
.slide-fade-enter-from,
.slide-fade-leave-to { transform: translateY(-4px); opacity: 0; }
</style>
