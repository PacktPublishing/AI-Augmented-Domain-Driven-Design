using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events
{
    public sealed class ItemStockAdded(AvailabilityId aggregateId, Quantity quantity) : DomainEvent(aggregateId)
    {
        public Quantity Quantity { get; private set; } = quantity;
    }
}
