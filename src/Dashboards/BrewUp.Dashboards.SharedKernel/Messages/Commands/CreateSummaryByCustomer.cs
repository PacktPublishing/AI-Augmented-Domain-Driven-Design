using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.SharedKernel.Messages.Commands;

public class CreateSummaryByCustomer(CustomerId aggregateId,
    CustomerName customerName,
    SalesOrderYear salesOrderYear) : Command(aggregateId)
{
    public CustomerName CustomerName { get; private set; } = customerName;
    public SalesOrderYear SalesOrderYear { get; private set; } = salesOrderYear;
}