using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events
{
    public sealed class AvailabilityCreated(AvailabilityId aggregateId,
        WarehouseId warehouseId,
        BeerId beerId,
        Quantity quantity) : DomainEvent(aggregateId)
    {
        public WarehouseId WarehouseId { get; private set; } = warehouseId;
        public BeerId BeerId { get; private set; } = beerId;
        public Quantity Quantity { get; private set; } = quantity;
    }
}
