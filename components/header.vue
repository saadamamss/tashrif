<template>
  <header class="fixed top-0 left-0 right-0 w-full z-10">
    <nav class="bg-white py-3 sm:py-4 px-6 relative">
      <div class="max-wrapper flex justify-between items-center">
        <!-- الجزء الأيمن -->
        <div class="flex items-center space-x-4 space-x-reverse">
          <nuxt-link to="/">
            <span class="logo">
              <Logo />
            </span>
          </nuxt-link>
          <!-- يمكن إضافة أي عناصر إضافية هنا مثل زر تسجيل الدخول -->
        </div>

        <!-- الجزء الأوسط (القائمة الأفقية) -->
        <div
          class="nav-menu-horizontal hidden md:flex items-center gap-8 md:gap-4 lg:gap-8"
        >
          <a
            href="#home"
            @click.stop.prevent="goToSection"
            :class="{ active: currentSection == 'home' }"
            class="text-sm text-[#25343E] transition-colors"
          >
            الرئيسية
          </a>
          <a
            href="#about"
            :class="{ active: currentSection == 'about' }"
            @click.stop.prevent="goToSection"
            class="text-sm text-[#25343E] transition-colors"
          >
            عن المنصة
          </a>
          <nuxt-link
            to="/jobs"
            :class="{ active: currentSection == 'jobs' }"
            class="text-sm text-[#25343E] transition-colors"
          >
            الوظائف
          </nuxt-link>
          <a
            href="#partners"
            :class="{ active: currentSection == 'partners' }"
            @click.stop.prevent="goToSection"
            class="text-sm text-[#25343E] transition-colors"
          >
            الأخبار
          </a>
          <a
            href="#contact"
            :class="{ active: currentSection == 'contact' }"
            @click.stop.prevent="goToSection"
            class="text-sm text-[#25343E] transition-colors"
          >
            تواصل معنا
          </a>
        </div>
        <!-- الجزء الأيسر -->
        <div class="hidden md:block" v-if="isAuthenticated">
          <!-- <nuxt-link
            to="/dashboard/profile"
            class="w-[48px] h-[48px] flex items-center justify-center bg-[#f5f5f5] rounded-full"
          >
            <img src="~/assets/images/profile-image.svg" />
          </nuxt-link> -->
          <DropDown
            trigger-style="bg-[#f5f5f5] rounded-full border-2 border-[#fff]/0 active:border-[#ecb42b]"
          >
            <template #trigger>
              <span>
                <img src="~/assets/images/profile-image.svg" />
              </span>
            </template>
            <template #list>
              <ul class="px-0 min-w-[150px]">
                <li>
                  <nuxt-link
                    to="/dashboard"
                    class="block py-2 px-3 hover:bg-[#f8f9f9] text-sm"
                  >
                    لوحة التحكم
                  </nuxt-link>
                </li>
                <li>
                  <nuxt-link
                    to="#"
                    @click.stop.prevent="logout"
                    class="block py-2 px-3 hover:bg-[#f8f9f9] text-sm text-red-500 font-medium"
                  >
                    خروج
                  </nuxt-link>
                </li>
              </ul>
            </template>
          </DropDown>
        </div>
        <div
          class="hidden md:flex items-center gap-2"
          v-else
        >
          <button
            @click="
              () => {
                $emit('open-register');
                toggleMobileMenu();
              }
            "
            class="text-center text-sm md:px-4 lg:px-6 btn-outline hidden md:block"
          >
            إنشاء حساب
          </button>
          <button
            @click="
              () => {
                $emit('open-login');
                toggleMobileMenu();
              }
            "
            class="text-center text-sm md:px-4 lg:px-6 btn-primary hidden md:block"
          >
            تسجيل الدخول
          </button>
        </div>
        <div class="md:hidden">
          <button
            class="text-gray-600 hover:text-gray-900"
            @click="toggleMobileMenu"
          >
            <svg
              class="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h16M4 18h16"
              />
            </svg>
          </button>
        </div>
      </div>

      <!-- القائمة المنسدلة للجوال -->
      <transition name="slide-fade">
        <div
          v-if="isMobileMenuOpen"
          class="absolute mobile-nav right-0 w-full md:hidden bg-white py-2 px-4 shadow-md z-10"
        >
          <ul class="px-0 nav-menu-vertical text-center">
            <li class="py-1">
              <nuxt-link
                to="/"
                @click.once="toggleMobileMenu"
                class="py-2 text-sm text-[#25343E]"
              >
                الرئيسية
              </nuxt-link>
            </li>
            <li class="py-1">
              <a
                href="#about"
                @click.stop.prevent="goToSection"
                class="py-2 text-sm text-[#25343E]"
              >
                عن المنصة
              </a>
            </li>
            <li class="py-1">
              <nuxt-link
                to="/jobs"
                @click.once="toggleMobileMenu"
                class="py-2 text-sm text-[#25343E]"
              >
                الوظائف
              </nuxt-link>
            </li>
            <li class="py-1">
              <a
                href="#partners"
                @click.stop.prevent="goToSection"
                class="py-2 text-sm text-[#25343E]"
              >
                الأخبار
              </a>
            </li>
            <li class="py-1">
              <a
                href="#contact"
                @click.stop.prevent="goToSection"
                class="py-2 text-sm text-[#25343E]"
              >
                تواصل معنا
              </a>
            </li>
            <li class="py-2" v-if="isAuthenticated">
              <hr />
            </li>
            <li class="py-1">
              <nuxt-link
                v-if="isAuthenticated"
                to="/dashboard"
                class="text-sm py-2 text-[#25343E]"
              >
                لوحة التحكم
              </nuxt-link>
            </li>
            <li class="py-1">
              <nuxt-link
                v-if="isAuthenticated"
                to="#"
                class="text-sm py-2 font-medium text-red-500"
                @click="logout"
              >
                خروج
              </nuxt-link>
            </li>
          </ul>

          <div
            v-if="!isAuthenticated"
            class="flex items-center justify-center space-x-2 flex-wrap space-x-reverse py-4"
          >
            <button
              @click="
                () => {
                  $emit('openRegister');
                  toggleMobileMenu();
                }
              "
              class="text-center text-sm btn-outline hidden md:block"
            >
              إنشاء حساب
            </button>
            <button
              @click="
                () => {
                  $emit('openLogin');
                  toggleMobileMenu();
                }
              "
              class="text-center text-sm btn-primary hidden md:block"
            >
              تسجيل الدخول
            </button>
          </div>
        </div>
      </transition>
    </nav>
  </header>
