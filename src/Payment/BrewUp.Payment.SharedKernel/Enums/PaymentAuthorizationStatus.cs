using BrewUp.Shared.Helpers;

namespace BrewUp.Payment.SharedKernel.Enums;

public sealed class PaymentAuthorizationStatus(int id, string name) : Enumeration(id, name)
{
    public static PaymentAuthorizationStatus Authorized = new(1, nameof(Authorized).ToLowerInvariant());
    public static PaymentAuthorizationStatus Declined = new(2, nameof(Declined).ToLowerInvariant());
    public static PaymentAuthorizationStatus Pending = new(3, nameof(Pending).ToLowerInvariant());

    public static IEnumerable<PaymentAuthorizationStatus> List() => [Authorized, Declined, Pending];

    public static PaymentAuthorizationStatus FromName(string name)
    {
        var status = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        return status ?? throw new Exception($"Possible values for PaymentAuthorizationStatus: {string.Join(",", List().Select(s => s.Name))}");
    }

    public static PaymentAuthorizationStatus From(int id)
    {
        var status = List().SingleOrDefault(s => s.Id == id);
        return status ?? throw new Exception($"Possible values for PaymentAuthorizationStatus: {string.Join(",", List().Select(s => s.Name))}");
    }
}
