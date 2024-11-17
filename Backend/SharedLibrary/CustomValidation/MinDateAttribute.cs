using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.CustomValidation
{
    public class MinDateAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate;

        public MinDateAttribute(int year, int month, int day)
        {
            _minDate = new DateTime(year, month, day);
        }
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage = "Date is required.");
            }

            if ((DateTime)value < _minDate)
            {
                return new ValidationResult(ErrorMessage = $"Date cannot be before {_minDate.Date}.");
            }

            return ValidationResult.Success;
        }
    }
}
