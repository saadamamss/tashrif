<template>
  <Dialog v-model="model">
    <div class="content bg-white rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10">
      <div class="flex border-b pb-6 items-start justify-between">
        <div>
          <h3 class="text-lg md:text-lg lg:text-2xl font-semibold mb-3">
            اختر نوع الحساب
          </h3>
          <p class="text-base text-[#75797C]">
            يرجى تحديد نوع الحساب الذي يناسبك للاستفادة من خدمات المنصة:
          </p>
        </div>
        <button @click="closeModal">x</button>
      </div>
      <div class="modal-body pt-6">
        <div class="flex flex-col sm:flex-row gap-8">
          <div class="flex flex-col gap-4 bg-[#F9F9F9] p-4 lg:p-6 rounded-lg">
            <span class="block">
              <PersonIcon width="40" height="41" color="#ECB42B" />
            </span>
            <h2 class="font-bold text-lg">إنشاء حساب كفرد</h2>
            <p class="text-sm text-[#75797C]">
              إذا كنت تبحث عن فرص عمل موسمية في موسم الحج والعمرة، يمكنك التقديم
              على الوظائف المناسبة لك وتتبع حالتها بسهولة
            </p>
            <div class="block mt-4">
              <nuxt-link
                to="/register/individual"
                @click.once="closeModal"
                class="btn-primary font-medium text-sm"
              >
                متابعة كفرد
              </nuxt-link>
            </div>
          </div>
          <div class="flex flex-col bg-[#F9F9F9] gap-4 p-4 lg:p-6 rounded-lg">
            <span class="block">
              <CityIcon width="40" height="41" color="#ECB42B" />
            </span>
            <h2 class="font-bold text-lg">إنشاء حساب كجهة مقدمة للوظائف</h2>
            <p class="text-sm text-[#75797C]">
              إذا كنت تبحث عن فرص عمل موسمية في موسم الحج والعمرة، يمكنك التقديم
              على الوظائف المناسبة لك وتتبع حالتها بسهولة
            </p>
            <div class="block mt-4">
              <nuxt-link
                to="/register/entity"
                @click.once="closeModal"
                class="btn-primary font-medium text-sm"
              >
                متابعة كجهة
              </nuxt-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Dialog>
</template>
<script setup>
const model = defineModel();
const closeModal = () => {
  model.value = false;
};
</script>
