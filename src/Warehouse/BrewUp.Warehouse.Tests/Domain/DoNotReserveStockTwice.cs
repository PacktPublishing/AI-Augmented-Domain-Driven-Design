using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Domain.CommandHandlers;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.DomainIds;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Warehouse.Tests.Domain;

public sealed class DoNotReserveStockTwice : CommandSpecification<ReserveStock>
{
    private readonly StockReservationId _reservationId = new(Guid.CreateVersion7().ToString());
    private readonly WarehouseId _warehouseId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly BeerId _beerId = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new StockReserved(
            _reservationId,
            _warehouseId,
            _salesOrderId,
            Rows(),
            Guid.CreateVersion7());
    }

    protected override ReserveStock When() => new(
        _reservationId,
        _warehouseId,
        _salesOrderId,
        Rows(),
        _correlationId);

    protected override ICommandHandlerAsync<ReserveStock> OnHandler() =>
        new ReserveStockCommandHandler(Repository, NullLoggerFactory.Instance);

    protected override IEnumerable<DomainEvent> Expect() => [];

    private IReadOnlyList<ItemRequested> Rows() =>
    [
        new ItemRequested(
            _beerId,
            new Quantity(1, "Bottle"),
            new Quantity(5, "Bottle"))
    ];

    public sealed class FailedReservationReplay : CommandSpecification<ReserveStock>
    {
        private readonly StockReservationId _reservationId =
            new(Guid.CreateVersion7().ToString());
        private readonly WarehouseId _warehouseId =
            new(Guid.CreateVersion7().ToString());
        private readonly SalesOrderId _salesOrderId =
            new(Guid.CreateVersion7().ToString());
        private readonly BeerId _beerId = new(Guid.CreateVersion7().ToString());

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new StockReservationFailed(
                _reservationId,
                _warehouseId,
                _salesOrderId,
                "All requested stock is not available.",
                Guid.CreateVersion7());
        }

        protected override ReserveStock When() => new(
            _reservationId,
            _warehouseId,
            _salesOrderId,
            [
                new ItemRequested(
                    _beerId,
                    new Quantity(2, "Bottle"),
                    new Quantity(1, "Bottle"))
            ],
            Guid.CreateVersion7());

        protected override ICommandHandlerAsync<ReserveStock> OnHandler() =>
            new ReserveStockCommandHandler(Repository, NullLoggerFactory.Instance);

        protected override IEnumerable<DomainEvent> Expect() => [];
    }
}
