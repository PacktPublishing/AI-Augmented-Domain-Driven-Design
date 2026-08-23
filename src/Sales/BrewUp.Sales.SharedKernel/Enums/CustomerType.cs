using BrewUp.Shared.Helpers;

namespace BrewUp.Sales.SharedKernel.Enums;

public sealed class CustomerType(int id, string name) : Enumeration(id, name)
{
    public static CustomerType Gold = new (1, nameof(Gold).ToLowerInvariant());
    public static CustomerType Silver = new (2, nameof(Silver).ToLowerInvariant());
    public static CustomerType Bronze = new (3, nameof(Bronze).ToLowerInvariant());

    public static IEnumerable<CustomerType> List() => new[] { Gold, Silver, Bronze };

    public static CustomerType FromName(string name)
    {
        var customerType = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        return customerType ??
               throw new Exception($"Possible values for CustomerType: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static CustomerType From(int id)
    {
        var customerType = List().SingleOrDefault(s => s.Id == id);

        return customerType ??
               throw new Exception($"Possible values for CustomerType: {string.Join(",", List().Select(s => s.Name))}");
    }
}