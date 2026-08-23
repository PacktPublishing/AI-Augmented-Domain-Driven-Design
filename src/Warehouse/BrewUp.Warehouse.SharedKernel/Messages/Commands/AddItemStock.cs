using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.SharedKernel.Messages.Commands
{
    public sealed class AddItemStock(AvailabilityId aggregateId, Quantity quantity, 
        Guid correlationId) : Command(aggregateId, correlationId)
    {
        public Quantity Quantity { get; private set; } = quantity;
    }
}
