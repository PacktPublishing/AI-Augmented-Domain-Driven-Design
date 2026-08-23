using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events
{
    public sealed class SagaOrderRequestAccepted(WarehouseId aggregateId, Guid correlationId) : DomainEvent(aggregateId, correlationId) { }
}
