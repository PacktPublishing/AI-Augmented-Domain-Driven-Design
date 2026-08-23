using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Shared.Messages.Events.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace BrewUp.Sales.Facade.Acl;

public sealed class SagaSalesOrderAvailabilityCheckedIntegrationEventHandler(
    ISalesOrderService salesOrderService, IServiceBus serviceBus,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<SagaSalesOrderAvailabilityCheckedIntegrationEvent>(loggerFactory)
{
    public override async Task HandleAsync(SagaSalesOrderAvailabilityCheckedIntegrationEvent @event,
        CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var canConfirmSalesOrderResult = await salesOrderService
            .ChkAvailabilityForSagaRowsAsync(@event.Rows, cancellationToken).ConfigureAwait(false);
        // if (canConfirmSalesOrderResult.IsError)
        //     return;

        AcceptSalesOrder command = new(new SalesOrderId(@event.SalesOrderId), MessageHelpers.GetCorrelationId(@event));
        await serviceBus.SendAsync(command, cancellationToken).ConfigureAwait(false);
    }
}