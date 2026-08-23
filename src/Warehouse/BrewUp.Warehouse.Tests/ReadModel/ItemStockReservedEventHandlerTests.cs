using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Warehouse.ReadModel.EventHandlers;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;

namespace BrewUp.Warehouse.Tests.ReadModel;

public sealed class ItemStockReservedEventHandlerTests
{
    [Fact]
    public async Task Updates_Availability_With_Remaining_Quantity()
    {
        var availabilityId = new AvailabilityId(Guid.CreateVersion7().ToString());
        var remainingQuantity = new Quantity(6, "Bottle");
        var availabilityService = new RecordingAvailabilityService();
        var handler = new ItemStockReservedEventHandler(availabilityService, new NullLoggerFactory());

        await handler.HandleAsync(new ItemStockReserved(
            availabilityId,
            new StockReservationId(Guid.CreateVersion7().ToString()),
            new SalesOrderId(Guid.CreateVersion7().ToString()),
            new Quantity(4, "Bottle"),
            remainingQuantity,
            Guid.CreateVersion7()));

        Assert.Equal(availabilityId.Value, availabilityService.UpdatedAvailabilityId?.Value);
        Assert.Equal(remainingQuantity, availabilityService.UpdatedQuantity);
    }

    private sealed class RecordingAvailabilityService : IAvailabilityService
    {
        public AvailabilityId? UpdatedAvailabilityId { get; private set; }
        public Quantity? UpdatedQuantity { get; private set; }

        public Task<Result<string>> AddItemStockAsync(
            AvailabilityId availabilityId,
            Quantity quantity,
            CancellationToken cancellationToken)
        {
            UpdatedAvailabilityId = availabilityId;
            UpdatedQuantity = quantity;
            return Task.FromResult(Result<string>.Success(availabilityId.Value));
        }

        public Task<Result<bool>> AddAvailabilityAsync(
            AvailabilityId availabilityId,
            WarehouseId warehouseId,
            BeerId beerId,
            Quantity quantity,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<AvailabilityJson>> GetAvailabilityByBeerIdAsync(
            BeerId beerId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<AvailabilityJson>> GetAvailabilityByWarehouseIdAndBeerIdAsync(
            WarehouseId warehouseId,
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
