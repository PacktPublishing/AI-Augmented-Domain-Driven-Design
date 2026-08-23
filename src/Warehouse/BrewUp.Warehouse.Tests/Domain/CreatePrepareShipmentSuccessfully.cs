using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Warehouse.Domain.CommandHandlers;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Warehouse.Tests.Domain;

public sealed class CreatePrepareShipmentSuccessfully : CommandSpecification<PrepareShipment>
{
    private readonly ShipmentId _shipmentId = new (Guid.CreateVersion7().ToString());
    private readonly SalesOrderId _salesOrderId = new (Guid.CreateVersion7().ToString());
    private readonly CustomerId _customerId = new (Guid.CreateVersion7().ToString());
    private readonly DeliveryDate _deliveryDate = new (DateTime.UtcNow.AddDays(7));
    private readonly List<OrderRowDto> _rows = [];
    
    private readonly Guid _correlationId = Guid.CreateVersion7();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield break;
    }

    protected override PrepareShipment When() => new (_shipmentId, _salesOrderId, _customerId,
        _deliveryDate, _rows, _correlationId);

    protected override ICommandHandlerAsync<PrepareShipment> OnHandler() =>
        new PrepareShipmentCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new ShipmentPendingForPreparation(_shipmentId, _salesOrderId, _customerId, _deliveryDate, _rows,
            ShipmentState.PendingPreparation, _correlationId);
    }
}