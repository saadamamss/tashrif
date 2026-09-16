<template>
  <header class="fixed top-0 left-0 right-0 z-[100]">
    <nav class="relative">
      <div
        class="max-wrapper py-2 px-4 rounded-b-3xl bg-white flex justify-between items-center shadow-sm"
      >
        <div class="flex items-center">
          <div class="hidden lg:flex search relative items-center">
            <input
              class="w-full ps-10 max-w-[300px] h-[48px] border bg-bg-light py-2 text-sm rounded-xl focus:outline-none focus:border-primary transition"
              type="text"
            />
            <span
              class="flex items-center justify-center bg-white shadow-sm block h-[28px] w-[28px] rounded-lg absolute right-[10px]"
            >
              <Search />
            </span>
          </div>

          <button
            class="block lg:hidden text-gray-600 hover:text-gray-900"
            @click="$emit('toggleSide')"
            aria-label="فتح القائمة الجانبية"
          >
            <Hamburger />
          </button>
        </div>
        <div class="flex items-center space-x-4 space-x-reverse">
          <nuxt-link to="/">
            <span
              class="block w-[55px] h-[55px] sm:w-[60px] sm:h-[60px] md:w-[80px] md:h-[80px] logo"
            >
              <Logo />
            </span>
          </nuxt-link>
        </div>
        <div class="flex gap-3">
          <!-- :prevent="true" -->
          <DropDown
            prevent
            label="الإشعارات"
            trigger-style="bg-bg-light rounded-full border-2 border-[#fff]/0 active:border-primary"
          >
            <template #trigger>
              <span class="block w-12 h-12 flex items-center justify-center relative">
                <BellIcon />
                <span
                  v-if="unreadCount > 0"
                  class="absolute top-1 start-1 w-5 h-5 bg-red-500 text-white text-[10px] rounded-full flex items-center justify-center"
                >
                  {{ unreadCount > 9 ? '+9' : unreadCount }}
                </span>
              </span>
            </template>
            <template #list>
              <ul v-if="notifications.length > 0" class="min-w-[280px] max-h-[400px] overflow-y-auto">
                <li
                  v-for="n in notifications"
                  :key="n.id"
                  class="p-3 border-b last:border-b-0 hover:bg-bg-subtle cursor-pointer"
                  :class="{ 'bg-bg-light': !n.isRead }"
                  @click="markAsRead(n.id)"
                >
                  <p class="text-sm font-medium">{{ n.title }}</p>
                  <p v-if="n.body" class="text-xs text-gray-500 mt-1">{{ n.body }}</p>
                  <p class="text-[10px] text-gray-400 mt-1">{{ formatTime(n.createdAt) }}</p>
                </li>
                <li v-if="hasMore" class="p-2 text-center">
                  <button
                    :disabled="loadingMore"
                    class="text-xs text-primary hover:underline disabled:opacity-50"
                    @click.stop="loadMore"
                  >
                    {{ loadingMore ? 'جاري التحميل...' : 'عرض المزيد' }}
                  </button>
                </li>
                <li v-else class="p-2 text-center">
                  <p class="text-[10px] text-gray-400">لا يوجد المزيد</p>
                </li>
              </ul>
              <ul v-else>
                <li>
                  <div class="flex flex-col p-3">
                    <p class="text=base">لا يوجد تنبيهات</p>
                  </div>
                </li>
              </ul>
            </template>
          </DropDown>

          <DropDown
            trigger-style="bg-bg-light rounded-full border-2 border-[#fff]/0 active:border-primary"
          >
            <template #trigger>
              <span>
                <img 
                :src="avatarSrc" alt="صورة المستخدم" class="w-[48px] h-[48px] rounded-full object-cover" />
              </span>
            </template>
            <template #list>
              <ul class="px-0 min-w-[150px]">
                <li>
                  <nuxt-link
                    to="/dashboard/profile"
                    class="block py-2 px-3 hover:bg-bg-subtle text-sm"
                  >
                    الملف الشخصى
                  </nuxt-link>
                </li>
                <li>
                  <nuxt-link
                    to="#"
                    @click.stop.prevent="logout"
                    class="block py-2 px-3 hover:bg-bg-subtle text-sm text-red-500 font-medium"
                  >
                    خروج
                  </nuxt-link>
                </li>
              </ul>
            </template>
          </DropDown>
        </div>
      </div>
    </nav>
  </header>
</template>

<script setup>
import { ref } from "vue";
import Logo from "../icons/logo.vue";
import DropDown from "../elements/DropDown.vue";
import { buildImageUrl } from "~/services/help";

const { logout, user } = useAuth();
const { notifications, unreadCount, loading, loadingMore, hasMore, fetchNotifications, loadMore, addNotification, markAsRead } = useNotifications();
const { start, on, isConnected } = useSignalr();

const avatarSrc = computed(() => buildImageUrl(user.value?.avatarUrl, '/images/profile-image.svg'));

function formatTime(dateStr) {
  const d = new Date(dateStr);
  const now = new Date();
  const diff = now - d;
  if (diff < 60000) return "الآن";
  if (diff < 3600000) return `منذ ${Math.floor(diff / 60000)} دقيقة`;
  if (diff < 86400000) return `منذ ${Math.floor(diff / 3600000)} ساعة`;
  return d.toLocaleDateString("ar-SA");
}

onMounted(async () => {
  // 1. Fetch existing notifications from DB
  await fetchNotifications();

  // 2. Connect to SignalR for live notifications
  if (!isConnected.value) {
    await start();
  }

  on("NewApplication", (data) => {
    addNotification({
      id: data.id || Date.now(),
      userId: user.value?.id ?? 0,
      title: `متقدم جديد على ${data.jobTitle}`,
      body: `${data.applicantName} قام بالتقديم`,
      type: "new_application",
      referenceId: data.applicationId,
      referenceType: "application",
      isRead: false,
      createdAt: new Date().toISOString(),
    });
  });

  on("InterviewScheduled", (data) => {
    addNotification({
      id: data.id || Date.now(),
      userId: user.value?.id ?? 0,
      title: `مقابلة مجدولة`,
      body: `مقابلة لوظيفة ${data.jobTitle} بتاريخ ${data.date}`,
      type: "interview_scheduled",
      referenceId: data.interviewId,
      referenceType: "interview",
      isRead: false,
      createdAt: new Date().toISOString(),
    });
  });

  on("ContractSent", (data) => {
    addNotification({
      id: data.id || Date.now(),
      userId: user.value?.id ?? 0,
      title: `عقد جديد`,
      body: `تم إرسال عقد لوظيفة ${data.jobTitle} من ${data.entityName}`,
      type: "contract_sent",
      referenceId: data.contractId,
      referenceType: "contract",
      isRead: false,
      createdAt: new Date().toISOString(),
    });
  });

  on("ContractSigned", (data) => {
    addNotification({
      id: data.id || Date.now(),
      userId: user.value?.id ?? 0,
      title: `تم توقيع العقد`,
      body: `${data.userName} وقّع العقد لوظيفة ${data.jobTitle}`,
      type: "contract_signed",
      referenceId: data.contractId,
      referenceType: "contract",
      isRead: false,
      createdAt: new Date().toISOString(),
    });
  });
});
</script>

<style scoped lang="scss">
@use "~/assets/scss/components/header";

.transition-colors {
  transition: color 0.2s ease-in-out;
}
.logo svg {
  width: 100%;
  height: 100%;
}
.mobile-nav {
  top: 100%;
}
</style>
