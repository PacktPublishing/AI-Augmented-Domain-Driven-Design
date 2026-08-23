using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands
{
    public sealed class RequestBeersAvailability(WarehouseId warehouseId,
                                    Guid correlationId,
                                    IEnumerable<ItemRequested> rows) : Command(warehouseId, correlationId)
    {
        public IEnumerable<ItemRequested> Rows { get; } = rows;
    }
}
