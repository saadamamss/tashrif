<template>
  <div>
    <div>
      <div v-if="selectedApplicants.length">
        <div
          class="flex flex-col xs:flex-row xs:justify-between xs:items-center gap-4 mb-4"
        >
          <h2 class="text-sm lg:text-lg font-medium text-muted">
            تم تحديد {{ selectedApplicants.length }} متقدمين
          </h2>
          <div class="self-end flex gap-3">
            <!--
              جدولة المواعيد (Appointment Scheduling) — NOT YET IMPLEMENTED
              Batch self-scheduling feature: entity sets a date range + time range
              (e.g. Sept 20-25, 9AM-5PM) and each applicant picks their own
              interview slot within that window. Requires:
              - Backend: new DTO with startDate/endDate/startTime/endTime + batch endpoint
              - Frontend: wire AppointmentScheduling.vue @submit-appointment to the endpoint
              Currently the dialog opens but submit does nothing.
            -->
            <!--
            <button
              class="btn-outline text-sm"
              @click="showShecdualDialog = true"
            >
              جدولة المواعيد
            </button>
            -->

            <button
              class="btn-primary text-sm"
              @click="emit('schedule-interview', [...selectedApplicants])"
            >
              إجراء مقابلة
            </button>

            <button
              class="text-sm py-3 px-4 xs:px-6 rounded-full bg-danger text-white hover:bg-danger/85 transition"
              @click="emit('bulk-refuse', [...selectedApplicants])"
            >
              رفض المحدد
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
          :status="applicant.status"
          :job-title="jobTitle"
          badge-text="القائمة المختصرة"
          badge-style="bg-badge-green/10 text-badge-green"
          card-style=" bg-[#fff]"
          v-model="selectedApplicants"
          @shortlist="emit('shortlist', applicant)"
          @interview="emit('interview', applicant)"
          @refuse="emit('bulk-refuse', [applicant])"
          @view-details="emit('view-details', applicant)"
        />
      </div>
    </div>

    <!-- AppointmentScheduling — see comment above about unimplemented feature -->
    <!-- <AppointmentScheduling v-model="showShecdualDialog" /> -->
  </div>
</template>

<script setup>
// import AppointmentScheduling from "./AppointmentScheduling.vue"; // unused — feature not implemented

/** @type {{ shorList: Array<import('~/types/application').Application>, jobTitle?: string, displayMethod: string }} */
const props = defineProps(["shorList", "jobTitle", "displayMethod"]);
const selectedApplicants = ref([]);
const showInterviewDialog = ref(false);
// const showShecdualDialog = ref(false); // unused — feature not implemented
const emit = defineEmits(['shortlist', 'interview', 'schedule-interview', 'bulk-refuse', 'view-details']);
</script>
