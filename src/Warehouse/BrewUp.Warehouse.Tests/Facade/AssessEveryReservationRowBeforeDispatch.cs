using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Warehouse.Facade.Acl;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Tests.Facade;

public sealed class AssessEveryReservationRowBeforeDispatch
{
    [Fact]
    public async Task MissingRowDoesNotStopOrderedAssessmentOrDispatch()
    {
        var reservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var missingBeer = new BeerId(Guid.CreateVersion7().ToString());
        var availableBeer = new BeerId(Guid.CreateVersion7().ToString());
        var thirdBeer = new BeerId(Guid.CreateVersion7().ToString());
        var availabilityService = new RecordingAvailabilityService(
            new Dictionary<string, AvailabilityJson>
            {
                [availableBeer.Value] = Availability(
                    warehouseId,
                    availableBeer,
                    7,
                    "Bottle"),
                [thirdBeer.Value] = Availability(
                    warehouseId,
                    thirdBeer,
                    2,
                    "Keg")
            });
        var serviceBus = new RecordingServiceBus(availabilityService);
        var handler = new StockReservationRequestedIntegrationEventHandler(
            serviceBus,
            availabilityService,
            NullLoggerFactory.Instance);
        var correlationId = Guid.CreateVersion7();

        await handler.HandleAsync(
            new StockReservationRequestedIntegrationEvent(
                reservationId,
                warehouseId,
                salesOrderId,
                [
                    Requested(missingBeer, 1, "Bottle"),
                    Requested(availableBeer, 4, "Bottle"),
                    Requested(thirdBeer, 1, "Keg")
                ],
                correlationId),
            CancellationToken.None);

        Assert.Equal(
            [missingBeer.Value, availableBeer.Value, thirdBeer.Value],
            availabilityService.RequestedBeerIds);
        var command = Assert.Single(serviceBus.Commands);
        Assert.Equal(3, serviceBus.AvailabilityCallsAtDispatch);
        Assert.Equal(reservationId, command.AggregateId);
        Assert.Equal(warehouseId, command.WarehouseId);
        Assert.Equal(salesOrderId, command.SalesOrderId);
        Assert.Equal(correlationId, command.MessageId);
        Assert.Equal(
            [missingBeer.Value, availableBeer.Value, thirdBeer.Value],
            command.Rows.Select(row => row.BeerId.Value));
        Assert.Equal(
            [(0m, "Bottle"), (7m, "Bottle"), (2m, "Keg")],
            command.Rows.Select(row =>
                (row.QuantityAvailable.Value, row.QuantityAvailable.UnitOfMeasure)));
    }

    private static ItemRequested Requested(
        BeerId beerId,
        decimal quantity,
        string unitOfMeasure) =>
        new(
            beerId,
            new Quantity(quantity, unitOfMeasure),
            new Quantity(0, unitOfMeasure));

    private static AvailabilityJson Availability(
        WarehouseId warehouseId,
        BeerId beerId,
        decimal quantity,
        string unitOfMeasure) =>
        new()
        {
            Id = Guid.CreateVersion7().ToString(),
            WarehouseId = warehouseId.Value,
            BeerId = beerId.Value,
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure
        };

    private sealed class RecordingAvailabilityService(
        IReadOnlyDictionary<string, AvailabilityJson> availability)
        : IAvailabilityService
    {
        public List<string> RequestedBeerIds { get; } = [];

        public Task<Result<AvailabilityJson>> GetAvailabilityByWarehouseIdAndBeerIdAsync(
            WarehouseId warehouseId,
            BeerId beerId,
            CancellationToken cancellationToken)
        {
            RequestedBeerIds.Add(beerId.Value);
            return Task.FromResult(
                availability.TryGetValue(beerId.Value, out var projection)
                    ? Result<AvailabilityJson>.Success(projection)
                    : Result<AvailabilityJson>.Error("Not found."));
        }

        public Task<Result<bool>> AddAvailabilityAsync(
            AvailabilityId availabilityId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Result<AvailabilityJson>> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<string>> AddItemStockAsync(
            AvailabilityId availabilityId,
            Quantity quantity,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Result<AvailabilityJson>> GetAvailabilityByBeerIdAsync(
            BeerId beerId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Result<ReorderThreshold>> GetReorderThresholdByBeerIdAsync(
            BeerId beerId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class RecordingServiceBus(
        RecordingAvailabilityService availabilityService) : IServiceBus
    {
        public List<ReserveStock> Commands { get; } = [];
        public int AvailabilityCallsAtDispatch { get; private set; }

        public Task SendAsync<T>(T command, CancellationToken cancellationToken)
            where T : class, ICommand
        {
            AvailabilityCallsAtDispatch = availabilityService.RequestedBeerIds.Count;
            Commands.Add(Assert.IsType<ReserveStock>(command));
            return Task.CompletedTask;
        }
    }
}
