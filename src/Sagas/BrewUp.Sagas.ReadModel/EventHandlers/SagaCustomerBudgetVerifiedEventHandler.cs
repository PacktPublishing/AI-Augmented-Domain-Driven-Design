using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaCustomerBudgetVerifiedEventHandler(
    IEventBus eventBus,
    ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<SagaCustomerBudgetVerified>(loggerFactory)
{
    public override async Task HandleAsync(SagaCustomerBudgetVerified @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        
        // IntegrationEvent for SagaCustomerBudgetVerified, to be consumed by the SalesOrderSagaOrchestrator
        SagaCustomerBudgetVerifiedIntegrationEvent integrationEvent = new(new IntegrationId(@event.AggregateId.Value),
            correlationId, @event.Order, @event.Customer);
        await eventBus.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}