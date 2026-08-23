using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Shared.Validators;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return Results.BadRequest("Request body is required");

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(argument, null, null);

        if (!TryValidateObjectRecursive(argument, validationResults, validationContext))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage);
            return Results.BadRequest(new { Errors = errors });
        }

        return await next(context);
    }

    private static bool TryValidateObjectRecursive(object obj, List<ValidationResult> results, ValidationContext validationContext)
    {
        var isValid = Validator.TryValidateObject(obj, validationContext, results, true);

        var properties = obj.GetType().GetProperties()
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            if (value == null) continue;

            if (value is IEnumerable<object> enumerable && !(value is string))
            {
                foreach (var item in enumerable)
                {
                    var context = new ValidationContext(item, null, null);
                    if (!TryValidateObjectRecursive(item, results, context))
                        isValid = false;
                }
            }
            else if (property.PropertyType.Assembly == obj.GetType().Assembly)
            {
                var context = new ValidationContext(value, null, null);
                if (!TryValidateObjectRecursive(value, results, context))
                    isValid = false;
            }
        }

        return isValid;
    }
}