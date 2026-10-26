<script setup>
import { Form } from "vee-validate";
import TextInput from "./elements/TextInput.vue";
//
const { showModal, closeModal, isLoginModalShow } = useLoginModal();

const auth = useAuth();
const route = useRoute();
const router = useRouter();

const formData = ref({});
const isSubmitting = ref(false);
const showResend = ref(false);
// D8/B-1 fix (2026-10-22 review): resend is keyed by the NATIONAL ID (the login
// credential) — never by email. The composable sends { nationalId }.
const resendNationalId = ref("");
const resending = ref(false);

////
const closeModel = () => {
  closeModal();
};

const handleSubmit = async () => {
  try {
    isSubmitting.value = true;
    auth.error.value = null;

    const result = await auth.login({
      nationalId: formData.value.nationalId,
      password: formData.value.password,
    });
    if (!result.success) {
      useToast().show(auth.error.value || "فشل تسجيل الدخول", "error");
      if (result.requiresVerification) {
        showResend.value = true;
        resendNationalId.value = formData.value.nationalId || "";
      }
      return;
    }

    closeModal();
    useToast().show("تم تسجيل الدخول بنجاح", "success");

    const redirectPath = route.query.redirect || "/dashboard";
    router.replace(redirectPath);
  } catch (error) {
    useToast().show("حدث خطأ غير متوقع", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const handleResend = async () => {
  resending.value = true;
  const result = await auth.resendVerification(resendNationalId.value);
  resending.value = false;
  if (result.success) {
    useToast().show("تم إعادة إرسال رابط التفعيل إلى بريدك الإلكتروني", "success");
    showResend.value = false;
  } else {
    useToast().show(auth.error.value || "فشل إعادة الإرسال", "error");
  }
};
</script>

<template>
  <Transition name="modal">
    <div v-if="isLoginModalShow" class="modal-mask">
      <div class="modal-container" @click.self="closeModel">
        <div class="modal-content">
          <div
            class="content bg-white rounded-2xl px-4 py-6 sm:p-6 md:p-8 lg:p-10"
          >
            <div class="flex border-b pb-6 items-center justify-between">
              <div>
                <h3 class="text-lg md:text-xl lg:text-2xl font-semibold mb-3">
                  تسجيل الدخول
                </h3>
                <p class="text-base text-[#75797C]">
                  ادخل رقم الهوية الوطنية أو الإقامة و كلمة المرور لتسجيل الدخول
                  لحسابك.
                </p>
              </div>
            </div>
            <div class="modal-body pt-6">
              <Form @submit="handleSubmit" v-slot="{ errors }">
                <div
                  class="flex flex-col space-y-4 bg-bg-subtle px-4 lg:px-6 py-8 rounded-lg mb-6"
                >
                  <div class="mb-4">
                    <TextInput
                      name="nationalId"
                      id="nationalId"
                      label="رقم الهوية الوطنية أو الإقامة"
                      height="48"
                      class="bg-white shadow-sm"
                      placeholder="0000000000000"
                      required
                      v-model="formData.nationalId"
                      rules="required|numeric"
                      :error="errors.nationalId"
                    />
                  </div>
                  <div>
                    <TextInput
                      name="password"
                      id="password"
                      label="كلمة المرور "
                      height="48"
                      class="bg-white shadow-sm"
                      placeholder="**********"
                      required
                      v-model="formData.password"
                      rules="required"
                      :error="errors.password"
                    />
                  </div>
                </div>

                <!--  -->
                <div v-if="showResend" class="mt-6 p-4 bg-blue-50 rounded-lg border border-blue-100 text-sm text-blue-900 direction:rtl text-align:right">
                  <p class="mb-3 font-medium">لم يتم تفعيل بريدك الإلكتروني بعد</p>
                  <p class="mb-3">يرجى التحقق من بريدك الإلكتروني والنقر على رابط التفعيل. لم تصل الرسالة؟</p>
                  <div class="flex justify-end gap-2">
                    <button
                      type="button"
                      @click="handleResend"
                      :disabled="resending"
                      class="btn-primary text-xs py-2 px-4"
                    >
                      <span v-if="resending">جاري الإرسال...</span>
                      <span v-else>إعادة إرسال رابط التفعيل</span>
                    </button>
                  </div>
                </div>
                <div v-else>
                  <div class="flex justify-between items-center">
                    <button
                      class="btn-outline text-sm h-[48px]"
                      type="button"
                      @click="closeModel"
                    >
                      إلغاء
                    </button>
                    <button
                      class="btn-primary text-sm h-[48px]"
                      type="submit"
                      :disabled="isSubmitting"
                    >
                      <span v-if="isSubmitting"> انتظر قليلاً... </span>
                      <span v-else> تسجيل دخول </span>
                    </button>
                  </div>
                </div>
              </Form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped lang="scss">
/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

/* Modal Styles */
.modal-mask {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center;
  direction: rtl;
  z-index: 1000;
}

.modal-container {
  @apply fixed inset-0 flex items-center justify-center p-2 sm:p-4;
}

.modal-content {
  @apply max-w-[846px] w-full max-h-[90vh] overflow-y-auto;
  &::-webkit-scrollbar {
    width: 4px;
  }
  &::-webkit-scrollbar-thumb {
    width: 4px;
    background-color: #494949;
  }
  &::-webkit-scrollbar-track {
    width: 4px;
    background-color: #f5f5f5;
  }
}
.modal-header {
  @apply flex justify-between items-center p-4 border-b;
}

.modal-title {
  @apply text-lg font-semibold text-lg text-gray-800;
}

.modal-close-btn {
  @apply text-gray-500 hover:text-gray-700 text-2xl;
}

.modal-footer {
  @apply flex justify-end gap-3 p-4 border-t;
}

.modal-cancel-btn {
  @apply px-4 py-2 text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200;
}

.modal-confirm-btn {
  @apply px-4 py-2 text-white bg-blue-600 rounded-md hover:bg-blue-700;
}
</style>
