using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Sales.SharedKernel.Messages.Events;

public sealed class QuantityErrorRaised(SalesOrderId aggregateId, Guid correlationId,
    IEnumerable<SalesOrderRowJson> rows,
    string message) : DomainEvent(aggregateId, correlationId)
{
    public IEnumerable<SalesOrderRowJson> Rows { get; } = rows;
}