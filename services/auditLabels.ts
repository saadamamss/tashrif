// Single source of truth for audit-log display labels + badge colours.
// Keys MUST equal the backend `AuditActions` constants (tashrif.Data/Constants/AuditActions.cs) —
// that equality is the contract. Colours use tailwind.config.js tokens, like analyticsColors.ts.
export const auditLabels: Record<string, string> = {
  "admin.user_deactivated": "تعطيل مستخدم",
  "admin.user_activated": "تفعيل مستخدم",
  "admin.job_deactivated": "إغلاق وظيفة",
  "admin.profile_updated": "تحديث بيانات المشرف",
  "auth.login": "تسجيل دخول",
  "auth.password_changed": "تغيير كلمة المرور",
  "profile.updated": "تحديث الملف الشخصي",
  "contract.sent": "إرسال عقد",
  "contract.signed": "توقيع عقد",
  "interview.scheduled": "جدولة مقابلة",
};

export const auditBadgeClasses: Record<string, string> = {
  "admin.user_deactivated": "bg-danger/10 text-danger",
  "admin.user_activated": "bg-success/10 text-success",
  "admin.job_deactivated": "bg-muted/10 text-muted",
  "admin.profile_updated": "bg-primary/10 text-primary",
  "auth.login": "bg-badge-green/10 text-badge-green",
  "auth.password_changed": "bg-primary/10 text-primary",
  "profile.updated": "bg-primary/10 text-primary",
  "contract.sent": "bg-badge-green/10 text-badge-green",
  "contract.signed": "bg-success/10 text-success",
  "interview.scheduled": "bg-primary/10 text-primary",
};
