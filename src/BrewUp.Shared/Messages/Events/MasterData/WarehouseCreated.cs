using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.MasterData;

public sealed class WarehouseCreated(WarehouseId aggregateId, 
    WarehouseName warehouseName) : IntegrationEvent(aggregateId)
{
    public WarehouseName WarehouseName { get; private set; } = warehouseName;
}