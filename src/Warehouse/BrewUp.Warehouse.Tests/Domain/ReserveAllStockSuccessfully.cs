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

public sealed class ReserveAllStockSuccessfully : CommandSpecification<ReserveStock>
{
    private readonly StockReservationId _reservationId = new(Guid.CreateVersion7().ToString());
    private readonly WarehouseId _warehouseId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly BeerId _beerA = new(Guid.CreateVersion7().ToString());
    private readonly BeerId _beerB = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given() => [];

    protected override ReserveStock When() => new(
        _reservationId,
        _warehouseId,
        _salesOrderId,
        [
            new ItemRequested(
                _beerA,
                new Quantity(2, "Bottle"),
                new Quantity(5, "Bottle")),
            new ItemRequested(
                _beerB,
                new Quantity(3, "Bottle"),
                new Quantity(3, "Bottle"))
        ],
        _correlationId);

    protected override ICommandHandlerAsync<ReserveStock> OnHandler() =>
        new ReserveStockCommandHandler(Repository, NullLoggerFactory.Instance);

    protected override IEnumerable<DomainEvent> Expect()
    {
        var expectedRows = new[]
        {
            new ItemRequested(
                _beerA,
                new Quantity(2, "Bottle"),
                new Quantity(5, "Bottle")),
            new ItemRequested(
                _beerB,
                new Quantity(3, "Bottle"),
                new Quantity(3, "Bottle"))
        };

        yield return new StockReserved(
            _reservationId,
            _warehouseId,
            _salesOrderId,
            expectedRows,
            _correlationId);
    }
}
