using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Shared.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Payment.SharedKernel.Messages.Commands;

public sealed class AuthorizePayment(PaymentAuthorizationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    Price amount) : Command(aggregateId, correlationId)
{
    public string SalesOrderId { get; } = salesOrderId;
    public Price Amount { get; } = amount;
}
