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

public sealed class DoNotProjectFailedReservationAsAvailability
{
    [Fact]
    public async Task FailedProjectionContainsNoRowsAndSubtractsNothing()
    {
        var reservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var warehouseId = new WarehouseId(Guid.CreateVersion7().ToString());
        var beerId = new BeerId(Guid.CreateVersion7().ToString());
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var persister = new ReservationPersister();
        var availability = AvailabilityProjection.Create(
            new AvailabilityId(Guid.CreateVersion7().ToString()),
            warehouseId,
            beerId,
            new Quantity(10, "Bottle"));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKeyedSingleton<IPersister>("warehouse", persister);
        services.AddReadModel();
        services.AddSingleton<IQueries<AvailabilityProjection>>(
            new AvailabilityQueries(availability));
        services.AddSingleton<IQueries<ReservationProjection>>(
            new ReservationQueries(persister));
        using var provider = services.BuildServiceProvider();
        var handler = new StockReservationFailedEventHandler(
            provider.GetRequiredService<IStockReservationService>(),
            NullLoggerFactory.Instance);

        await handler.HandleAsync(
            new StockReservationFailed(
                reservationId,
                warehouseId,
                salesOrderId,
                "All requested stock is not available.",
                Guid.CreateVersion7()),
            CancellationToken.None);

        var projection = Assert.Single(persister.Reservations);
        Assert.Equal("failed", projection.Status);
        Assert.Empty(projection.Rows);
        Assert.Equal(
            "All requested stock is not available.",
            projection.FailureReason);

        var result = await provider
            .GetRequiredService<IAvailabilityService>()
            .GetAvailabilityByWarehouseIdAndBeerIdAsync(
                warehouseId,
                beerId,
                CancellationToken.None);
        Assert.True(result.IsSuccess);
        result.TryGetValue(out AvailabilityJson remaining);
        Assert.Equal(10, remaining.Quantity);
    }

    [Fact]
    public async Task ErrorFailedProjectionResultIsPropagated()
    {
        var persister = new ReservationPersister
        {
            InsertResult = Result<bool>.Error("insert failed")
        };
        var handler = new StockReservationFailedEventHandler(
            new StockReservationService(
                persister,
                new ReservationQueries(persister)),
            NullLoggerFactory.Instance);

        await Assert.ThrowsAsync<PersistenceException>(() =>
            handler.HandleAsync(
                new StockReservationFailed(
                    new StockReservationId(Guid.CreateVersion7().ToString()),
                    new WarehouseId(Guid.CreateVersion7().ToString()),
                    new SalesOrderId(Guid.CreateVersion7().ToString()),
                    "All requested stock is not available.",
                    Guid.CreateVersion7()),
                CancellationToken.None));
    }

    private sealed class ReservationPersister : IPersister
    {
        public List<ReservationProjection> Reservations { get; } = [];
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
            CancellationToken cancellationToken) =>
            Task.FromResult(Result<ReservationProjection>.Error("Not found."));

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
