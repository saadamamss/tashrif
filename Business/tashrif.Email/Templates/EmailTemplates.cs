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


    /// <summary>
    /// The verification mail (D4/A1). <paramref name="link"/> points at the FRONTEND
    /// page (`{Frontend:BaseUrl}/verify-email?token=…`), never at the API — an email
    /// scanner that prefetches the URL must not burn the single-use token.
    /// </summary>
    public static string VerifyEmail(string name, string link) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تأكيد بريدك الإلكتروني</h1>
            <p>عزيزي {name}،</p>
            <p>تم إنشاء حسابك بنجاح. لتفعيل الحساب والبدء في استخدام المنصة، يرجى الضغط على الرابط التالي:</p>
            <p><a href=""{link}"" style=""display:inline-block;padding:12px 24px;background:#ECB42B;color:#000;border-radius:8px;text-decoration:none;"">تفعيل الحساب</a></p>
            <p>أو انسخ الرابط التالي في متصفحك:</p>
            <p>{link}</p>
            <p>الرابط صالح لمدة 24 ساعة، ويمكن استخدامه مرة واحدة فقط.</p>
            <p>إذا لم تطلب إنشاء حساب، فيمكنك تجاهل هذه الرسالة.</p>
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

    public static string Shortlisted(string individualName, string jobTitle, string entityName) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تم ترشحك</h1>
            <p>عزيزي {individualName}،</p>
            <p>تم ترشحك لوظيفة {jobTitle} في {entityName}.</p>
            <p>في انتظار الخطوة القادمة.</p>
        </div>";

    public static string ApplicationRefused(string individualName, string jobTitle, string entityName) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تم رفض طلبك</h1>
            <p>عزيزي {individualName}،</p>
            <p>تم رفض طلبك لوظيفة {jobTitle} في {entityName}.</p>
            <p>نتمنى لك التوفيق في مسيرتك المهنية.</p>
        </div>";

    public static string ApplicationWithdrawn(string entityName, string applicantName, string jobTitle) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تم سحب الطلب</h1>
            <p>عزيزي {entityName}،</p>
            <p>قام {applicantName} بسحب طلبه لوظيفة {jobTitle}.</p>
        </div>";

    public static string ApplicationRestored(string individualName, string jobTitle, string entityName) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تمت إعادة تقييمك</h1>
            <p>عزيزي {individualName}،</p>
            <p>تمت إعادة تقييمك لوظيفة {jobTitle} في {entityName}.</p>
            <p>سيتم مراجعة طلبك مرة أخرى.</p>
        </div>";

    public static string ContractExpired(string recipientName, string jobTitle, string otherParty) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>انتهت مهلة العقد</h1>
            <p>عزيزي {recipientName}،</p>
            <p>انتهت مهلة توقيع العقد لوظيفة {jobTitle} ({otherParty}) دون توقيع.</p>
            <p>يمكن للجهة تحديث العقد وإعادة إرساله بمهلة جديدة.</p>
        </div>";

    public static string ContractUpdated(string individualName, string jobTitle) => $@"
        <div dir=""rtl"" style=""font-family: sans-serif;"">
            <h1>تم تحديث العقد</h1>
            <p>عزيزي {individualName}،</p>
            <p>قامت الجهة بتحديث العقد لوظيفة {jobTitle}. يرجى الاطلاع على الشروط الجديدة وتوقيعه.</p>
        </div>";
}