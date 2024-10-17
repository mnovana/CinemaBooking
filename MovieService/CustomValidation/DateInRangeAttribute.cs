using System.ComponentModel.DataAnnotations;

namespace MovieService.CustomValidation
{
    public class DateInRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage = "Date is required.");
            }

            DateTime minDate = new DateTime(1900, 1, 1);
            DateTime maxDate = DateTime.Now.AddYears(5);

            if ((DateTime)value < minDate || (DateTime)value > maxDate)
            {
                return new ValidationResult(ErrorMessage = "Date is not in range.");
            }

            return ValidationResult.Success;
        }
    }
}
