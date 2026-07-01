<script setup>
const props = defineProps({
  percent: {
    type: Number,
    default: 0,
    validator: (value) => value >= 0,
  },
  size: {
    type: Number,
    default: 94,
  },
});
let counter = ref(0);
const animStatus = ref("paused");
//
const progressCounter = (percent) => {
  const delay = Math.round(animationDuration.value / props.percent);
  setInterval(() => {
    if (counter.value == props.percent) {
      clearInterval;
    } else {
      counter.value++;
    }
  }, delay);
};

const animationDuration = computed(() => {
  switch (true) {
    case props.percent < 26:
      return 500;
    case props.percent > 25 && props.percent < 76:
      return 1400;
    case props.percent > 75:
      return 2000;
    default:
      return 2000;
  }
});
const circum = computed(() => {
  return 2 * Math.PI * ((props.size - 10) / 2);
});
onMounted(() => {
  progressCounter(props.percent);
  animStatus.value = "running";
});
</script>

<template>
  <div class="">
    <div class="circle-progress">
      <div class="outer">
        <div class="inner">
          <span class="text-base font-bold"> {{ counter }}% </span>
        </div>
      </div>

      <svg
        xmlns="http://www.w3.org/200/svg"
        version="1.1"
        width="100%"
        height="100%"
      >
        <defs>
          <linearGradient id="GradientColor" x1="0%" y1="100%" x2="0%" y2="0%">
            <stop offset="0%" stop-color="#EBF0EF" />
            <stop offset="50%" stop-color="#ECB42B" />
            <stop offset="100%" stop-color="#ECB42B" />
          </linearGradient>
        </defs>
        <circle
          :cx="size / 2"
          :cy="size / 2"
          :r="(size - 10) / 2"
          stroke-linecap="round"
          stroke="url(#GradientColor)"
          :style="{
            '--circum': circum,
            strokeDasharray: circum,
            strokeDashoffset: circum - (percent / 100) * circum,
            animationPlayState: animStatus,
            animationDuration: animationDuration + 'ms',
          }"
        ></circle>
      </svg>
    </div>
  </div>
</template>
<style lang="scss" scoped>
.circle-progress {
  position: relative;
  width: 94px;
  height: 94px;
  .outer {
    width: 100%;
    height: 100%;
    // box-shadow: 6px 6px 10px -1px rgba($color: #000000, $alpha: 0.15),
    //   -6px -6px 10px -1px rgba($color: #fff, $alpha: 0.7);
    /* Ellipse 824 */
    background-color: #f8f9f9;
    box-shadow: 0px 10.3014px 10.3014px rgba(112, 179, 216, 0.23);

    border-radius: 50%;
    padding: 10px;
    .inner {
      width: 100%;
      height: 100%;
      box-shadow: inset -6px 4px 6px -1px rgba(112, 179, 216, 0.23);
      // box-shadow: inset 4px 4px 10px -1px rgba($color: #000000, $alpha: 0.15),
      //   inset -4px -4px 10px -1px rgba($color: #fff, $alpha: 0.7);
      background-color: #fff;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }

  svg {
    position: absolute;
    top: 0px;
    left: 0px;
    transform: rotate(-90deg);
    circle {
      fill: none;
      stroke-width: 10px;
      animation-name: anim;
      animation-timing-function: linear;
      animation-fill-mode: forwards;
    }
  }
}

@keyframes anim {
  0% {
    stroke-dashoffset: var(--circum);
  }
}
</style>
