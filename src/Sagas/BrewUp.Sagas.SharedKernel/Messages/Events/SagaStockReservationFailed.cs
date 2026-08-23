using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events;

public sealed class SagaStockReservationFailed(IntegrationId aggregateId, Guid correlationId,
    string reason) : DomainEvent(aggregateId, correlationId)
{
    public string Reason { get; private set; } = reason;
}
