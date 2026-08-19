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
            <button
              class="btn-outline text-sm"
              @click="showShecdualDialog = true"
            >
              جدولة المواعيد
            </button>

            <button
              class="btn-primary text-sm"
              @click="emit('schedule-interview', [...selectedApplicants])"
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
          :job-title="jobTitle"
          badge-text="القائمة المختصرة"
          badge-style="bg-badge-green/10 text-badge-green"
          card-style=" bg-[#fff]"
          v-model="selectedApplicants"
          @shortlist="emit('shortlist', applicant)"
          @interview="emit('interview', applicant)"
          @delete="emit('delete', applicant)"
        />
      </div>
    </div>

    <AppointmentScheduling v-model="showShecdualDialog" />
  </div>
</template>

<script setup>
import AppointmentScheduling from "./AppointmentScheduling.vue";

/** @type {{ shorList: Array<import('~/types/application').Application>, jobTitle?: string, displayMethod: string }} */
const props = defineProps(["shorList", "jobTitle", "displayMethod"]);
const selectedApplicants = ref([]);
const showInterviewDialog = ref(false);
const showShecdualDialog = ref(false);
const emit = defineEmits(['shortlist', 'delete', 'interview', 'schedule-interview']);
</script>
