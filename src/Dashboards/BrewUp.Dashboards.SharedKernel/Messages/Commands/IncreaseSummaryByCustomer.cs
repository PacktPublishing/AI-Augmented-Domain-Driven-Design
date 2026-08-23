using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.SharedKernel.Messages.Commands;

public class IncreaseSummaryByCustomer(CustomerId aggregateId,
    SalesOrderValue salesOrderValue) : Command(aggregateId)
{
    public SalesOrderValue SalesOrderValue { get; private set; } = salesOrderValue;
}