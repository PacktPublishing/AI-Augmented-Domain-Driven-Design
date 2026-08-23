using BrewUp.Sagas.SharedKernel.Messages.Events;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUp.Sagas.ReadModel.EventHandlers;

public sealed class SagaSalesOrderSuccessfullyCompletedEventHandler(ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<SagaSalesOrderSuccessfullyCompleted>(loggerFactory)
{
    public override Task HandleAsync(SagaSalesOrderSuccessfullyCompleted @event,
        CancellationToken cancellationToken = new ())
    {
        return Task.CompletedTask;
    }
}