</template>

<script setup>
import { ref } from "vue";
import Logo from "./icons/logo.vue";
import DropDown from "./elements/drop-down.vue";
const { currentSection, goToSection: scrollToSection } = useScrollSpy();
const router = useRouter();
const isMobileMenuOpen = ref(false);
const authStore = useAuthStore();
//
const isAuthenticated = computed(() => authStore.isAuthenticated);
//
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value;
};

const dropdown = ref(false);
const logout = () => {
  authStore.logout();
};

const goToSection = (e) => {
  isMobileMenuOpen.value = false;
  scrollToSection(e);
};
</script>

<style scoped lang="scss">
@use "~/assets/scss/components/header";

.transition-colors {
  transition: color 0.2s ease-in-out;
}
.logo {
  @apply block w-[50px] h-[50px] sm:w-[60px] sm:h-[60px] md:w-[80px] md:h-[80px];
  svg {
    width: 100%;
    height: 100%;
  }
}
.mobile-nav {
  top: 100%;
}

header {
  transition: box-shadow 0.3s ease;
  nav {
    .logo {
      transition: height 0.3s ease;
    }
  }
  &.scroll-header {
    nav {
      .logo {
        height: 60px;
      }
    }
    box-shadow: 0 0 10px 0px rgba($color: #000000, $alpha: 0.2);
  }
}

/* Transition effects */
.slide-fade-enter-active {
  transition: all 0.2s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.15s ease-in;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  transform: translateY(-5px);
  opacity: 0;
}
</style>
