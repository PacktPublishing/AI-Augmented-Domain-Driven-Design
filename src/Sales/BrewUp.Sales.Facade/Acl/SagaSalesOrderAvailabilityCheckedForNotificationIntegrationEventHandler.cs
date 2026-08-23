using BrewUp.Sales.ReadModel.Services;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sales.Facade.Acl;

public sealed class SagaSalesOrderAvailabilityCheckedForNotificationIntegrationEventHandler(ISalesOrderService salesOrderService,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<SagaSalesOrderAvailabilityCheckedIntegrationEvent>(loggerFactory)
{
    public override Task HandleAsync(SagaSalesOrderAvailabilityCheckedIntegrationEvent @event,
        CancellationToken cancellationToken = new ())
    {
        return Task.CompletedTask;
    }
}