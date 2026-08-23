using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

/// <summary>
/// Raised when Warehouse BC rejects the stock reservation request.
/// The saga records this outcome without compensation (out of scope per OQ-7).
/// </summary>
public sealed class SagaStockReservationRejected(IntegrationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string reason) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
