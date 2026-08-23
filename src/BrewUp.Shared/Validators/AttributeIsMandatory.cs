using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.Validators;

public class AttributeIsMandatoryAttribute(string propertyName = "Attribute") : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult(ErrorMessage ?? $"{propertyName} is mandatory");

        var property = value.GetType().GetProperty(propertyName);
        if (property == null)
            return new ValidationResult(ErrorMessage ?? $"{propertyName} is mandatory");

        var attributeValue = property.GetValue(value);
        
        if (attributeValue == null || string.IsNullOrWhiteSpace(attributeValue.ToString()))
        {
            return new ValidationResult(ErrorMessage ?? $"{propertyName} is mandatory");
        }

        return ValidationResult.Success;
    }
}