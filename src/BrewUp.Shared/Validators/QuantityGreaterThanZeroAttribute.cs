using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.CustomTypes;

namespace BrewUp.Shared.Validators;

public class QuantityGreaterThanZeroAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not Quantity quantity) return ValidationResult.Success;
        
        return quantity.Value <= 0 
            ? new ValidationResult(ErrorMessage ?? "Quantity must be greater than 0") 
            : ValidationResult.Success;
    }
}