using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Warehouse.ReadModel.Dtos;

public class Availability : DtoBase
{
    public string WarehouseId { get; private set; }
    public string BeerId { get; private set; }
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; }

    protected Availability() { }
    
    public static Availability Create(AvailabilityId aggregateId, 
        WarehouseId warehouseId,
        BeerId beerId, 
        Quantity quantity) => new (aggregateId.Value, 
            warehouseId.Value, 
            beerId.Value, 
            quantity.Value, 
            quantity.UnitOfMeasure);

    internal AvailabilityJson ToJson()
    {
        return new AvailabilityJson
        {
            Id = Id,
            WarehouseId = WarehouseId,
            BeerId = BeerId,
            Quantity = Quantity,
            UnitOfMeasure = UnitOfMeasure
        };
    }

    private Availability(string aggregateId, 
        string warehouseId,
        string beerId,
        decimal quantity,
        string unitOfMeasure)
    {
        Id = aggregateId;
        WarehouseId = warehouseId;
        BeerId = beerId;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
    }

    public void UpdateQuantity(Quantity quantity)
    {
        Quantity = quantity.Value;
        UnitOfMeasure = quantity.UnitOfMeasure;
    }
}