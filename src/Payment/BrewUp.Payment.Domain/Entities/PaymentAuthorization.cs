using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Enums;
using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using Muflone.Core;

namespace BrewUp.Payment.Domain.Entities;

public class PaymentAuthorization : AggregateRoot
{
    private PaymentAuthorizationStatus _status = PaymentAuthorizationStatus.Pending;
    private string _salesOrderId = string.Empty;
    private Price _amount = new(0m, string.Empty);

    protected PaymentAuthorization() { }

    internal static PaymentAuthorization Create(PaymentAuthorizationId aggregateId,
        string salesOrderId, Price amount, Guid correlationId)
    {
        return new PaymentAuthorization(aggregateId, salesOrderId, amount, correlationId);
    }

    private PaymentAuthorization(PaymentAuthorizationId aggregateId, string salesOrderId, Price amount, Guid correlationId)
    {
        // Payment owns the authorization decision (BC-003)
        // For demo: approve when amount > 0, decline when amount <= 0
        if (amount.Value > 0)
            RaiseEvent(new PaymentAuthorized(aggregateId, correlationId, salesOrderId));
        else
            RaiseEvent(new PaymentDeclined(aggregateId, correlationId, salesOrderId,
                "Amount must be greater than zero"));
    }

    internal void Authorize(string salesOrderId, Price amount, Guid correlationId)
    {
        // Idempotency: already authorized/declined → no-op
        if (!Equals(_status, PaymentAuthorizationStatus.Pending))
            return;

        if (amount.Value > 0)
            RaiseEvent(new PaymentAuthorized(new PaymentAuthorizationId(Id.Value), correlationId, salesOrderId));
        else
            RaiseEvent(new PaymentDeclined(new PaymentAuthorizationId(Id.Value), correlationId, salesOrderId,
                "Amount must be greater than zero"));
    }

    private void Apply(PaymentAuthorized @event)
    {
        Id = @event.AggregateId;
        _salesOrderId = @event.SalesOrderId;
        _status = PaymentAuthorizationStatus.Authorized;
    }

    private void Apply(PaymentDeclined @event)
    {
        Id = @event.AggregateId;
        _salesOrderId = @event.SalesOrderId;
        _status = PaymentAuthorizationStatus.Declined;
    }
}
