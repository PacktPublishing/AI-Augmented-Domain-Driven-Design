using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Facade.EventHandlers;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.Tests.Facade;

public sealed class PublishReservationIntegrationOutcomes
{
    [Fact]
    public async Task SuccessPublishesOnlyTheReservedOutcomeWithOrderedRows()
    {
        var reservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var beerA = new BeerId(Guid.CreateVersion7().ToString());
        var beerB = new BeerId(Guid.CreateVersion7().ToString());
        var correlationId = Guid.CreateVersion7();
        var bus = new CapturingEventBus();
        var publisher = new StockReservedIntegrationEventPublisher(
            bus,
            NullLoggerFactory.Instance);

        await publisher.HandleAsync(
            new StockReserved(
                reservationId,
                warehouseId,
                salesOrderId,
                [
                    Row(beerA, 2, 5),
                    Row(beerB, 1, 3)
                ],
                correlationId),
            CancellationToken.None);

        var integrationEvent =
            Assert.IsType<StockReservedIntegrationEvent>(Assert.Single(bus.Published));
        Assert.Equal(reservationId, integrationEvent.AggregateId);
        Assert.Equal(warehouseId, integrationEvent.WarehouseId);
        Assert.Equal(salesOrderId, integrationEvent.SalesOrderId);
        Assert.Equal([beerA.Value, beerB.Value],
            integrationEvent.Rows.Select(row => row.BeerId.Value));
        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(integrationEvent));
    }

    [Fact]
    public async Task FailurePublishesOnlyTheFailedOutcomeWithNoRows()
    {
        var reservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var correlationId = Guid.CreateVersion7();
        var bus = new CapturingEventBus();
        var publisher = new StockReservationFailedIntegrationEventPublisher(
            bus,
            NullLoggerFactory.Instance);

        await publisher.HandleAsync(
            new StockReservationFailed(
                reservationId,
                warehouseId,
                salesOrderId,
                "All requested stock is not available.",
                correlationId),
            CancellationToken.None);

        var integrationEvent =
            Assert.IsType<StockReservationFailedIntegrationEvent>(
                Assert.Single(bus.Published));
        Assert.Equal(reservationId, integrationEvent.AggregateId);
        Assert.Equal(warehouseId, integrationEvent.WarehouseId);
        Assert.Equal(salesOrderId, integrationEvent.SalesOrderId);
        Assert.Equal(
            "All requested stock is not available.",
            integrationEvent.Reason);
        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(integrationEvent));
    }

    private static ItemRequested Row(BeerId beerId, decimal ordered, decimal available) =>
        new(
            beerId,
            new Quantity(ordered, "Bottle"),
            new Quantity(available, "Bottle"));

    private sealed class CapturingEventBus : IEventBus
    {
        public List<IEvent> Published { get; } = [];

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
            where T : class, IEvent
        {
            Published.Add(@event);
            return Task.CompletedTask;
        }
    }
}
