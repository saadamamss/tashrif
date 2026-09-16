import { defineRule, configure } from "vee-validate";
import { required, email, min, numeric } from "@vee-validate/rules";

export default defineNuxtPlugin(() => {
  // Define rules
  defineRule("required", required);
  defineRule("email", email);
  defineRule("min", min);
  defineRule("numeric", numeric);
  defineRule("confirmed", (value, [targetValue])=>{
     return value === targetValue || 'كلمة السر غير متطابقة';
  })
  
  // Configure default messages
  configure({
    generateMessage: (ctx) => {
      const messages = {
        required: `${ctx.label} مطلوب`,
        email: "يجب إدخال بريد إلكتروني صحيح",
        min: `يجب أن يحتوي ${ctx.label} على الأقل ${ctx.rule?.params?.[0]} أحرف`,
        numeric: `يجب أن يحتوي ${ctx.label} على أرقام فقط`,
      };
      return messages[ctx.rule.name] || `حقل ${ctx.field} غير صالح`;
    },
  });
});
