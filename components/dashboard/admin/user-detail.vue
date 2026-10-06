<script setup>
import { buildImageUrl } from "~/services/help";

const props = defineProps({
  user: { type: Object, default: null },
});
const emit = defineEmits(["close", "deactivated", "activated"]);

const confirmDialog = ref(false);
const actionLoading = ref(false);

const avatarSrc = computed(() =>
  buildImageUrl(props.user?.avatarUrl, "/images/avatar.png")
);

const typeLabel = computed(() => {
  const labels = { individual: "فرد", entity: "جهة", admin: "مدير النظام" };
  return labels[props.user?.type] || props.user?.type;
});

const deactivate = async () => {
  actionLoading.value = true;
  const { error } = await useApi().put(`/admin/users/${props.user.id}/deactivate`);
  actionLoading.value = false;
  confirmDialog.value = false;
  if (error) {
    useToast().show(error, "error");
    return;
  }
  useToast().show("تم تعطيل الحساب بنجاح", "success");
  emit("deactivated");
};

const activate = async () => {
  actionLoading.value = true;
  const { error } = await useApi().put(`/admin/users/${props.user.id}/activate`);
  actionLoading.value = false;
  if (error) {
    useToast().show(error, "error");
    return;
  }
  useToast().show("تم تفعيل الحساب بنجاح", "success");
  emit("activated");
};

const formatDate = (date) => {
  if (!date) return "-";
  return new Date(date).toLocaleDateString("ar-SA");
};
</script>

<template>
  <div v-if="user" class="bg-white rounded-2xl shadow-md p-4 sm:p-6 md:p-8">
    <div class="flex justify-between items-start mb-6">
      <button
        class="flex items-center gap-2 text-sm text-muted hover:text-primary transition"
        @click="$emit('close')"
      >
        <ChevronRightIcon color="#667178" />
        رجوع للقائمة
      </button>
      <span
        class="text-xs px-3 py-1.5 rounded-full"
        :class="user.isDeleted ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700'"
      >
        {{ user.isDeleted ? "معطل" : "نشط" }}
      </span>
    </div>

    <div class="flex flex-col sm:flex-row items-center gap-6 mb-8">
      <div class="w-24 h-24 rounded-full overflow-hidden border-4 border-bg-light bg-white">
        <img :src="avatarSrc" class="w-full h-full object-cover" alt="صورة المستخدم" />
      </div>
      <div class="text-center sm:text-right">
        <h1 class="text-xl font-bold mb-1">{{ user.name }}</h1>
        <p class="text-sm text-muted mb-2">{{ user.email }}</p>
        <span class="text-xs px-3 py-1 rounded-full bg-bg-light text-dark">{{ typeLabel }}</span>
      </div>
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
      <div class="p-4 bg-bg-light rounded-xl">
        <p class="text-xs text-muted mb-1">رقم الهوية</p>
        <p class="text-sm font-semibold">{{ user.nationalId || "-" }}</p>
      </div>
      <div class="p-4 bg-bg-light rounded-xl">
        <p class="text-xs text-muted mb-1">رقم الجوال</p>
        <p class="text-sm font-semibold">{{ user.phone || "-" }}</p>
      </div>
      <div class="p-4 bg-bg-light rounded-xl">
        <p class="text-xs text-muted mb-1">الجنس</p>
        <p class="text-sm font-semibold">{{ user.gender || "-" }}</p>
      </div>
      <div class="p-4 bg-bg-light rounded-xl">
        <p class="text-xs text-muted mb-1">الجنسية</p>
        <p class="text-sm font-semibold">{{ user.nationality || "-" }}</p>
      </div>
      <div class="p-4 bg-bg-light rounded-xl">
        <p class="text-xs text-muted mb-1">تاريخ التسجيل</p>
        <p class="text-sm font-semibold">{{ formatDate(user.createdAt) }}</p>
      </div>
    </div>

    <div class="flex justify-end gap-3">
      <button
        v-if="!user.isDeleted"
        class="btn-outline text-sm px-8 !border-red-300 !text-red-600 hover:!border-red-500"
        :disabled="actionLoading"
        @click="confirmDialog = true"
      >
        تعطيل الحساب
      </button>
      <button
        v-else
        class="btn-primary text-sm px-8"
        :disabled="actionLoading"
        @click="activate"
      >
        تفعيل الحساب
      </button>
    </div>

    <ConfirmDialog
      v-model="confirmDialog"
      title="تعطيل الحساب"
      message="هل أنت متأكد من تعطيل هذا الحساب؟ لن يستطيع المستخدم تسجيل الدخول بعد التعطيل. يمكن التراجع عن هذه الخطوة لاحقاً."
      confirm-text="تعطيل"
      :is-loading="actionLoading"
      @confirm="deactivate"
    />
  </div>
</template>
