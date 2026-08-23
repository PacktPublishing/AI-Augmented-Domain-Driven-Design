using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Warehouse.Domain.CommandHandlers;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using BrewUp.Warehouse.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Warehouse.Tests.Domain;

public class AddItemStockSuccessfully : CommandSpecification<AddItemStock>
{
    private readonly AvailabilityId _availabilityId = new (Guid.CreateVersion7().ToString());
    private readonly WarehouseId _warehouseId = new (Guid.CreateVersion7().ToString());
    private readonly BeerId _beerId = new (Guid.CreateVersion7().ToString());
    private readonly Quantity _quantity = new (10, "Bottle");
    
    private readonly Quantity _newQuantity = new (20, "Bottle");
    
    private readonly Guid _correlationId = Guid.CreateVersion7();
    
    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new AvailabilityCreated(_availabilityId, _warehouseId, _beerId, _quantity);;
    }

    protected override AddItemStock When()
    {
        return new AddItemStock(_availabilityId, _quantity, _correlationId);
    }

    protected override ICommandHandlerAsync<AddItemStock> OnHandler()
    {
        return new AddItemStockCommandHandlerAsync(Repository, new NullLoggerFactory());
    }

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new ItemStockAdded(_availabilityId, _newQuantity);
    }
}