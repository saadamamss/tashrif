<script setup>
import Logo from "~/components/icons/logo.vue";

const props = defineProps({
  error: {
    type: Object,
    default: () => ({ statusCode: 404, message: '' }),
  },
});

useHead({
  title: `خطأ ${props.error.statusCode} | منصة تشريف`,
});

const statusMessages = {
  404: 'الصفحة غير موجودة',
  403: 'ليس لديك صلاحية الوصول',
  500: 'حدث خطأ في الخادم',
};

const statusDescriptions = {
  404: 'عذراً، الصفحة التي تبحث عنها غير موجودة. قد تكون تم إزالتها أو تغيير رابطها.',
  403: 'عذراً، ليس لديك الصلاحية للوصول إلى هذه الصفحة.',
  500: 'عذراً، حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى لاحقاً.',
};

const message = computed(() => {
  if (props.error.message) return props.error.message;
  return statusDescriptions[props.error.statusCode] || 'حدث خطأ غير متوقع';
});
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center px-4 bg-bg-subtle">
    <nuxt-link to="/" class="mb-8">
      <Logo />
    </nuxt-link>

    <div class="text-center max-w-md">
      <h1 class="text-8xl font-bold text-primary mb-4">
        {{ error.statusCode }}
      </h1>
      <h2 class="text-2xl font-semibold text-dark mb-4">
        {{ statusMessages[error.statusCode] || 'خطأ' }}
      </h2>
      <p class="text-muted mb-8 leading-relaxed">
        {{ message }}
      </p>
      <div class="flex gap-4 justify-center">
        <nuxt-link to="/" class="btn-primary text-sm px-8 py-3">
          العودة إلى الرئيسية
        </nuxt-link>
        <button @click="clearError({ redirect: '/' })" class="btn-outline text-sm px-8 py-3">
          إعادة المحاولة
        </button>
      </div>
    </div>
  </div>
</template>
