<template>
  <div class="py-24 px-4 lg:px-0">
    <div class="max-w-[846px] p-10 rounded-2xl bg-white mx-auto text-center">
      <span class="block mb-8 inline-block">
        <EmailIcon width="70" height="74" color="#ECB42B" />
      </span>

      <div v-if="loading" class="mb-8">
        <p class="text-sm lg:text-base text-[#75797C]">جاري تفعيل حسابك...</p>
      </div>

      <div v-else-if="verified" class="mb-8">
        <h1 class="text-2xl md:text-3xl font-bold mb-6">تم تفعيل بريدك الإلكتروني!</h1>
        <p class="text-sm lg:text-base mb-6 text-[#75797C]">
          حسابك مفعل الآن، وجاري تحويلك إلى لوحة التحكم...
        </p>
      </div>

      <div v-else-if="error" class="mb-8 text-red-500">
        <h1 class="text-2xl md:text-3xl font-bold mb-6">فشل التفعيل</h1>
        <p class="text-sm lg:text-base mb-6">{{ error }}</p>
        <div class="flex justify-center">
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
  </div>
</template>

<script setup>
// F2 — public verification page. Route MUST stay `/verify-email` (no subfolder):
// the email link A1/D13 targets `{Frontend:BaseUrl}/verify-email?token=…`,
// so this page's path is part of the API contract with the mail template.
useHead({
  title: "تفعيل البريد الإلكتروني",
  meta: [{ name: "robots", content: "noindex, nofollow" }],
  bodyAttrs: { class: "register-page" },
});

const router = useRouter();
const route = useRoute();
const auth = useAuth();

const loading = ref(true);
const verified = ref(false);
const error = ref("");

const goToLogin = () => {
  router.push("/login");
};

const verify = async () => {
  const token = route.query.token;
  if (!token) {
    error.value = "رابط التفعيل غير صالح";
    loading.value = false;
    return;
  }

  try {
    const result = await auth.verifyEmail(token);
    if (result.success) {
      // D7 (user-approved): auto-login on verify — the backend already set the
      // HttpOnly cookies and verifyEmail() ran setUser(), so go straight in.
      verified.value = true;
      setTimeout(() => router.push("/dashboard"), 1200);
    } else {
      error.value = auth.error.value || "فشل تفعيل البريد الإلكتروني";
    }
  } catch (e) {
    error.value = "حدث خطأ غير متوقع";
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  verify();
});
</script>
