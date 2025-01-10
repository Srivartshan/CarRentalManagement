using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalManagement.Attributes
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (comparisonProperty == null)
            {
                return new ValidationResult($"Property {_comparisonProperty} not found.");
            }

            var comparisonValue = (DateTime)comparisonProperty.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
