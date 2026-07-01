<template>
  <div>
    <div>
      <div v-if="selectedApplicants.length">
        <div
          class="flex flex-col xs:flex-row xs:justify-between xs:items-center gap-4 mb-4"
        >
          <h2 class="text-sm lg:text-lg font-medium text-[#667178]">
            تم تحديد {{ selectedApplicants.length }} متقدمين
          </h2>
          <div class="flex self-end gap-3">
            <button
              class="btn-outline text-sm"
              @click="showShecdualDialog = true"
            >
              جدولة المواعيد
            </button>

            <button
              class="btn-primary text-sm"
              @click="showInterviewDialog = true"
            >
              إجراء مقابلة
            </button>
          </div>
        </div>
      </div>
      <div class="grid applicant-grid gap-4">
        <ApplicantCard
          v-for="(applicant, i) in shorList"
          :key="i"
          :applicant="applicant"
          :action="true"
          :select="true"
          badge-text="القائمة المختصرة"
          badge-style="bg-[#35685F]/10 text-[#35685F]"
          card-style=" bg-[#fff]"
          v-model="selectedApplicants"
        />
      </div>
    </div>

    <SendInterview
      v-model="showInterviewDialog"
      :applicants="selectedApplicants"
    />
    <AppointmentScheduling v-model="showShecdualDialog" />
  </div>
</template>

<script setup>
import AppointmentScheduling from "./AppointmentScheduling.vue";
import SendInterview from "./SendInterview.vue";

/** @type {{ shorList: Array<import('~/types/application').Application>, displayMethod: string }} */
const props = defineProps(["shorList", "displayMethod"]);
const selectedApplicants = ref([]);
const showInterviewDialog = ref(false);
const showShecdualDialog = ref(false);
</script>
