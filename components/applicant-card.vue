<template>
  <div class="applicant-card rounded-2xl p-4 sm:p-5" :class="cardStyle">
    <div class="pb-4 border-b-2">
      <div
        class="flex flex-col-reverse gap-y-4 xs:flex-row xs:items-center gap-x-2"
      >
        <input
          v-if="select"
          type="checkbox"
          class="hidden xs:block self-center accent-[#ecb42b] w-4 h-4"
          :value="applicant"
          v-model="model"
        />
        <div class="flex-1 flex items-center gap-2">
          <span
            class="applicant-avatar min-w-12 w-12 lg:w-14 min-h-12 h-12 lg:h-14 rounded-full relative overflow-hidden"
          >
            <img :src="applicant.avatar" class="w-full h-full object-cover" />
          </span>
          <div class="name">
            <span class="text-sm block font-medium text-[#161614] mb-1">
              {{ applicant.name }}
            </span>
            <span class="block text-xs text-[#667178]">
              {{ applicant.jobTitle }}
            </span>
          </div>
        </div>

        <div
          class="flex gap-2 xs:self-start items-center"
          :class="{
            'justify-between': action && select,
            'justify-end': !action || !select,
          }"
        >
          <input
            v-if="select"
            type="checkbox"
            class="xs:hidden self-center accent-[#ecb42b] w-4 h-4"
            :value="applicant"
            v-model="model"
          />
          <div class="flex items-center gap-2">
            <span
              class="block text-xs px-5 py-3 rounded-xl"
              :class="badgeStyle"
            >
              <!-- القائمة المختصرة -->
              {{ badgeText }}
            </span>
            <DropDown
              v-if="action"
              trigger-style="p-2 bg-[#f5f5f5] rounded-xl border border-[#fff]/0 active:border-[#ecb42b]"
            >
              <template #trigger>
                <span>
                  <ListDots />
                </span>
              </template>
              <template #list>
                <ul class="px-0">
                  <li>
                    <a
                      href="#"
                      @click.prevent=""
                      class="text-xs px-3 py-2 hover:bg-[#f8f9f9] transition block"
                      >إضافة للمرشحين</a
                    >
                  </li>
                  <li>
                    <a
                      href="#"
                      @click.prevent=""
                      class="text-xs px-3 py-2 hover:bg-[#f8f9f9] transition block"
                      >إجراء مقابلة</a
                    >
                  </li>
                  <li>
                    <a
                      href="#"
                      @click.prevent=""
                      class="text-xs px-3 py-2 hover:bg-[#f8f9f9] transition block"
                      >حذف</a
                    >
                  </li>
                </ul>
              </template>
            </DropDown>
          </div>
        </div>
      </div>
    </div>
    <div class="details pt-6">
      <div class="grid details-grid gap-y-6">
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <svg
                width="20"
                height="20"
                viewBox="0 0 20 20"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M9.99967 2.70703C8.04367 2.70703 6.45801 4.29269 6.45801 6.2487C6.45801 8.20471 8.04367 9.79036 9.99967 9.79036C11.9557 9.79036 13.5413 8.20471 13.5413 6.2487C13.5413 4.29269 11.9557 2.70703 9.99967 2.70703ZM7.70801 6.2487C7.70801 4.98305 8.73402 3.95703 9.99967 3.95703C11.2653 3.95703 12.2913 4.98305 12.2913 6.2487C12.2913 7.51435 11.2653 8.54036 9.99967 8.54036C8.73402 8.54036 7.70801 7.51435 7.70801 6.2487Z"
                  fill="#696C68"
                />
                <path
                  d="M4.58304 4.3737C3.08727 4.3737 1.87471 5.58626 1.87471 7.08203C1.87471 8.5778 3.08727 9.79036 4.58304 9.79036C4.92822 9.79036 5.20804 9.51054 5.20804 9.16536C5.20804 8.82019 4.92822 8.54036 4.58304 8.54036C3.77763 8.54036 3.12471 7.88745 3.12471 7.08203C3.12471 6.27662 3.77763 5.6237 4.58304 5.6237C4.92822 5.6237 5.20804 5.34388 5.20804 4.9987C5.20804 4.65352 4.92822 4.3737 4.58304 4.3737Z"
                  fill="#696C68"
                />
                <path
                  d="M4.78884 10.7757C4.75773 10.4319 4.45384 10.1785 4.11006 10.2096C3.32796 10.2803 2.5675 10.5812 1.89726 11.0973C1.85864 11.1271 1.80777 11.1641 1.74799 11.2076C1.46409 11.4141 0.979337 11.7668 0.652661 12.1803C0.443926 12.4445 0.248726 12.7903 0.213582 13.2061C0.176777 13.6416 0.322757 14.0646 0.635721 14.4502C1.11477 15.0404 1.7721 15.6237 2.68801 15.6237C3.03319 15.6237 3.31301 15.3439 3.31301 14.9987C3.31301 14.6535 3.03319 14.3737 2.68801 14.3737C2.35508 14.3737 2.0191 14.1711 1.60628 13.6625C1.4628 13.4857 1.4538 13.3746 1.45914 13.3114C1.46614 13.2286 1.51021 13.1112 1.63352 12.9551C1.84123 12.6922 2.13238 12.4782 2.41539 12.2702C2.49836 12.2092 2.58064 12.1488 2.65991 12.0877C3.15188 11.7089 3.6891 11.5028 4.22269 11.4545C4.56646 11.4234 4.81994 11.1195 4.78884 10.7757Z"
                  fill="#696C68"
                />
                <path
                  d="M14.9997 4.3737C14.6545 4.3737 14.3747 4.65352 14.3747 4.9987C14.3747 5.34388 14.6545 5.6237 14.9997 5.6237C15.8051 5.6237 16.458 6.27662 16.458 7.08203C16.458 7.88745 15.8051 8.54036 14.9997 8.54036C14.6545 8.54036 14.3747 8.82019 14.3747 9.16536C14.3747 9.51054 14.6545 9.79036 14.9997 9.79036C16.4954 9.79036 17.708 8.5778 17.708 7.08203C17.708 5.58626 16.4954 4.3737 14.9997 4.3737Z"
                  fill="#696C68"
                />
                <path
                  d="M15.8893 10.2096C15.5455 10.1785 15.2417 10.4319 15.2106 10.7757C15.1794 11.1195 15.4329 11.4234 15.7767 11.4545C16.3103 11.5028 16.8475 11.7089 17.3395 12.0877C17.4187 12.1488 17.501 12.2092 17.5839 12.2702C17.8669 12.4782 18.1582 12.6922 18.3659 12.9551C18.4892 13.1112 18.5332 13.2286 18.5402 13.3114C18.5456 13.3746 18.5366 13.4857 18.3931 13.6625C17.9803 14.1711 17.6443 14.3737 17.3114 14.3737C16.9662 14.3737 16.6864 14.6535 16.6864 14.9987C16.6864 15.3439 16.9662 15.6237 17.3114 15.6237C18.2273 15.6237 18.8846 15.0404 19.3637 14.4502C19.6766 14.0646 19.8226 13.6416 19.7858 13.2061C19.7507 12.7903 19.5555 12.4445 19.3467 12.1803C19.02 11.7668 18.5354 11.4142 18.2515 11.2076C18.1917 11.1642 18.1407 11.1271 18.1021 11.0973C17.4319 10.5812 16.6714 10.2803 15.8893 10.2096Z"
                  fill="#696C68"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M6.4076 12.0598C8.60575 10.7006 11.3938 10.7006 13.592 12.0598C13.6568 12.0998 13.7388 12.1478 13.8331 12.203C14.26 12.4528 14.9378 12.8494 15.4002 13.3194C15.6906 13.6146 15.9803 14.0171 16.0331 14.5188C16.0896 15.0556 15.8618 15.5525 15.4271 15.9825C14.7091 16.6929 13.8197 17.2904 12.6534 17.2904H7.34615C6.17994 17.2904 5.2905 16.6929 4.57248 15.9825C4.13784 15.5525 3.91 15.0556 3.96651 14.5188C4.01932 14.0171 4.30898 13.6146 4.59942 13.3194C5.06184 12.8494 5.7395 12.4529 6.16639 12.2031C6.26071 12.1479 6.34279 12.0998 6.4076 12.0598ZM12.9346 13.1229C11.1393 12.0128 8.86028 12.0128 7.065 13.1229C6.95724 13.1896 6.84168 13.2577 6.72246 13.328C6.2959 13.5795 5.82234 13.8588 5.49049 14.1961C5.28633 14.4036 5.2196 14.555 5.20964 14.6497C5.20337 14.7093 5.20637 14.8513 5.45162 15.0939C6.06206 15.6979 6.65219 16.0404 7.34615 16.0404H12.6534C13.3474 16.0404 13.9375 15.6979 14.548 15.0939C14.7932 14.8513 14.7962 14.7093 14.79 14.6497C14.78 14.555 14.7133 14.4036 14.5091 14.1961C14.1773 13.8588 13.7037 13.5795 13.2772 13.328C13.1579 13.2577 13.0424 13.1896 12.9346 13.1229Z"
                  fill="#696C68"
                />
              </svg>
            </span>
            <span class="text-[#667178]">المؤهل العلمى</span>
          </div>
          <div class="font-bold ps-7">
            {{ applicant.qualification }}
          </div>
        </div>
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <Calender />
            </span>
            <span class="text-[#667178]"> تم التقديم </span>
          </div>
          <div class="font-bold ps-7">
            {{ applicant.applyDate }}
          </div>
        </div>

        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <Gender />
            </span>
            <span class="text-[#667178]">الجنس</span>
          </div>
          <div class="font-bold ps-7">{{ applicant.gender }}</div>
        </div>
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <City />
            </span>
            <span class="text-[#667178]">المدينة</span>
          </div>
          <div class="font-bold ps-7">{{ applicant.city }}</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import DropDown from "./elements/drop-down.vue";
import Calender from "./icons/calender.vue";
import City from "./icons/city.vue";
import Gender from "./icons/gender.vue";
import ListDots from "./icons/list-dots.vue";
const model = defineModel();
const props = defineProps([
  "applicant",
  "badgeText",
  "badgeStyle",
  "cardStyle",
  "select",
  "action",
]);
</script>
