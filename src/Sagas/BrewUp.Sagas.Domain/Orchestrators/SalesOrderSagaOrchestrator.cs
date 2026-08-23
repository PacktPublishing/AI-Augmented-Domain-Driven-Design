using BrewUp.Sagas.Domain.Entities;
using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Sagas.SharedKernel.Messages.Commands;
using BrewUp.Shared.Messages.Events.Sagas;
using Lena.Core;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;

namespace BrewUp.Sagas.Domain.Orchestrators;

internal sealed class SalesOrderSagaOrchestrator(IRepository repository) :
    ISalesOrderSagaOrchestrator,
    ISagaStartedByAsync<StartSalesOrderSaga>,
    IIntegrationEventHandlerAsync<CustomerBudgetVerified>,
    IIntegrationEventHandlerAsync<CustomerBudgetUnVerified>,
    IIntegrationEventHandlerAsync<SalesOrderPlaced>,
    IIntegrationEventHandlerAsync<RequestBeersAvailabilityChecked>,
    IIntegrationEventHandlerAsync<SagasSalesOrderAccepted>,
    IIntegrationEventHandlerAsync<PaymentAuthorized>,
    IIntegrationEventHandlerAsync<PaymentAuthorizationFailed>,
    IIntegrationEventHandlerAsync<StockReserved>,
    IIntegrationEventHandlerAsync<StockReservationFailed>
{
    public async Task<Result<string>> StartSagaAsync(StartSalesOrderSaga command, CancellationToken cancellationToken)
    {
        await StartedByAsync(command).ConfigureAwait(false);
        
        return Result.Success(command.AggregateId.Value);
    }
    
    /// <summary>
    /// This is necessary to be able to start the saga from a command,
    /// as the ISagaStartedByAsync interface only has a StartSagaAsync method,
    /// but we want to have the possibility to start the saga from a command handler, and not from a domain event handler.
    /// With this operation, we initialized the saga's state.
    /// </summary>
    /// <param name="command"></param>
    public async Task StartedByAsync(StartSalesOrderSaga command)
    {
        Guid correlationId = Guid.CreateVersion7();
        
        var aggregate = SalesOrderSaga.Start(new SagaId(correlationId.ToString()),
            correlationId, command.SalesOrderNumber, command.SalesOrderDate, command.CustomerId,
            command.WarehouseId, command.SalesOrderDeliveryDate, command.Rows);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), CancellationToken.None).ConfigureAwait(false);
    }
    
    public async Task HandleAsync(CustomerBudgetVerified @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkCustomerBudgetAsVerified(@event.Customer, correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(CustomerBudgetUnVerified @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkAsRejected(@event.Message, correlationId);
        
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(RequestBeersAvailabilityChecked @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);

        aggregate!.MarkAvailabilityChecked(correlationId, @event.Rows);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
    
    public async Task HandleAsync(SalesOrderPlaced @event, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkSalesOrderAsPlaced(correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }
    
    public async Task HandleAsync(SagasSalesOrderAccepted @event, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkSagaAsSuccessfullyCompleted(correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(PaymentAuthorized @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkPaymentAuthorized(@event.PaymentAuthorizationId, correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(PaymentAuthorizationFailed @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkPaymentAuthorizationFailed(@event.Reason, correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(StockReserved @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkStockReserved(@event.StockReservationId, @event.Rows, correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(StockReservationFailed @event, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var correlationId = MessageHelpers.GetCorrelationId(@event);
        var aggregate = await repository
            .GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()), cancellationToken)
            .ConfigureAwait(false);
        aggregate!.MarkStockReservationFailed(@event.Reason, correlationId);
        await repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
    }

    #region Dispose

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SalesOrderSagaOrchestrator()
    {
        Dispose(false);
    }

    #endregion
}