<script setup>
import { buildImageUrl } from "~/services/help";
import EditAdminProfile from "~/components/EditAdminProfile.vue";

// Admin has no individual/entity profile endpoints — data comes from useAuth()
// (kept fresh by /auth/me) and is edited via PUT /admin/profile + /admin/profile/avatar.
const { user, setUser } = useAuth();

const editDialog = ref(false);
const avatarUploading = ref(false);
const fileInput = ref(null);

const avatarSrc = computed(() =>
  buildImageUrl(user.value?.avatarUrl, "/images/avatar.png")
);

const typeLabel = { admin: "مدير النظام" };

const fields = computed(() => [
  { label: "الاسم", value: user.value?.name },
  { label: "البريد الإلكتروني", value: user.value?.email },
  { label: "رقم الجوال", value: user.value?.phone },
  { label: "رقم الهوية", value: user.value?.nationalId },
  { label: "الجنس", value: user.value?.gender === "male" ? "ذكر" : user.value?.gender === "female" ? "أنثى" : user.value?.gender },
  { label: "الجنسية", value: user.value?.nationality },
]);

const handleAvatarChange = async (event) => {
  const file = event.target.files?.[0];
  if (!file) return;
  // reset so selecting the same file twice still triggers change
  event.target.value = "";

  if (!file.type.startsWith("image/")) {
    useToast().show("يرجى اختيار صورة", "error");
    return;
  }
  if (file.size > 5 * 1024 * 1024) {
    useToast().show("حجم الصورة يتجاوز 5 ميجابايت", "error");
    return;
  }

  avatarUploading.value = true;
  const formData = new FormData();
  formData.append("file", file);
  const { data, error } = await useApi().put("/admin/profile/avatar", formData);
  avatarUploading.value = false;

  if (error) {
    useToast().show(error, "error");
    return;
  }
  setUser(data); // flat UserDto → header avatar updates instantly
  useToast().show("تم تحديث الصورة بنجاح", "success");
};

const onSaved = (updatedUser) => {
  setUser(updatedUser); // flat UserDto → header/name update instantly
  editDialog.value = false;
};
</script>

<template>
  <div class="px-4 lg:px-0">
    <section class="bg-white rounded-xl shadow-sm p-4 sm:p-8">
      <div class="flex flex-col sm:flex-row items-center gap-6 mb-8">
        <div class="relative group">
          <div class="w-24 h-24 rounded-full overflow-hidden border-4 border-bg-light bg-white">
            <img :src="avatarSrc" class="w-full h-full object-cover" alt="صورة المدير" />
          </div>
          <label
            class="absolute inset-0 flex items-center justify-center bg-black/40 rounded-full opacity-0 group-hover:opacity-100 cursor-pointer transition"
            :class="{ 'pointer-events-none': avatarUploading }"
            aria-label="تغيير الصورة"
          >
            <UiLoadingSpinner v-if="avatarUploading" class="w-6 h-6 text-white" />
            <ImageIcon v-else />
            <input
              ref="fileInput"
              type="file"
              accept="image/*"
              class="hidden"
              @change="handleAvatarChange"
            />
          </label>
        </div>
        <div class="text-center sm:text-right flex-1">
          <h1 class="text-xl font-bold mb-1">{{ user?.name }}</h1>
          <p class="text-sm text-muted mb-2">{{ user?.email }}</p>
          <span class="text-xs px-3 py-1 rounded-full bg-amber-100 text-amber-700">
            {{ typeLabel[user?.type] || user?.type }}
          </span>
        </div>
        <button class="btn-primary text-sm px-8" @click="editDialog = true">
          تعديل الملف الشخصي
        </button>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div v-for="field in fields" :key="field.label" class="p-4 bg-bg-light rounded-xl">
          <p class="text-xs text-muted mb-1">{{ field.label }}</p>
          <p class="text-sm font-semibold">{{ field.value || "-" }}</p>
        </div>
      </div>

      <div class="flex justify-end mt-8">
        <nuxt-link to="/dashboard/change-password" class="btn-outline text-sm px-8">
          تغيير كلمة المرور
        </nuxt-link>
      </div>
    </section>

    <EditAdminProfile v-model="editDialog" @saved="onSaved" />
  </div>
</template>
