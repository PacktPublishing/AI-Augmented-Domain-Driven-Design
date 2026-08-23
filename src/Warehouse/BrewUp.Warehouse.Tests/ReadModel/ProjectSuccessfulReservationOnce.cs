using System.Linq.Expressions;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel;
using BrewUp.Warehouse.ReadModel.EventHandlers;
using BrewUp.Warehouse.ReadModel.Services;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Persistence;
using AvailabilityProjection = BrewUp.Warehouse.ReadModel.Dtos.Availability;
using ReservationProjection = BrewUp.Warehouse.ReadModel.Dtos.StockReservation;

namespace BrewUp.Warehouse.Tests.ReadModel;

public sealed class ProjectSuccessfulReservationOnce
{
    [Fact]
    public async Task SuccessfulProjectionSubtractsOrderedQuantityExactlyOnce()
    {
        var reservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var persister = new ReservationPersister();
        using var provider = CreateProvider(
            persister,
            AvailabilityProjection.Create(
                new AvailabilityId(Guid.CreateVersion7().ToString()),
                warehouseId,
                beerId,
                new Quantity(10, "Bottle")));
        var handler = new StockReservedEventHandler(
            provider.GetRequiredService<IStockReservationService>(),
            NullLoggerFactory.Instance);

        await handler.HandleAsync(
            new StockReserved(
                reservationId,
                warehouseId,
                salesOrderId,
                [
                    new ItemRequested(
                        beerId,
                        new Quantity(3, "Bottle"),
                        new Quantity(10, "Bottle"))
                ],
                Guid.CreateVersion7()),
            CancellationToken.None);

        var projection = Assert.Single(persister.Reservations);
        Assert.Equal(reservationId.Value, projection.Id);
        Assert.Equal(warehouseId.Value, projection.WarehouseId);
        Assert.Equal(salesOrderId.Value, projection.SalesOrderId);
        Assert.Equal("reserved", projection.Status);
        var row = Assert.Single(projection.Rows);
        Assert.Equal(beerId, row.BeerId);
        Assert.Equal(3, row.QuantityOrdered.Value);

        var availabilityService = provider.GetRequiredService<IAvailabilityService>();
        var first = await availabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync(
            warehouseId,
            beerId,
            CancellationToken.None);
        var second = await availabilityService.GetAvailabilityByWarehouseIdAndBeerIdAsync(
            warehouseId,
            beerId,
            CancellationToken.None);

        Assert.Equal(7, QuantityOf(first));
        Assert.Equal(7, QuantityOf(second));
        Assert.Equal(1, persister.InsertCount);
    }

    [Fact]
    public async Task FalseSuccessfulProjectionResultIsPropagated()
    {
        var persister = new ReservationPersister
        {
            InsertResult = Result<bool>.Success(false)
        };
        var service = new StockReservationService(
            persister,
            new ReservationQueries(persister));
        var handler = new StockReservedEventHandler(
            service,
            NullLoggerFactory.Instance);

        await Assert.ThrowsAsync<PersistenceException>(() =>
            handler.HandleAsync(
                new StockReserved(
                    new StockReservationId(Guid.CreateVersion7().ToString()),
                    new WarehouseId(Guid.CreateVersion7().ToString()),
                    new SalesOrderId(Guid.CreateVersion7().ToString()),
                    [
                        new ItemRequested(
                            new BeerId(Guid.CreateVersion7().ToString()),
                            new Quantity(1, "Bottle"),
                            new Quantity(1, "Bottle"))
                    ],
                    Guid.CreateVersion7()),
                CancellationToken.None));
    }

    private static ServiceProvider CreateProvider(
        ReservationPersister persister,
        AvailabilityProjection availability)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKeyedSingleton<IPersister>("warehouse", persister);
        services.AddReadModel();
        services.AddSingleton<IQueries<AvailabilityProjection>>(
            new AvailabilityQueries(availability));
        services.AddSingleton<IQueries<ReservationProjection>>(
            new ReservationQueries(persister));
        return services.BuildServiceProvider();
    }

    private static decimal QuantityOf(Result<AvailabilityJson> result)
    {
        Assert.True(result.IsSuccess);
        result.TryGetValue(out AvailabilityJson projection);
        return projection.Quantity;
    }

    private sealed class ReservationPersister : IPersister
    {
        public List<ReservationProjection> Reservations { get; } = [];
        public int InsertCount { get; private set; }
        public Result<bool> InsertResult { get; init; } = Result<bool>.Success(true);

        public Task<Result<T>> GetByIdAsync<T>(
            string id,
            CancellationToken cancellationToken)
            where T : DtoBase =>
            Task.FromResult(Result<T>.Error("Not found."));

        public Task<Result<bool>> InsertAsync<T>(
            T entity,
            CancellationToken cancellationToken)
            where T : DtoBase
        {
            Reservations.Add(Assert.IsType<ReservationProjection>(entity));
            InsertCount++;
            return Task.FromResult(InsertResult);
        }

        public Task<Result<bool>> UpdateAsync<T>(
            T entity,
            CancellationToken cancellationToken)
            where T : DtoBase =>
            Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> DeleteAsync<T>(
            T entity,
            CancellationToken cancellationToken)
            where T : DtoBase =>
            Task.FromResult(Result<bool>.Success(true));
    }

    private sealed class AvailabilityQueries(AvailabilityProjection availability)
        : IQueries<AvailabilityProjection>
    {
        public Task<Result<AvailabilityProjection>> GetByIdAsync(
            string id,
            CancellationToken cancellationToken) =>
            Task.FromResult(Result<AvailabilityProjection>.Success(availability));

        public Task<Result<PagedResult<AvailabilityProjection>>> GetByFilterAsync(
            Expression<Func<AvailabilityProjection, bool>>? query,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var results = query is null || query.Compile()(availability)
                ? new[] { availability }
                : [];
            return Task.FromResult(Result<PagedResult<AvailabilityProjection>>.Success(
                new PagedResult<AvailabilityProjection>(
                    results,
                    page,
                    pageSize,
                    results.Length)));
        }
    }

    private sealed class ReservationQueries(ReservationPersister persister)
        : IQueries<ReservationProjection>
    {
        public Task<Result<ReservationProjection>> GetByIdAsync(
            string id,
            CancellationToken cancellationToken)
        {
            var result = persister.Reservations.SingleOrDefault(item => item.Id == id);
            return Task.FromResult(
                result is null
                    ? Result<ReservationProjection>.Error("Not found.")
                    : Result<ReservationProjection>.Success(result));
        }

        public Task<Result<PagedResult<ReservationProjection>>> GetByFilterAsync(
            Expression<Func<ReservationProjection, bool>>? query,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var results = query is null
                ? persister.Reservations.ToArray()
                : persister.Reservations.Where(query.Compile()).ToArray();
            return Task.FromResult(Result<PagedResult<ReservationProjection>>.Success(
                new PagedResult<ReservationProjection>(
                    results,
                    page,
                    pageSize,
                    results.Length)));
        }
    }
}
