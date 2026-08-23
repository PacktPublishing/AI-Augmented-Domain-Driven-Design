using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.CustomTypes;

namespace BrewUp.Shared.Validators;

public class PriceGreaterThanZeroAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not Price price) return ValidationResult.Success;
        
        return price.Value <= 0 
            ? new ValidationResult(ErrorMessage ?? "Price must be greater than 0") 
            : ValidationResult.Success;
    }
}