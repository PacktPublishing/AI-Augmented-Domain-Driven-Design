using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Payment.SharedKernel.Messages.Commands;

public sealed class RequestPaymentAuthorization(
    PaymentAuthorizationId aggregateId,
    SalesOrderReference salesOrder,
    Guid correlationId) : Command(aggregateId, correlationId)
{
    public SalesOrderReference SalesOrder { get; } = salesOrder;
}
