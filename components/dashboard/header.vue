<template>
  <header class="fixed top-0 left-0 right-0 z-[100]">
    <nav class="relative">
      <div
        class="max-wrapper py-2 px-4 rounded-b-3xl bg-white flex justify-between items-center shadow-sm"
      >
        <div class="flex items-center">
          <div class="hidden lg:flex search relative items-center">
            <input
              class="w-full ps-10 max-w-[300px] h-[48px] border bg-[#f5f5f5] py-2 text-sm rounded-xl focus:outline-none focus:border-[#ecb42b] transition"
              type="text"
            />
            <span
              class="flex items-center justify-center bg-white shadow-sm block h-[28px] w-[28px] rounded-lg absolute right-[10px]"
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M5.99992 0.833008C3.14645 0.833008 0.833252 3.1462 0.833252 5.99967C0.833252 8.85315 3.14645 11.1663 5.99992 11.1663C7.24658 11.1663 8.39012 10.7248 9.28268 9.98959L10.3093 11.0162C10.048 11.5678 10.1455 12.247 10.6019 12.7033L12.6296 14.7311C13.21 15.3114 14.1509 15.3114 14.7313 14.7311C15.3117 14.1507 15.3117 13.2097 14.7313 12.6293L12.7036 10.6016C12.2473 10.1453 11.568 10.0478 11.0163 10.309L9.9898 9.28249C10.725 8.38991 11.1666 7.24636 11.1666 5.99967C11.1666 3.1462 8.85339 0.833008 5.99992 0.833008ZM1.83325 5.99967C1.83325 3.69849 3.69873 1.83301 5.99992 1.83301C8.30111 1.83301 10.1666 3.69849 10.1666 5.99967C10.1666 8.30086 8.30111 10.1663 5.99992 10.1663C3.69873 10.1663 1.83325 8.30086 1.83325 5.99967ZM11.9965 11.3087C11.8066 11.1189 11.4988 11.1189 11.309 11.3087C11.1191 11.4986 11.1191 11.8064 11.309 11.9962L13.3367 14.024C13.5265 14.2138 13.8343 14.2138 14.0242 14.024C14.214 13.8341 14.214 13.5263 14.0242 13.3364L11.9965 11.3087Z"
                  fill="#9DA4AE"
                />
              </svg>
            </span>
          </div>

          <button
            class="block lg:hidden text-gray-600 hover:text-gray-900"
            @click="$emit('toggleSide')"
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
            trigger-style="bg-[#f5f5f5] rounded-full border-2 border-[#fff]/0 active:border-[#ecb42b]"
          >
            <template #trigger>
              <span class="block w-12 h-12 flex items-center justify-center">
                <svg
                  width="20"
                  height="20"
                  viewBox="0 0 20 20"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M2.10828 12.3083C1.93107 13.47 2.72334 14.2763 3.69339 14.6782C7.41235 16.2188 12.5877 16.2188 16.3066 14.6782C17.2767 14.2763 18.069 13.47 17.8917 12.3083C17.7828 11.5944 17.2443 10.9999 16.8453 10.4194C16.3227 9.64971 16.2708 8.81018 16.2707 7.91699C16.2707 4.46521 13.4632 1.66699 10 1.66699C6.53678 1.66699 3.72928 4.46521 3.72928 7.91699C3.72921 8.81018 3.67728 9.64971 3.15468 10.4194C2.75571 10.9999 2.21719 11.5944 2.10828 12.3083Z"
                    fill="#25343E"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M10.0007 1.04199C6.19426 1.04199 3.10499 4.11806 3.10496 7.91694C3.10489 8.80056 3.04561 9.46748 2.63927 10.0669C2.58276 10.149 2.50779 10.2519 2.42587 10.3643C2.2836 10.5596 2.12036 10.7836 1.99654 10.9776C1.77801 11.32 1.56451 11.7329 1.49111 12.2141C1.25165 13.7839 2.35552 14.8002 3.45487 15.2556C4.2733 15.5947 5.15763 15.862 6.08002 16.0578C6.55841 17.7415 8.14141 18.9586 9.99989 18.9586C11.8582 18.9586 13.4411 17.7417 13.9197 16.0581C14.8427 15.8624 15.7276 15.5949 16.5465 15.2556C17.6459 14.8002 18.7497 13.7839 18.5103 12.2141C18.4369 11.733 18.2234 11.32 18.0048 10.9776C17.881 10.7836 17.7178 10.5596 17.5755 10.3643C17.4936 10.252 17.4187 10.1491 17.3622 10.067C16.9558 9.46752 16.8965 8.80067 16.8964 7.91699C16.8964 4.11809 13.8071 1.04199 10.0007 1.04199ZM4.35496 7.91699C4.35496 4.81234 6.88069 2.29199 10.0007 2.29199C13.1207 2.29199 15.6464 4.81239 15.6464 7.91705C15.6465 8.81907 15.691 9.83089 16.3289 10.7705L16.3309 10.7734C16.4358 10.926 16.5279 11.0511 16.6147 11.169C16.7303 11.3261 16.8365 11.4705 16.9511 11.6501C17.1288 11.9285 17.2391 12.1698 17.2746 12.4026C17.3895 13.1562 16.9089 13.7525 16.0681 14.1008C12.5023 15.578 7.49908 15.578 3.93327 14.1008C3.09253 13.7525 2.61185 13.1562 2.72681 12.4026C2.76232 12.1698 2.87254 11.9285 3.05024 11.6501C3.16487 11.4705 3.27108 11.3261 3.38668 11.1691C3.47334 11.0513 3.56574 10.9258 3.67045 10.7734L3.67245 10.7705C4.3104 9.83089 4.35489 8.81902 4.35496 7.91699ZM12.4698 16.3031C10.846 16.5106 9.15359 16.5105 7.52986 16.3029C8.00612 17.1352 8.92781 17.7086 9.99989 17.7086C11.0719 17.7086 11.9935 17.1353 12.4698 16.3031Z"
                    fill="#25343E"
                  />
                </svg>
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
                    to="/dashboard/profile"
                    class="block py-2 px-3 hover:bg-[#f8f9f9] text-sm"
                  >
                    الملف الشخصى
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
      </div>
    </nav>
  </header>
</template>

<script setup>
import { ref } from "vue";
import Logo from "../icons/logo.vue";
import DropDown from "../elements/drop-down.vue";

const authStore = useAuthStore();
const logout = () => {
  authStore.logout();
};
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
