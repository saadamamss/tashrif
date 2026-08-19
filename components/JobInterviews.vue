<template>
  <div>
    <div v-if="selectedApplicants.length">
      <div class="flex justify-between items-center gap-4 mb-4">
        <h2 class="text-sm lg:text-lg font-medium text-muted">
          تم تحديد {{ selectedApplicants.length }} متقدمين
        </h2>
        <button
          class="self-end btn-primary text-sm"
          @click="emit('send-contract', [...selectedApplicants])"
        >
          إرسال العقد
        </button>
      </div>
    </div>
    <div class="grid applicant-grid gap-4">
      <ApplicantCard
        v-for="(applicant, i) in interviewList"
        :key="i"
        :applicant="applicant"
        :action="true"
        :select="true"
        :job-title="jobTitle"
        badge-text="طلب مقابلة"
        badge-style="bg-badge-green/10 text-badge-green"
        card-style=" bg-[#fff]"
        v-model="selectedApplicants"
        @shortlist="emit('shortlist', applicant)"
        @interview="emit('interview', applicant)"
        @delete="emit('delete', applicant)"
      />
    </div>
  </div>
</template>

<script setup>
/** @type {{ interviewList: Array<import('~/types/application').Application>, jobTitle?: string, displayMethod: string }} */
const props = defineProps(["interviewList", "jobTitle", "displayMethod"]);
const selectedApplicants = ref([]);
const emit = defineEmits(['send-contract', 'shortlist', 'delete', 'interview']);
</script>
