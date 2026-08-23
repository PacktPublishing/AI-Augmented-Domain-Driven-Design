using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Domain.CommandHandlers;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Warehouse.Tests.Domain;

public sealed class ReserveItemStockSuccessfully : CommandSpecification<ReserveItemStock>
{
    private readonly AvailabilityId _availabilityId = new(Guid.CreateVersion7().ToString());
    private readonly WarehouseId _warehouseId = new(Guid.CreateVersion7().ToString());
    private readonly BeerId _beerId = new(Guid.CreateVersion7().ToString());
    private readonly Quantity _availableQuantity = new(10, "Bottle");
    private readonly Quantity _reservedQuantity = new(4, "Bottle");
    private readonly Quantity _remainingQuantity = new(6, "Bottle");
    private readonly StockReservationId _stockReservationId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new AvailabilityCreated(_availabilityId, _warehouseId, _beerId, _availableQuantity);
    }

    protected override ReserveItemStock When() =>
        new(_availabilityId, _stockReservationId, _salesOrderId, _reservedQuantity, _correlationId);

    protected override ICommandHandlerAsync<ReserveItemStock> OnHandler() =>
        new ReserveItemStockCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new ItemStockReserved(_availabilityId, _stockReservationId, _salesOrderId,
            _reservedQuantity, _remainingQuantity, _correlationId);
    }
}
