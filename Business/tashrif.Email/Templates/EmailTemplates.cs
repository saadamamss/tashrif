namespace tashrif.Email.Templates;

public static class EmailTemplates
{
    public static string Welcome(string name) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>مرحباً بك في تشريف</h1>
            <p>عزيزي {name}،</p>
            <p>تم تسجيل حسابك بنجاح. يمكنك الآن تسجيل الدخول والبدء في استخدام المنصة.</p>
            <p>مع تحياتنا،<br>فريق تشريف</p>
        </div>";


    public static string NewApplicant (string entityName, string applicantName, string jobTitle)=> $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>مستقدم جديد</h1>
            <p>عزيزي {entityName}،</p>
            <p>قام {applicantName} بالتقديم على وظيفة {jobTitle}.</p>
        </div>";

    public static string InterviewScheduled(string individualName, string jobTitle, string date, string time, string location) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>موعد مقابلة</h1>
            <p>عزيزي {individualName}،</p>
            <p>تم جدولة مقابلة لك لوظيفة {jobTitle}.</p>
            <p>التاريخ: {date}</p>
            <p>الوقت: {time}</p>
            <p>المكان: {location}</p>
        </div>";

    public static string ContractSent(string individualName, string jobTitle) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>عقد جديد</h1>
            <p>عزيزي {individualName}،</p>
            <p>تم إرسال عقد لوظيفة {jobTitle} في انتظار توقيعك.</p>
        </div>";
    
    public static string ContractSigned(string entityName, string applicantName, string jobTitle) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تم توقيع العقد</h1>
            <p>عزيزي {entityName}،</p>
            <p>قام {applicantName} بتوقيع العقد لوظيفة {jobTitle}.</p>
        </div>";
}