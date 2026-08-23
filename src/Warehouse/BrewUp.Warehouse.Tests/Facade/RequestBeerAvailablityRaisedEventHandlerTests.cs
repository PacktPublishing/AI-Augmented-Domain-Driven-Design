using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Warehouse.Facade.Acl;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Tests.Facade;

public sealed class RequestBeerAvailablityRaisedEventHandlerTests
{
    [Fact]
    public async Task Reserves_Stock_And_Publishes_Reservation_Reference_When_All_Rows_Are_Available()
    {
        var availabilityId = new AvailabilityId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var eventBus = new RecordingEventBus();
        var serviceBus = new RecordingServiceBus();
        var handler = new RequestBeerAvailablityRaisedEventHandler(
            eventBus,
            serviceBus,
            new FixedAvailabilityService(new AvailabilityJson
            {
                Id = availabilityId.Value,
                WarehouseId = warehouseId.Value,
                BeerId = beerId.Value,
                Quantity = 10,
                UnitOfMeasure = "Bottle"
            }),
            new NullLoggerFactory());

        await handler.HandleAsync(new RequestBeerAvailablityRaised(
            new IntegrationId(Guid.CreateVersion7().ToString()),
            Guid.CreateVersion7(),
            warehouseId.Value,
            [new ItemRequested(beerId, new Quantity(4, "Bottle"), new Quantity(0, "Bottle"))]));

        var command = Assert.IsType<ReserveItemStock>(Assert.Single(serviceBus.SentCommands));
        var integrationEvent = Assert.IsType<RequestBeersAvailabilityChecked>(Assert.Single(eventBus.PublishedEvents));
        Assert.NotNull(integrationEvent.StockReservationId);
        Assert.Equal(integrationEvent.StockReservationId.Value, command.StockReservationId.Value);
        Assert.Equal(availabilityId.Value, command.AggregateId.Value);
        Assert.Equal(4, command.Quantity.Value);
    }

    private sealed class RecordingEventBus : IEventBus
    {
        public List<IEvent> PublishedEvents { get; } = [];

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class RecordingServiceBus : IServiceBus
    {
        public List<ICommand> SentCommands { get; } = [];

        public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : class, ICommand
        {
            SentCommands.Add(command);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class FixedAvailabilityService(AvailabilityJson availability) : IAvailabilityService
    {
        public Task<Result<AvailabilityJson>> GetAvailabilityByWarehouseIdAndBeerIdAsync(
            WarehouseId warehouseId,
            BeerId beerId,
            CancellationToken cancellationToken) =>
            Task.FromResult(Result<AvailabilityJson>.Success(availability));

        public Task<Result<bool>> AddAvailabilityAsync(
            AvailabilityId availabilityId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<string>> AddItemStockAsync(
            AvailabilityId availabilityId,
            Quantity quantity,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<AvailabilityJson>> GetAvailabilityByBeerIdAsync(
            BeerId beerId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<AvailabilityJson>> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Result<ReorderThreshold>> GetReorderThresholdByBeerIdAsync(
            BeerId beerId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
