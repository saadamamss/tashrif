<script lang="ts" setup>
import { formatDate, formatPeriod } from "~/services/help";

const props = defineProps({
    contract: { type: Object, default: () => ({}) },
})

const emits = defineEmits<{
    (e: "openSignContract", contract: object): void
}>();

const isExpired = computed(() => {
    const end = props.contract?.endDate;
    if (!end) return false;
    return new Date(end) <= new Date();
});

const fileSizeText = computed(() => {
    const bytes = Number(props.contract?.fileSize) || 0;
    if (!bytes) return "";
    return `${(bytes / 1024 / 1024).toFixed(2)} Mb`;
});
</script>
<template>
    <div class="interview-card shadow-md bg-white relative overflow-hidden rounded-2xl p-6">
        <div class="pb-4 border-b-2">
            <div class="flex items-center gap-2">
                <span class="company-logo border-2 rounded-md overflow-hidden py-1 px-2">
                    <img :src="contract.entityLogo || '/images/partner-3.svg'" class="w-10 h-6 object-cover" />
                </span>
                <span class="company-name font-medium text-sm text-dark">
                    {{ contract.entityName }}
                </span>
            </div>
        </div>
        <div class="details py-6 flex flex-col gap-4">
            <div class="flex gap-2 items-center">
                <span>
                    <CalenderIcon width="18" height="18" color="#696C68" />
                </span>
                <span class="text-icon-muted text-xs">
                   {{ formatPeriod(contract.createdAt ,contract.endDate) }}
                </span>
            </div>
            <div class="flex gap-2 items-center">
                <span>
                    <BriefcaseIcon width="18" height="18" />
                </span>
                <span class="text-icon-muted text-xs">
                    {{ contract.jobTitle }}
                </span>
            </div>
        </div>

        <div class="p-4 rounded-xl bg-bg-subtle mb-2">
            <div class="flex justify-between gap-4 items-center">
                <div class="flex items-center gap-2 min-w-0">
                    <span class="block p-2 bg-white rounded-xl shrink-0">
                        <Pdf />
                    </span>
                    <div class="flex-1 min-w-0">
                        <span class="block text-slate-900 text-sm mb-1 truncate">
                            {{ contract.fileName }}
                        </span>
                        <span class="text-xs block text-slate-400">
                            {{ fileSizeText }}
                        </span>
                    </div>
                </div>
                <div class="shrink-0">
                    <button class="block shadow-sm p-2 bg-white rounded-lg">
                        <Download />
                    </button>
                </div>
            </div>
        </div>
        <p class="text-xs text-muted mb-6">
            {{ contract.notes || "لا توجد ملاحظات مرفقة بهذا العقد" }}
        </p>

        <div class="flex gap-4">
            <button v-if="contract.status === 'sent' && !isExpired" @click="emits('openSignContract', contract)"
                class="flex-1 text-center btn-primary text-sm">
                توقيع العقد الإلكترونى
            </button>
            <span v-else-if="contract.status === 'sent' && isExpired"
                class="flex-1 text-center text-sm font-bold text-red-500 bg-red-500/10 rounded-xl py-2.5">
                انتهت صلاحية توقيع العقد
            </span>
            <span v-else
                class="flex-1 text-center text-sm font-bold text-badge-green bg-badge-green/10 rounded-xl py-2.5">
                تم توقيع العقد
            </span>
        </div>
    </div>
</template>