using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Payment.ReadModel.Dtos;

public sealed class PaymentAuthorization : DtoBase
{
    public string SalesOrderId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? ProviderReference { get; set; }

    private PaymentAuthorization()
    {
    }

    public static PaymentAuthorization CreatePending(
        PaymentAuthorizationId authorizationId,
        SalesOrderReference salesOrder) =>
        new()
        {
            Id = authorizationId.Value,
            SalesOrderId = salesOrder.Value
        };

    public void MarkAuthorized(string providerReference)
    {
        Status = "authorized";
        ProviderReference = providerReference;
    }
}
