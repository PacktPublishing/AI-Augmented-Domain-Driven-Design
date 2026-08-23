using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class BeersAddedToCart(SalesOrderId aggregateId, IEnumerable<SalesOrderRowJson> rows) : DomainEvent(aggregateId)
{
    public IEnumerable<SalesOrderRowJson> Rows { get; private set; } = rows.ToList();
}