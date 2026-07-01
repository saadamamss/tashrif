<template>
  <div>
    <div v-if="selectedApplicants.length">
      <div class="flex justify-between items-center gap-4 mb-4">
        <h2 class="text-sm lg:text-lg font-medium text-[#667178]">
          تم تحديد {{ selectedApplicants.length }} متقدمين
        </h2>
        <button
          class="self-end btn-primary text-sm"
          @click="showContractDialog = true"
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
        badge-text="طلب مقابلة"
        badge-style="bg-[#35685F]/10 text-[#35685F]"
        card-style=" bg-[#fff]"
        v-model="selectedApplicants"
      />
    </div>

    <SendContract
      v-model="showContractDialog"
      :applicants="selectedApplicants"
    />
  </div>
</template>

<script setup>
import SendContract from "./SendContract.vue";

const props = defineProps(["interviewList", "displayMethod"]);
const selectedApplicants = ref([]);
const showContractDialog = ref(false);
</script>
