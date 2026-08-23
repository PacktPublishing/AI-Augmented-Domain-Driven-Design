using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.SharedKernel.Messages.Commands;

public sealed class CreateSummaryByProducts(BeerId aggregateId, BeerName beerName, SalesOrderYear salesOrderYear)
    : Command(aggregateId)
{
    public BeerName BeerName { get; private set; } = beerName;
    public SalesOrderYear SalesOrderYear { get; private set; } = salesOrderYear;
}