using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Muflone.Core;

namespace BrewUp.Warehouse.Domain.Entities;

public class Shipment : AggregateRoot
{
    [Required]
    private SalesOrderId _salesOrderId;
    [Required]
    private CustomerId _customerId;
    [Required]
    private DeliveryDate _deliveryDate;
    
    [Required]
    private IEnumerable<OrderRowDto> _rows;
    [Required]
    private    ShipmentState _shipmentState;
    
    protected Shipment() { }

    internal static Shipment Create(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId,
        DeliveryDate deliveryDate,
        IEnumerable<OrderRowDto> rows, Guid correlationId)
    {
        return new Shipment(shipmentId, salesOrderId, customerId, deliveryDate, rows, correlationId);
    }

    private Shipment(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId, DeliveryDate deliveryDate,
        IEnumerable<OrderRowDto> rows, Guid correlationId)
    {
        RaiseEvent(new ShipmentPendingForPreparation(shipmentId, salesOrderId, customerId, deliveryDate, rows,
            ShipmentState.PendingPreparation, correlationId));
    }
    
    private void Apply(ShipmentPendingForPreparation @event)
    {
        Id = @event.AggregateId;
        _salesOrderId = @event.SalesOrderId;
        _customerId = @event.CustomerId;
        _deliveryDate = @event.DeliveryDate;
        _rows = @event.Rows;
        _shipmentState = ShipmentState.PendingPreparation;
    }
}