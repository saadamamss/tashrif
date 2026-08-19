<template>
  <Dialog v-model="model">
    <div class="bg-white rounded-2xl shadow-md overflow-hidden">
      <div class="bg-bg-light p-6">
        <h2 class="text-xl font-bold text-gray-800 mb-3 text-right">
          تعديل الملف الشخصي للجهة
        </h2>
        <p class="text-gray-600 text-sm text-right">
          أكمل بيانات جهتك لتظهر بشكل احترافي أمام الباحثين عن عمل وتزيد من
          فرصك في جذب المرشحين المناسبين.
        </p>
      </div>
      <Form @submit="submit" v-slot="{ errors }">
        <div class="p-6">
          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="companyName"
                id="companyName"
                label="اسم الجهة"
                required
                rules="required"
                placeholder="اسم الجهة"
                v-model="formData.companyName"
                :error="errors.companyName"
              />
            </div>
            <div class="flex-1">
              <label class="block text-sm mb-2">البريد الإلكتروني</label>
              <div
                class="text-sm rounded-2xl w-full h-[38px] px-2 bg-bg-light flex items-center text-gray-400"
              >
                {{ formData.email }}
              </div>
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="phone"
                id="phone"
                label="رقم الجوال"
                required
                rules="required"
                placeholder="05xxxxxxxx"
                v-model="formData.phone"
                :error="errors.phone"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="companyField"
                id="companyField"
                label="مجال العمل"
                required
                rules="required"
                placeholder="مجال العمل"
                v-model="formData.companyField"
                :error="errors.companyField"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="companySize"
                id="companySize"
                label="حجم الشركة"
                placeholder="(مثال: صغيرة، متوسطة، كبيرة)"
                v-model="formData.companySize"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="commercialReg"
                id="commercialReg"
                label="رقم السجل التجاري"
                placeholder="رقم السجل التجاري"
                v-model="formData.commercialReg"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="sector"
                id="sector"
                label="القطاع"
                placeholder="القطاع"
                v-model="formData.sector"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="country"
                id="country"
                label="الدولة"
                placeholder="الدولة"
                v-model="formData.country"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="city"
                id="city"
                label="المدينة"
                placeholder="المدينة"
                v-model="formData.city"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="zone"
                id="zone"
                label="المنطقة"
                placeholder="المنطقة"
                v-model="formData.zone"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="district"
                id="district"
                label="الحي"
                placeholder="الحي"
                v-model="formData.district"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="street"
                id="street"
                label="الشارع"
                placeholder="الشارع"
                v-model="formData.street"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="zipcode"
                id="zipcode"
                label="الرمز البريدي"
                placeholder="الرمز البريدي"
                v-model="formData.zipcode"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="website"
                id="website"
                label="الموقع الإلكتروني"
                placeholder="www.example.com"
                v-model="formData.website"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="facebookAccount"
                id="facebookAccount"
                label="فيسبوك"
                placeholder="facebook.com/..."
                v-model="formData.facebookAccount"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="twitterAccount"
                id="twitterAccount"
                label="تويتر"
                placeholder="twitter.com/..."
                v-model="formData.twitterAccount"
              />
            </div>
          </div>

          <div class="flex gap-6 mb-6">
            <div class="flex-1">
              <TextInput
                name="youtubeAccount"
                id="youtubeAccount"
                label="يوتيوب"
                placeholder="youtube.com/..."
                v-model="formData.youtubeAccount"
              />
            </div>
            <div class="flex-1">
              <TextInput
                name="companyDesc"
                id="companyDesc"
                label="نبذة عن الجهة"
                placeholder="نبذة قصيرة عن الجهة"
                v-model="formData.companyDesc"
              />
            </div>
          </div>
        </div>
        <div
          class="p-6 flex justify-between space-x-3 space-x-reverse bg-bg-light"
        >
          <button type="button" @click="cancel" class="btn-outline text-sm">
            إلغاء
          </button>
          <button type="submit" class="btn-primary text-sm" :disabled="isSubmitting">
            {{ isSubmitting ? "جارٍ الحفظ..." : "حفظ" }}
          </button>
        </div>
      </Form>
    </div>
  </Dialog>
</template>

<script setup>
import { Form } from "vee-validate";
import TextInput from "./elements/TextInput.vue";

const model = defineModel();
const emit = defineEmits(["saved"]);

const formData = ref({ email: "" });
const isSubmitting = ref(false);

watch(model, async (open) => {
  if (!open) return;
  try {
    const { data, error } = await useApi().get("/entities/profile");
    if (!error && data) {
      formData.value = {
        companyName: data.name || "",
        email: data.email || "",
        phone: data.phone || "",
        companyField: data.companyField || "",
        companySize: data.companySize || "",
        commercialReg: data.commercialReg || "",
        sector: data.sector || "",
        country: data.country || "",
        city: data.city || "",
        zone: data.zone || "",
        district: data.district || "",
        street: data.street || "",
        zipcode: data.zipcode || "",
        website: data.website || "",
        facebookAccount: data.facebookUrl || "",
        twitterAccount: data.twitterUrl || "",
        youtubeAccount: data.youtubeUrl || "",
        companyDesc: data.description || "",
      };
    }
  } catch {
    // leave form empty on failure
  }
});

const submit = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true;

  const payload = {
    name: formData.value.companyName,
    phone: formData.value.phone,
    companyField: formData.value.companyField,
    companySize: formData.value.companySize,
    commercialReg: formData.value.commercialReg,
    sector: formData.value.sector,
    country: formData.value.country,
    city: formData.value.city,
    zone: formData.value.zone,
    district: formData.value.district,
    street: formData.value.street,
    zipcode: formData.value.zipcode,
    website: formData.value.website,
    facebookAccount: formData.value.facebookAccount,
    twitterAccount: formData.value.twitterAccount,
    youtubeAccount: formData.value.youtubeAccount,
    companyDesc: formData.value.companyDesc,
  };

  try {
    const { error } = await useApi().put("/entities/profile", payload);

    if (error) {
      useToast().show(error, "error");
      return;
    }

    useToast().show("تم تحديث الملف الشخصي بنجاح", "success");
    model.value = false;
    emit("saved");
  } catch {
    useToast().show("حدث خطأ أثناء تحديث الملف الشخصي", "error");
  } finally {
    isSubmitting.value = false;
  }
};

const cancel = () => {
  model.value = false;
};
</script>