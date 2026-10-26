<template>
  <div class="py-20 px-4 lg:px-0">
    <div class="max-w-[846px] p-10 rounded-2xl bg-white mx-auto">
      <span class="block mb-8">
        <EmailIcon width="70" height="74" color="#ECB42B" />
      </span>

      <div class="mb-8">
        <h1 class="text-2xl md:text-3xl font-bold mb-6">
          تم إنشاء حسابك بنجاح!
        </h1>
        <p class="text-sm lg:text-base mb-6 text-[#75797C]">
          شكرًا لك على إنشاء حسابك. يرجى التحقق من بريدك الإلكتروني والنقر على رابط التفعيل لتسجيل الدخول.
        </p>
        <p class="text-sm lg:text-base text-[#75797C]">لم تصل الرسالة؟ يمكنك طلب إعادة الإرسال من شاشة تسجيل الدخول. 😊</p>
      </div>

      <div class="flex justify-start">
        <a
          href="#"
          @click.prevent="goToLogin"
          class="btn-primary text-sm lg:px-10"
        >
          الذهاب إلى تسجيل الدخول
        </a>
      </div>
    </div>
  </div>
</template>
<script setup>
useHead({
  title: 'تم إنشاء الحساب',
  meta: [
    { name: "robots", content: "noindex, nofollow" },
  ],
  bodyAttrs: {
    class: "register-page",
  },
});

const router = useRouter();
const { showModal, closeModal } = useLoginModal();
const goToLogin = () => {
  showModal();
  // router.push("/")
};
</script>
