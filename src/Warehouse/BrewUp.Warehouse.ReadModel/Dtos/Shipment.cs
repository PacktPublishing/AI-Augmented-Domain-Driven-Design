using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;

namespace BrewUp.Warehouse.ReadModel.Dtos;

public class Shipment : DtoBase
{
    public string SalesOrderId { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public DateTime DeliveryDate { get; private set; }
    public IEnumerable<OrderRowDto> Rows { get; private set; } = [];
    public string ShipmentState { get; private set; } = string.Empty;
    
    protected Shipment() { }
    
    public static Shipment Create(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId, DeliveryDate deliveryDate,
        ShipmentState shipmentState, IEnumerable<OrderRowDto> rows)
    {
        return new Shipment(shipmentId, salesOrderId, customerId, deliveryDate, shipmentState, rows);
    }

    private Shipment(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId, DeliveryDate deliveryDate,
        ShipmentState shipmentState, IEnumerable<OrderRowDto> rows)
    {
        Id = shipmentId.Value;
        SalesOrderId = salesOrderId.Value;
        CustomerId = customerId.Value;
        DeliveryDate = deliveryDate.Value;
        Rows = rows;

        ShipmentState = shipmentState.Name;
    }
    
    public ShipmentJson ToJson()
    {
        return new ShipmentJson
        {
            Id = Id,
            SalesOrderId = SalesOrderId,
            CustomerId = CustomerId,
            DeliveryDate = DeliveryDate,
            Rows = Rows,
            ShipmentState = ShipmentState
        };
    }
}