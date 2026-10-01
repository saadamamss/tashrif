namespace tashrif.Data.Validation;

public static class RequestValidator
{
    /// <summary>
    /// Enforces DataAnnotations on a hand-built DTO (e.g. /api/auth/register, which reads
    /// Request.Form directly so [ApiController] auto-validation never fires). Throws
    /// ArgumentException with the first Arabic message — ExceptionMiddleware maps it to
    /// 400 { message }, which useApi surfaces via data.message.
    /// </summary>
    public static void Validate(object dto)
    {
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, new System.ComponentModel.DataAnnotations.ValidationContext(dto), results, true))
        {
            var message = results.FirstOrDefault()?.ErrorMessage ?? "بيانات غير صحيحة";
            throw new ArgumentException(message);
        }
    }
}
