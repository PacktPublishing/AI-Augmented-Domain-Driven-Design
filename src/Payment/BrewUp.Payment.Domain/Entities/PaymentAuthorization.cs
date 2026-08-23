using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Events;
using Muflone.Core;

namespace BrewUp.Payment.Domain.Entities;

public class PaymentAuthorization : AggregateRoot
{
    private SalesOrderReference _salesOrder = null!;
    private bool _isPending;
    private bool _isAuthorized;

    protected PaymentAuthorization()
    {
    }

    internal static PaymentAuthorization Create() => new();

    internal void Request(
        PaymentAuthorizationId aggregateId,
        SalesOrderReference salesOrder,
        Guid correlationId)
    {
        if (_isPending || _isAuthorized)
            return;

        RaiseEvent(new PaymentAuthorizationRequested(aggregateId, salesOrder, correlationId));
    }

    private void Apply(PaymentAuthorizationRequested @event)
    {
        Id = @event.AggregateId;
        _salesOrder = @event.SalesOrder;
        _isPending = true;
    }

    internal void RecordAuthorized(string providerReference, Guid correlationId)
    {
        if (_isAuthorized)
            return;

        RaiseEvent(new PaymentAuthorized(
            new PaymentAuthorizationId(Id.Value),
            providerReference,
            correlationId));
    }

    private void Apply(PaymentAuthorized @event)
    {
        _isPending = false;
        _isAuthorized = true;
    }
}
