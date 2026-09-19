<template>
  <div class="applicant-card rounded-2xl p-4 sm:p-5" :class="cardStyle">
    <div class="pb-4 border-b-2">
      <div
        class="flex flex-col-reverse gap-y-4 xs:flex-row xs:items-center gap-x-2"
      >
        <input
          v-if="select"
          type="checkbox"
          class="hidden xs:block self-center accent-primary w-4 h-4"
          :value="applicant"
          v-model="model"
        />
        <div class="flex-1 flex items-center gap-2">
          <span
            class="applicant-avatar min-w-12 w-12 lg:w-14 min-h-12 h-12 lg:h-14 rounded-full relative overflow-hidden bg-bg-light"
          >
            <img v-if="applicant.avatar" :src="applicant.avatar" class="w-full h-full object-cover" />
            <span
              v-else
              class="flex items-center justify-center w-full h-full text-primary font-bold text-xl"
            >
              {{ (applicant.userName || applicant.name || "").charAt(0) }}
            </span>
          </span>
          <div class="name">
            <span class="text-sm block font-medium text-dark mb-1">
              {{ applicant.userName || applicant.name }}
            </span>
            <span v-if="jobTitle" class="block text-xs text-muted">
              {{ jobTitle }}
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
            class="xs:hidden self-center accent-primary w-4 h-4"
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
              v-if="action && dropdownActions.length"
              trigger-style="p-2 bg-bg-light rounded-xl border border-[#fff]/0 active:border-primary"
            >
              <template #trigger>
                <span>
                  <ListDots />
                </span>
              </template>
              <template #list>
                <ul class="px-0">
                  <li v-for="item in dropdownActions" :key="item.event">
                    <button
                      @click="emit(item.event)"
                      class="text-xs px-3 py-2 hover:bg-bg-subtle transition block w-full text-start"
                      :class="{ 'text-danger': item.danger }"
                    >{{ item.label }}</button>
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
              <PersonIcon width="20" height="20" color="#696C68" />
            </span>
            <span class="text-muted">المؤهل العلمى</span>
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
            <span class="text-muted"> تم التقديم </span>
          </div>
          <div class="font-bold ps-7">
            {{ formatDate(applicant.createdAt) }}
          </div>
        </div>

        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <Gender />
            </span>
            <span class="text-muted">الجنس</span>
          </div>
          <div class="font-bold ps-7">{{ genderLabel }}</div>
        </div>
        <div class="text-xs">
          <div class="flex gap-2 items-center mb-2">
            <span>
              <City />
            </span>
            <span class="text-muted">المدينة</span>
          </div>
          <div class="font-bold ps-7">{{ applicant.userCity || applicant.city }}</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { computed } from "vue";
import DropDown from "./elements/DropDown.vue";
import Calender from "./icons/calender.vue";
import City from "./icons/city.vue";
import Gender from "./icons/gender.vue";
import ListDots from "./icons/list-dots.vue";
import PersonIcon from "./icons/person.vue";
import { formatDate } from "~/services/help";
const model = defineModel();
const emit = defineEmits(['shortlist', 'interview', 'send-contract', 'refuse', 'delete', 'restore', 'view-details']);

/** @type {{ applicant: import('~/types/application').Application, jobTitle?: string, badgeText: string, badgeStyle: string, cardStyle: string, select: boolean, action: boolean, status: string }} */
const props = defineProps([
  "applicant",
  "jobTitle",
  "badgeText",
  "badgeStyle",
  "cardStyle",
  "select",
  "action",
  "status",
]);

const dropdownActions = computed(() => {
  const detailsAction = { label: 'تفاصيل الطلب', event: 'view-details' }
  switch (props.status) {
    case 'new':
      return [
        detailsAction,
        { label: 'إضافة للمرشحين', event: 'shortlist' },
        { label: 'إجراء مقابلة', event: 'interview' },
        { label: 'رفض', event: 'refuse', danger: true },
      ]
    case 'shortlisted':
      return [
        detailsAction,
        { label: 'إجراء مقابلة', event: 'interview' },
        { label: 'رفض', event: 'refuse', danger: true },
      ]
    case 'interview':
      return [
        detailsAction,
        { label: 'إرسال العقد', event: 'send-contract' },
        { label: 'رفض', event: 'refuse', danger: true },
      ]
    case 'refused':
      return [
        detailsAction,
        { label: 'إعادة للمتقدمين', event: 'restore' },
        { label: 'حذف', event: 'delete', danger: true },
      ]
    default:
      return [detailsAction]
  }
});

const genderLabel = computed(() => {
  const value = (props.applicant?.userGender || props.applicant?.gender || "").toLowerCase()
  if (value === "male" || value === "ذكر") return "ذكر"
  if (value === "female" || value === "أنثى" || value === "انثى") return "أنثى"
  return props.applicant?.userGender || props.applicant?.gender || ""
})
</script>
