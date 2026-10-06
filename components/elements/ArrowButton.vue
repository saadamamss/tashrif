<template>
  <button class="inline-block arrow-btn">
    <svg
      width="182"
      height="56"
      viewBox="0 0 182 56"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M22.5293 0.5H178C180.884 0.5 182.53 3.79229 180.8 6.09961L166.4 25.2998C165.2 26.8998 165.2 29.1002 166.4 30.7002L180.8 49.9004C182.53 52.2077 180.884 55.5 178 55.5H22.5293C21.4134 55.5 20.3643 54.9677 19.7051 54.0674L2.13379 30.0674C1.23248 28.8363 1.23248 27.1637 2.13379 25.9326L19.7051 1.93262C20.3643 1.03227 21.4134 0.5 22.5293 0.5Z"
        fill="white"
        stroke="#F5F5F5"
        class="bg"
      />

      <text
        x="50%"
        y="50%"
        dominant-baseline="middle"
        text-anchor="middle"
        fill="#161616"
        font-weight="bold"
        font-size="14"
        class="text"
      >
        <slot />
      </text>
    </svg>
  </button>
</template>

<style scoped lang="scss">
.arrow-btn.active svg .bg{
  fill: #ecb42b;
  stroke: #ecb42b
}

.arrow-btn svg .bg,
.arrow-btn svg .border {
  transition: fill 0.3s ease,stroke 0.3s ease;
}

.svg-button:hover .button-bg {
  fill: #3e8e41;
}
</style>
