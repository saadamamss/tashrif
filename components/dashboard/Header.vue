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
              <span class="block w-12 h-12 flex items-center justify-center">
                <BellIcon />
              </span>
            </template>
            <template #list>
              <ul>
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

const avatarSrc = computed(() => buildImageUrl(user.value?.avatarUrl, '/images/profile-image.svg'));
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
