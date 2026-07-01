<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl overflow-hidden">
      <div class="px-8 py-14">
        <div class="flex justify-center mb-6">
          <span class="p-4 bg-[#F9FAFB] border-2 border-[#F3F4F6] rounded-full">
            <Checkmark />
          </span>
        </div>
        <div class="text-center mx-auto max-w-lg">
          <h2 class="text-base md:text-lg font-semibold mb-3">
            تم نشر الوظيفة بنجاح!
          </h2>
          <p class="text-sm text-muted leading-[1.7]">
            تم نشر وظيفة "مشرف حجاج" بنجاح، وستظهر الآن للباحثين عن العمل ضمن
            الوظائف المتاحة على المنصة.
          </p>
        </div>
      </div>
      <div class="flex justify-between bg-bg-light p-6">
        <button @click="model = false" class="btn-outline text-sm px-10">
          إلغاء
        </button>
        <button @click="routeToHome" class="btn-primary text-sm px-10">
          الإنتقال للصفحة الرئيسية
        </button>
      </div>
    </div>
  </Dialog>
</template>
<script setup>
import Dialog from "./Dialog.vue";

const router = useRouter();
const model = defineModel();
const routeToHome = () => {
  model.value = false;
  setTimeout(() => {
    router.push("/dashboard");
  }, 200);
};
</script>
