using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using BrewUp.Dashboards.SharedKernel.Persistence;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Domain.CommandHandlers;

public sealed class CreateSummaryByProductsCommandHandler(IDashboardsRepository<SalesByProducts> repository, ILoggerFactory loggerFactory) 
    : DashboardsCommandHandlerBaseAsync<CreateSummaryByProducts>(repository, loggerFactory)
{
    public override async Task HandleAsync(CreateSummaryByProducts command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        await repository.AddAsync(SalesByProducts.Create(new BeerId(command.AggregateId.Value),
            new BeerName(command.BeerName.Value), command.SalesOrderYear), cancellationToken);
    }
}