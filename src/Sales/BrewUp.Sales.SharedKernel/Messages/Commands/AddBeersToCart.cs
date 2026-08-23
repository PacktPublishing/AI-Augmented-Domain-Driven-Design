using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Commands;

namespace BrewUp.Sales.SharedKernel.Messages.Commands;

public sealed class AddBeersToCart(SalesOrderId aggregateId, IEnumerable<SalesOrderRowJson> rows) : Command(aggregateId)
{
    public IEnumerable<SalesOrderRowJson> Rows { get; private set; } = rows;
}