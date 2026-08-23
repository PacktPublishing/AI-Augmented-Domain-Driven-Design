using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Warehouse.SharedKernel.Messages.Events
{
    public sealed class RequestAccepted(WarehouseId aggregateId, IEnumerable<SalesOrderRowJson> rows) : DomainEvent(aggregateId)
    {
        public IEnumerable<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
    }
}
