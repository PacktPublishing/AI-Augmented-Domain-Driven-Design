using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.MasterData;

namespace BrewUp.MasterData.Domain.Helpers;

internal static class WarehouseHelper
{
    internal static WarehouseCreated ToWarehouseCreated(this Warehouse warehouse) =>
        new WarehouseCreated(new WarehouseId(warehouse.Id), new WarehouseName(warehouse.Name));
}