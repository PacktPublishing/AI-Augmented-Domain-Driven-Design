using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;

namespace BrewUp.MasterData.Entities.Dtos;

public class Warehouse : DtoBase
{
    public string Name { get; set; } = string.Empty;
    
    protected Warehouse() 
    { }
    
    public static Warehouse Create(WarehouseId warehouseId, WarehouseName name) =>
        new (warehouseId.Value, name.Value);
    
    private Warehouse(string warehouseId, string name)
    {
        Id = warehouseId;
        Name = name;
    }
    
    public WarehouseJson ToJson() =>
        new ()
        {
            Id = Id,
            Name = Name
        };
}