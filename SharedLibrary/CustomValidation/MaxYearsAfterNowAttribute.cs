using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.CustomValidation
{
    public class MaxYearsAfterNowAttribute : ValidationAttribute
    {
        private readonly int _years;

        public MaxYearsAfterNowAttribute(int years)
        {
            _years = years;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage = "Date is required.");
            }

            DateTime maxDate = DateTime.Now.AddYears(_years);

            if ((DateTime)value > maxDate)
            {
                return new ValidationResult(ErrorMessage = $"Date cannot be after {maxDate.Date}.");
            }

            return ValidationResult.Success;
        }
    }
}
