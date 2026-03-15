<template>
  <div class="w-full relative overflow-hidden px-5" dir="rtl">
    <div class="flex flex-col items-center justify-center min-h-[550px]">
      <div class="w-full max-w-[846px]">
        <div class="bg-white rounded-3xl shadow-lg overflow-hidden flex w-full">
          <div class="p-10 flex flex-col justify-center w-full">
            <h1 class="text-xl sm:text-2xl font-bold text-right mb-3 text-gray-800">
              الرجاء إدخال رمز OTP
            </h1>
            <p class="text-right text-gray-400 text-sm lg:text-base">
              تم إرسال رمز التحقق (OTP) إلى رقم جوالك. يرجى إدخال الرمز في الحقل
              أدناه لاستكمال عملية التسجيل.
            </p>

            <div class="my-4 h-px bg-gray-200 w-full"></div>

            <form class="mt-4" @submit.prevent="verifyOtp">
              <div class="rounded-md -space-y-px">
                <div class="flex justify-center gap-2 py-3" dir="ltr">
                  <input
                    v-for="(digit, index) in otp"
                    :key="index"
                    v-model="otp[index]"
                    @input="handleInput(index, $event)"
                    @keydown.delete="handleBackspace(index, $event)"
                    @keydown.left="handleArrowKey(index, $event, -1)"
                    @keydown.right="handleArrowKey(index, $event, 1)"
                    @paste="handlePaste($event)"
                    placeholder="0"
                    ref="inputs"
                    type="text"
                    maxlength="1"
                    class="appearance-none font-bold rounded-xl relative block w-16 h-16 px-3 py-2 placeholder-[#B4B6B7] text-gray-900 bg-[#f5f5f5] focus:outline-none focus:z-10 sm:text-base text-center"
                    :class="{ 'border-blue-500': activeIndex === index }"
                  />
                </div>
              </div>

              <div class="flex items-center justify-center py-6">
                <div class="text-center">
                  <p class="text-sm md:text-base text-gray-700 mb-3 font-bold">
                    لم يصلك رمز التحقق؟
                  </p>
                  <button 
                    type="button"
                    @click="resendOtp"
                    class="text-base font-medium text-primary text-[#ECB42B]/80 hover:text-[#ECB42B] focus:outline-none underline"
                    :disabled="resendDisabled"
                    :class="{
                      'cursor-not-allowed': resendDisabled,
                    }"
                  >
                    {{
                      resendDisabled
                        ? `إعادة الإرسال خلال (${countdown}s) ثانية ...`
                        : "إعادة الإرسال"
                    }}
                  </button>
                </div>
              </div>

              <div class="flex justify-between mt-4">
                <nuxt-link 
                  to="/register/individual"
                  @click.once="$emit('cancel')"
                  class="btn-outline md:px-10 md:py-3 text-sm"
                >
                  إلغاء
                </nuxt-link>
                <button 
                  type="submit"
                  class="btn-primary md:px-10 md:py-3 text-sm"
                  :disabled="!isOtpComplete"
                  :class="{ 'opacity-50 cursor-not-allowed': !isOtpComplete }"
                >
                  تأكيد
                </button>
              </div>
            </form>

            <!-- Error message -->
            <div
              v-if="errorMessage"
              class="text-red-500 text-sm text-center mt-2"
            >
              {{ errorMessage }}
            </div>

            <!-- Success message (you can replace this with a redirect in a real app) -->
            <div
              v-if="successMessage"
              class="text-green-500 text-sm text-center mt-2"
            >
              {{ successMessage }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  emits:["cancel"],
  data() {
    return {
      otp: ["", "", "", ""],
      activeIndex: 0,
      errorMessage: "",
      successMessage: "",
      countdown: 30,
      resendDisabled: true,
      timer: null,
    };
  },
  computed: {
    isOtpComplete() {
      return this.otp.every((digit) => digit !== "");
    },
    otpString() {
      return this.otp.join("");
    },
  },
  mounted() {
    // Focus the first input on mount
    this.$refs.inputs[0].focus();
    this.startCountdown();
  },
  beforeUnmount() {
    clearInterval(this.timer);
  },
  methods: {
    handleInput(index, event) {
      const value = event.target.value;

      // Only allow numeric input
      if (!/^\d*$/.test(value)) {
        this.otp[index] = "";
        return;
      }

      // Move to next input if a digit was entered
      if (value && index < this.otp.length - 1) {
        this.$refs.inputs[index + 1].focus();
        this.activeIndex = index + 1;
      }

      // Highlight the current active input
      this.activeIndex = index;
    },
    handleBackspace(index, event) {
      if (!this.otp[index] && index > 0) {
        // Move to previous input if current is empty
        this.$refs.inputs[index - 1].focus();
        this.activeIndex = index - 1;
      }
    },
    handleArrowKey(index, event, direction) {
      const newIndex = index + direction;
      if (newIndex >= 0 && newIndex < this.otp.length) {
        this.$refs.inputs[newIndex].focus();
        this.activeIndex = newIndex;
        event.preventDefault();
      }
    },
    handlePaste(event) {
      event.preventDefault();
      const pasteData = event.clipboardData.getData("text/plain").trim();

      if (/^\d{6}$/.test(pasteData)) {
        // Split the pasted data into individual digits
        const digits = pasteData.split("");
        digits.forEach((digit, i) => {
          if (i < this.otp.length) {
            this.otp[i] = digit;
          }
        });

        // Focus the last input
        const lastIndex = Math.min(this.otp.length - 1, digits.length - 1);
        this.$refs.inputs[lastIndex].focus();
        this.activeIndex = lastIndex;
      }
    },
    verifyOtp() {
      if (!this.isOtpComplete) {
        this.errorMessage = "Please enter the complete OTP code";
        return;
      }

      // In a real app, you would send this to your backend for verification
      console.log("Verifying OTP:", this.otpString);

      // Simulate API call
      setTimeout(() => {
        // For demo purposes, any OTP with all 6 digits the same is considered invalid
        if (new Set(this.otp).size === 1) {
          this.errorMessage = "Invalid OTP code. Please try again.";
          this.successMessage = "";
        } else {
          this.errorMessage = "";
          this.successMessage = "OTP verified successfully!";
          // In a real app, you would redirect or perform other actions here
        }
      }, 1000);
    },
    resendOtp() {
      if (this.resendDisabled) return;

      // In a real app, you would call your backend to resend the OTP
      console.log("Resending OTP...");

      // Reset OTP fields
      this.otp = ["", "", "", ""];
      this.errorMessage = "";
      this.successMessage = "A new OTP has been sent to your email/phone.";

      // Focus the first input
      this.$refs.inputs[0].focus();
      this.activeIndex = 0;

      // Restart countdown
      this.startCountdown();
    },
    startCountdown() {
      this.resendDisabled = true;
      this.countdown = 30;

      clearInterval(this.timer);
      this.timer = setInterval(() => {
        this.countdown--;
        if (this.countdown <= 0) {
          clearInterval(this.timer);
          this.resendDisabled = false;
        }
      }, 1000);
    },
  },
};
</script>
