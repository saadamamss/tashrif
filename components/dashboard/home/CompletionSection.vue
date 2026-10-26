<script setup>
// 1. imports
import UiSkeleton from '~/components/ui/UiSkeleton.vue'
import PercentCircle from '~/components/PercentCircle.vue'

// 2. props / emits
defineProps({
  percent: { type: Number, default: 0 },
  loading: { type: Boolean, default: false },
})
const emit = defineEmits(['action'])

// 3. methods
const onAction = () => emit('action')
</script>

<template>
  <section class="complete-profile bg-white rounded-xl shadow-sm mt-4">
    <div class="container-fluid p-8">
      <div class="flex flex-col md:flex-row gap-8 md:gap-6 items-start justify-between">
        <div v-if="loading" class="flex flex-col sm:flex-row gap-6 max-w-[760px] w-full">
          <UiSkeleton variant="circle" width="94px" height="94px" />
          <div class="flex-1">
            <UiSkeleton variant="rect" width="180px" height="20px" class="mb-3" />
            <UiSkeleton variant="text" width="100%" height="14px" class="mb-2" />
            <UiSkeleton variant="text" width="70%" height="14px" />
          </div>
        </div>
        <div v-else class="flex flex-col sm:flex-row gap-6 max-w-[760px]">
          <PercentCircle
            :percent="percent"
            gradient-start="#ECB42B05"
            gradient-middle="#ECB42B01"
            gradient-end="#ECB42B"
            :size="94"
          />
          <div>
            <h1 class="text-base font-semibold mb-3">أستكمل ملفك الشخصي</h1>
            <p class="text-sm text-muted">
              استكمالك لملفك الشخصي يساعدنا في ترشيح الوظائف الأنسب لك، ويزيد
              من فرص قبولك لدى الجهات. أضف معلوماتك الشخصية، مؤهلاتك، وخبراتك
              العملية لتظهر بشكل احترافي أمام أصحاب العمل.
            </p>
          </div>
        </div>
        <div>
          <UiSkeleton v-if="loading" variant="rect" width="192px" height="44px" rounded="rounded-full" />
          <button v-else class="btn-primary text-nowrap text-sm px-8" @click="onAction">
            إستكمل ملفك الشخصى
          </button>
        </div>
      </div>
    </div>
  </section>
</template>
