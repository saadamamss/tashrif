<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
    <div class="bg-white rounded-2xl p-8 w-full max-w-md mx-4 shadow-xl">
      <h2 class="text-xl font-bold text-dark mb-2">تغيير كلمة المرور</h2>
      <p class="text-muted text-sm mb-6">
        يجب تغيير كلمة المرور الافتراضية قبل الوصول للوحة التحكم
      </p>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-dark mb-1">كلمة المرور الحالية</label>
          <input
            v-model="form.currentPassword"
            type="password"
            class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition"
            placeholder="أدخل كلمة المرور الحالية"
            required
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-dark mb-1">كلمة المرور الجديدة</label>
          <input
            v-model="form.newPassword"
            type="password"
            class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition"
            placeholder="8 أحرف على الأقل"
            minlength="8"
            required
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-dark mb-1">تأكيد كلمة المرور الجديدة</label>
          <input
            v-model="form.confirmPassword"
            type="password"
            class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition"
            placeholder="أعد إدخال كلمة المرور الجديدة"
            minlength="8"
            required
          />
        </div>

        <p v-if="errorMessage" class="text-red-500 text-sm">{{ errorMessage }}</p>
        <p v-if="successMessage" class="text-green-600 text-sm">{{ successMessage }}</p>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-primary text-dark font-bold py-2.5 rounded-lg hover:bg-yellow-500 transition disabled:opacity-50"
        >
          {{ loading ? "جاري التغيير..." : "تغيير كلمة المرور" }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
const { changePassword } = useAuth();

const form = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const loading = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

async function handleSubmit() {
  errorMessage.value = "";
  successMessage.value = "";

  if (form.newPassword !== form.confirmPassword) {
    errorMessage.value = "كلمتا المرور غير متطابقتين";
    return;
  }

  if (form.newPassword.length < 8) {
    errorMessage.value = "كلمة المرور الجديدة يجب أن تكون 8 أحرف على الأقل";
    return;
  }

  loading.value = true;
  const result = await changePassword(form.currentPassword, form.newPassword);
  loading.value = false;

  if (result.error) {
    errorMessage.value = result.error;
  } else {
    successMessage.value = "تم تغيير كلمة المرور بنجاح";
    setTimeout(() => {
      navigateTo("/dashboard");
    }, 1500);
  }
}
</script>
