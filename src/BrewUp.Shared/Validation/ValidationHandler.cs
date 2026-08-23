using FluentValidation;

namespace BrewUp.Shared.Validation;

public sealed class ValidationHandler
{
    public bool IsValid { get; private set; } = true;
    public Dictionary<string, string[]> Errors { get; private set; } = new();

    public async Task ValidateAsync<T>(IValidator<T> validator, T validateMe) where T : class
    {
        var validationResult = await validator.ValidateAsync(validateMe);
        if (validationResult.IsValid)
        {
            Errors = new Dictionary<string, string[]>();
            return;
        }

        Errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(k => k.Key, v => v.Select(e => e.ErrorMessage).ToArray());

        IsValid = false;
    }
    
    public void ValidateQueryString(IEnumerable<string> queryString)
    {
        var errors = new Dictionary<string, string[]>();
        IsValid = true;

        foreach (var parameter in queryString)
        {
            if (!string.IsNullOrEmpty(parameter)) continue;
            
            errors.Add($"{nameof(parameter)}", [$"{nameof(parameter)} is required."]);
            
            IsValid = false;
        }
        
        if (!IsValid)
            FillErrors(errors);
    }

    private void FillErrors(Dictionary<string, string[]> errors)
    {
        Errors = errors;
        IsValid = false;
    }
}