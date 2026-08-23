using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands
{
    public sealed class CreateAvailability(AvailabilityId aggregateId,
          WarehouseId warehouseId,
          BeerId beerId,
          Quantity quantity) : Command(aggregateId)
    {
        public WarehouseId WarehouseId { get; private set; } = warehouseId;
        public BeerId BeerId { get; private set; } = beerId;
        public Quantity Quantity { get; private set; } = quantity;
    }
}
