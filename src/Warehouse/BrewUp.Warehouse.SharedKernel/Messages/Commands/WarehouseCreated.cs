using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands
{
    public class WarehouseCreated(WarehouseId warehouseId,
                                  WarehouseJson warehouse,
                                  Guid correlationId) : Command(warehouseId, correlationId)
    {
        public WarehouseJson Warehouse { get; } = warehouse;
    }
}
