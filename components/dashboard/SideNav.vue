<template>
  <nav
    class="side-nav w-[260px] fixed pt-[80px] sm:pt-[90px] md:pt-[110px] z-[80]"
    :class="{ open: props.openSideNav }"
    aria-label="التنقل الجانبي"
  >
    <div class="content bg-white shadow rounded-xl border">
      <div class="list p-6">
        <ul class="px-0 space-y-2">
          <li v-for="(item, index) in listItems" key="index">
            <nuxt-link
              :to="item.to"
              @click="$emit('toggleSide')"
              class="flex gap-2 h-[48px] items-center rounded-xl px-3"
              :class="{
                'bg-primary': $route.name.startsWith(item.name),
                'bg-[#ecb42a]':
                  $route.name == 'dashboard' && item.name == 'dashboard-index',
              }"
              :aria-current="$route.name.startsWith(item.name) ? 'page' : undefined"
            >
              <component :is="item.icon"></component>
              <span class="text-sm">{{ item.label }} </span>
            </nuxt-link>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>
<script setup>
import File from "~/components/icons/file.vue";
import Home from "~/components/icons/home.vue";
import JobRequest from "~/components/icons/job-request.vue";
import Jobs from "~/components/icons/jobs.vue";
import Person from "~/components/icons/person.vue";
import Shake from "~/components/icons/shake.vue";

const { userType } = useAuth();
/** @type {{ openSideNav: boolean }} */
const props = defineProps(["openSideNav"]);
const additionalItems = computed(() => {
  if (userType.value === "individual") {
    return [
      {
        label: "استكشاف الوظائف",
        icon: Jobs,
        to: "/dashboard/jobs-explore",
        name: "dashboard-jobs-explore",
      },
      {
        label: "طلبات العمل",
        icon: JobRequest,
        to: "/dashboard/job-requests",
        name: "dashboard-job-requests",
      },
    ];
  } else {
    return [
      {
        label: "وظائفى المنشورة",
        icon: Jobs,
        to: "/dashboard/published-jobs",
        name: "dashboard-published-jobs",
      },
    ];
  }
});
const listItems = computed(() => [
  {
    label: "الصفحة الرئيسية",
    icon: Home,
    to: "/dashboard",
    name: "dashboard-index",
  },
  ...additionalItems.value,
  {
    label: "عقود العمل",
    icon: File,
    to: "/dashboard/employment-contracts",
    name: "dashboard-employment-contracts",
  },
  {
    label: "مقابلات العمل",
    icon: Shake,
    to: "/dashboard/interviews",
    name: "dashboard-interviews",
  },
  {
    label: "الملف الشخصى",
    icon: Person,
    to: "/dashboard/profile",
    name: "dashboard-profile",
  },
]);
</script>
<style scoped lang="scss">
.side-nav {
  right: calc(calc(100% - 1460px) / 2);
  transition: opacity 0.3s ease, right 0.3s ease;
  // transition-delay: 0.4s;
  @media (max-width: 1480px) {
    right: 0px;
  }
  @media (max-width: 991px) {
    right: -300px;
    &.open {
      right: 0px;
    }
  }

  &.hide_side {
    transition: opacity 0.3s ease, right 0.3s ease;
    right: -300px;
    opacity: 0;
  }

  // &.open.hide_side {
  //   @media (min-width: 991px) {
  //     transition: opacity 0.3s ease, right 0.3s ease;
  //     right: -300px;
  //     opacity: 0;
  //   }
  // }

  .content {
    height: calc(100vh - 120px);
    @media (max-width: 768px) {
      height: calc(100vh - 100px);
    }
  }
}
</style>
