using DomainAvailability = BrewUp.Warehouse.Domain.Entities.Availability;
using DomainQuantity = BrewUp.Warehouse.Domain.Entities.Quantity;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Muflone;

namespace BrewUp.Warehouse.Tests.Domain;

/// <summary>
/// US2: ReserveStock with partial quantity raises StockReserved capped to available stock.
/// </summary>
public class ReserveStockPartially
{
    [Fact]
    public void When_RequestedMoreThanAvailable_ReservesAvailableQuantityOnly()
    {
        var sharedId = Guid.CreateVersion7().ToString();
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var salesOrderId = Guid.CreateVersion7().ToString();
        var correlationId = Guid.CreateVersion7();

        var availId = new AvailabilityId(sharedId);
        var warehouseId = new WarehouseId(sharedId);

        // Only 3 in stock
        var availability = DomainAvailability.Create(availId, warehouseId, beerId, new DomainQuantity(3, "Bottle"));
        ((IAggregate)availability).ClearUncommittedEvents();

        // Request 10 — should get partial reservation of 3
        availability.ReserveStock(
            [new ItemRequested(beerId, new Quantity(10, "Bottle"), new Quantity(3, "Bottle"))],
            salesOrderId, correlationId);

        var uncommitted = ((IAggregate)availability).GetUncommittedEvents().Cast<object>().ToList();
        var reserved = uncommitted.OfType<StockReserved>().ToList();

        Assert.Single(reserved);
        Assert.Equal(salesOrderId, reserved[0].SalesOrderId);
        // Partial: reserved quantity is capped to 3 (available), not 10 (requested)
        var reservedRow = reserved[0].ReservedRows.FirstOrDefault();
        Assert.NotNull(reservedRow);
        Assert.Equal(3m, reservedRow.QuantityOrdered.Value);
    }
}
