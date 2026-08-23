using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.SharedKernel.Messages.Commands;

public sealed class IncreaseSummaryByProducts(
    BeerId aggregateId,
    SalesOrderValue salesOrderValue,
    SalesOrderQuantity salesOrderQuantity) : Command(aggregateId)
{
    public SalesOrderValue SalesOrderValue { get; private set; } = salesOrderValue;
    public SalesOrderQuantity SalesOrderQuantity { get; private set; } = salesOrderQuantity;
}