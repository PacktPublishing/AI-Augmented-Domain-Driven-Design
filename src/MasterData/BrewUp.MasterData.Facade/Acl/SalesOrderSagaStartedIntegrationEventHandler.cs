using BrewUp.MasterData.SharedKernel.CustomTypes;
using BrewUp.MasterData.SharedKernel.Messages.Commands;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.MasterData.Facade.Acl;

public sealed class SalesOrderSagaStartedIntegrationEventHandler(IServiceBus serviceBus, 
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<SalesOrderSagaStartedIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SalesOrderSagaStartedIntegrationEvent @event,
        CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        VerifyCustomerBudget command = new(new CustomerId(@event.AggregateId.Value), MessageHelpers.GetCorrelationId(@event), 
            new Amount(@event.AmountToCheck, "EUR"));
        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}