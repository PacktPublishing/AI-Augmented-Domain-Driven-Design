using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.SharedKernel.Messages.Events
{
    public sealed class SagaOrderRequestRejected(WarehouseId aggregateId, Guid correlationId,
        string message) : DomainEvent(aggregateId, correlationId)
    {
        public string Message { get; private set; } = message;
    }
}
