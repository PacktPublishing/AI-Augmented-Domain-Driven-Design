using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.Infrastructure.Hubs;
using BrewUp.Dashboards.SharedKernel.CustomTypes;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using BrewUp.Dashboards.SharedKernel.Persistence;
using BrewUp.Shared.DomainIds;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Domain.CommandHandlers;

public sealed class CreateSummaryByCustomersCommandHandler(IDashboardsRepository<SalesByCustomers> repository,
    IDashboardsHubHelper dashboardsHubHelper,
    ILoggerFactory loggerFactory) 
    : DashboardsCommandHandlerBaseAsync<CreateSummaryByCustomer>(repository, loggerFactory)
{
    public override async Task HandleAsync(CreateSummaryByCustomer command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await repository.AddAsync(SalesByCustomers.Create(new CustomerId(command.AggregateId.Value),
            new CustomerName(command.CustomerName.Value), command.SalesOrderYear), cancellationToken);
        
        await dashboardsHubHelper.TellChildrenThatCustomersDashboardWasUpdated(command.CustomerName.Value, cancellationToken);
    }
}