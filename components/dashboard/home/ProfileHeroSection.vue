<script setup>
// 1. imports
import UiSkeleton from '~/components/ui/UiSkeleton.vue'

// 2. props
const props = defineProps({
  avatarSrc: { type: String, required: true },
  name: { type: String, default: '' },
  subtitle: { type: String, default: '' },
  statsItems: { type: Array, default: () => [] },
  columns: { type: Number, default: 4 },
  loading: { type: Boolean, default: false },
  editTo: { type: String, default: '/dashboard/profile' },
})

// 3. reactive state
const gridClass = computed(() =>
  props.columns === 3
    ? 'grid grid-cols-1 sm:grid-cols-3 gap-6 mt-8'
    : 'grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6 mt-8'
)
const skeletonCount = computed(() => props.statsItems.length || props.columns)
</script>

<template>
  <section class="bg-white profile-statistics rounded-xl shadow-sm overflow-hidden min-h-[495px]">
    <div class="container-fluid">
      <div class="back-image h-[197px] overflow-hidden">
        <img
          src="~/assets/images/profile-back.png"
          class="w-full h-full object-cover"
          alt="back-profile"
        />
      </div>
      <div class="px-8 pb-8">
        <div class="flex flex-col sm:flex-row justify-between gap-6 items-center -mt-16">
          <div class="text-center sm:text-right">
            <div
              class="border-4 border-bg-light mx-auto sm:mx-0 rounded-full w-32 h-32 mb-4 overflow-hidden bg-white"
            >
              <UiSkeleton v-if="loading" variant="circle" width="128px" height="128px" />
              <img v-else :src="avatarSrc" class="w-full h-full object-cover" alt="user avatar" />
            </div>
            <div v-if="loading">
              <UiSkeleton variant="rect" width="160px" height="24px" class="mx-auto sm:mx-0 mb-3" />
              <UiSkeleton variant="text" width="112px" height="16px" class="mx-auto sm:mx-0" />
            </div>
            <div v-else>
              <h1 class="text-xl font-bold mb-3">{{ name }}</h1>
              <p class="text-base text-gray-500">{{ subtitle }}</p>
            </div>
          </div>

          <div class="text-center sm:text-right">
            <slot name="actions">
              <NuxtLink
                :to="editTo"
                class="flex items-center justify-center gap-2 bg-bg-light px-4 py-3 rounded-full text-sm border border-primary/0 hover:border-primary transition"
              >
                <EditSmall />
                <span> تعديل الملف الشخصى </span>
              </NuxtLink>
            </slot>
          </div>
        </div>

        <div v-if="loading" :class="gridClass">
          <UiSkeleton
            v-for="i in skeletonCount"
            :key="i"
            variant="rect"
            width="100%"
            height="92px"
          />
        </div>
        <div v-else :class="gridClass">
          <div
            v-for="item in statsItems"
            :key="item.label"
            class="statistic-card p-4 flex justify-between bg-white border border-2 rounded-xl"
          >
            <div>
              <h1 class="md:text-[32px] font-extrabold mb-2 text-[#333] leading-[1.05]">
                {{ item.value }}
              </h1>
              <h3 class="text-sm text-muted">{{ item.label }}</h3>
            </div>
            <span>
              <component :is="item.icon" v-bind="item.iconProps" />
            </span>
          </div>
        </div>

        <slot name="footer" />
      </div>
    </div>
  </section>
</template>
