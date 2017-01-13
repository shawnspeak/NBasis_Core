using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NBasis.Validation
{
    public class RequiredGuid : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' is required";

        public RequiredGuid() : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);

            // value must be required
            if (string.IsNullOrWhiteSpace(input))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // can't parse guid
            if (!Guid.TryParse(input, out Guid guid))
            {
                // not a validstring representation of a guid
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (guid == Guid.Empty)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return null;
        }
    }
}
