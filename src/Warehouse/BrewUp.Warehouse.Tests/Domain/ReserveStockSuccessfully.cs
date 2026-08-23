using DomainAvailability = BrewUp.Warehouse.Domain.Entities.Availability;
using DomainQuantity = BrewUp.Warehouse.Domain.Entities.Quantity;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Muflone;

namespace BrewUp.Warehouse.Tests.Domain;

/// <summary>
/// US2: ReserveStock succeeds when stock is available.
/// Uses IAggregate.GetUncommittedEvents() to verify the domain event raised.
/// </summary>
public class ReserveStockSuccessfully
{
    [Fact]
    public void When_StockAvailable_ReserveStockRaisesStockReserved()
    {
        var sharedId = Guid.CreateVersion7().ToString();
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var salesOrderId = Guid.CreateVersion7().ToString();
        var correlationId = Guid.CreateVersion7();

        var availId = new AvailabilityId(sharedId);
        var warehouseId = new WarehouseId(sharedId);

        var availability = DomainAvailability.Create(availId, warehouseId, beerId, new DomainQuantity(10, "Bottle"));
        ((IAggregate)availability).ClearUncommittedEvents();

        var rows = new[] {
            new ItemRequested(beerId, new Quantity(5, "Bottle"), new Quantity(10, "Bottle"))
        };

        availability.ReserveStock(rows, salesOrderId, correlationId);

        var uncommitted = ((IAggregate)availability).GetUncommittedEvents().Cast<object>().ToList();
        var reserved = uncommitted.OfType<StockReserved>().ToList();

        Assert.Single(reserved);
        Assert.Equal(salesOrderId, reserved[0].SalesOrderId);
        Assert.NotEmpty(reserved[0].StockReservationId.Value);
    }

    [Fact]
    public void When_NoStock_ReserveStockRaisesRejected()
    {
        var sharedId = Guid.CreateVersion7().ToString();
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var salesOrderId = Guid.CreateVersion7().ToString();
        var correlationId = Guid.CreateVersion7();

        var availId = new AvailabilityId(sharedId);
        var warehouseId = new WarehouseId(sharedId);

        var availability = DomainAvailability.Create(availId, warehouseId, beerId, new DomainQuantity(0, "Bottle"));
        ((IAggregate)availability).ClearUncommittedEvents();

        availability.ReserveStock(
            [new ItemRequested(beerId, new Quantity(5, "Bottle"), new Quantity(0, "Bottle"))],
            salesOrderId, correlationId);

        var uncommitted = ((IAggregate)availability).GetUncommittedEvents().Cast<object>().ToList();
        Assert.Contains(uncommitted, e => e is StockReservationRejected);
        Assert.DoesNotContain(uncommitted, e => e is StockReserved);
    }
}



