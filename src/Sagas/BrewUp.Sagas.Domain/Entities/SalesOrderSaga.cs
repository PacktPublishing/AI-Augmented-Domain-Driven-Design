using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Sagas.SharedKernel.Enums;
using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Core;

namespace BrewUp.Sagas.Domain.Entities;

public class SalesOrderSaga : AggregateRoot
{
    private string _salesOrderId = string.Empty;
    private string _salesOrderNumber = string.Empty;
    private DateTime _salesOrderDate;
    private string _customerId = string.Empty;
    private string _warehouseId = string.Empty;
    private CustomerJson _customer;
    private DateTime _salesOrderDeliveryDate;
    private List<SalesOrderRowJson> _rows = [];
    
    private SagaState _status;
    
    private string _paymentAuthorizationId = string.Empty;
    private string _stockReservationId = string.Empty;
    
    private DateTime _startDate;
    private DateTime _endDate;
    
    protected SalesOrderSaga()
    {}

    internal static SalesOrderSaga Start(SagaId aggregateId, Guid correlationId, string salesOrderNumber,
        DateTime salesOrderDate, string customerId, string warehouseId, DateTime salesOrderDeliveryDate,
        IEnumerable<SalesOrderRowJson> rows)
    {
        return new SalesOrderSaga(aggregateId, correlationId, salesOrderNumber, salesOrderDate, customerId, 
            warehouseId, salesOrderDeliveryDate, rows);
    }

    private SalesOrderSaga(SagaId aggregateId, Guid correlationId, string salesOrderNumber,
        DateTime salesOrderDate, string customerId, string warehouseId, DateTime salesOrderDeliveryDate,
        IEnumerable<SalesOrderRowJson> rows)
    {
        RaiseEvent(new SalesOrderSagaStarted(aggregateId, correlationId, salesOrderNumber, salesOrderDate, customerId,
            warehouseId, salesOrderDeliveryDate, rows));
    }

    private void Apply(SalesOrderSagaStarted @event)
    {
        Id = @event.AggregateId;
        _salesOrderId = @event.AggregateId.Value;
        _salesOrderNumber = @event.SalesOrderNumber;
        _salesOrderDate = @event.SalesOrderDate;
        _customerId = @event.CustomerId;
        _warehouseId = @event.WarehouseId;
        _salesOrderDeliveryDate = @event.SalesOrderDeliveryDate;
        _rows = @event.Rows.ToList();
        
        _startDate = DateTime.UtcNow;
        _status = SagaState.Accepted;
    }

    internal void MarkAsRejected(string message, Guid correlationId)
    {
        RaiseEvent(new SalesOrderSagaRejected(new IntegrationId(Id.Value), correlationId, message));
    }

    private void Apply(SalesOrderSagaRejected @event)
    {
        _endDate = DateTime.UtcNow;
        _status = SagaState.Rejected;
    }

    internal void MarkCustomerBudgetAsVerified(CustomerJson customer, Guid correlationId)
    {
        RaiseEvent(new SagaCustomerBudgetVerified(new CustomerId(_customerId), correlationId, 
            customer,
            new CreateSalesOrderJson
            {
                OrderNumber = _salesOrderNumber,
                OrderDate = _salesOrderDate,
                CustomerId = _customerId,
                DeliveryDate = _salesOrderDeliveryDate,
                Rows = _rows.ToList()
            }));
    }

    internal void MarkAvailabilityChecked(Guid correlationId, IEnumerable<ItemRequested> rows)
    {
        RaiseEvent(new SagaSalesOrderAvailablityChecked(new IntegrationId(Id.Value), 
            correlationId, Id.Value, rows));
    }

    private void Apply(SagaSalesOrderAvailablityChecked @event)
    {
        // just to set the new state into EventStore
    }

    internal void MarkOrderNotAvailable(string message, Guid correlationId)
    {
        RaiseEvent(new SagaOrderRequestRejected(new WarehouseId(_warehouseId), correlationId, message));
    }

    private void Apply(SagaCustomerBudgetVerified @event)
    {
        _customer = @event.Customer;
    }

    internal void MarkSalesOrderAsPlaced(Guid correlationId)
    {
        RaiseEvent(new SagaSalesOrderPlaced(new IntegrationId(Id.Value), correlationId,
            _warehouseId, _rows));
    }
    
    private void Apply(SagaSalesOrderPlaced @event)
    {
        _status = SagaState.Accepted;
    }

    internal void MarkSagaAsSuccessfullyCompleted(Guid correlationId)
    {
        RaiseEvent(new SagaSalesOrderSuccessfullyCompleted(new IntegrationId(Id.Value), correlationId));
    }
    
    private void Apply(SagaSalesOrderSuccessfullyCompleted @event)
    {
        _endDate = DateTime.UtcNow;
        _status = SagaState.Closed;
    }

    // ── Confirmation path ────────────────────────────────────────────

    internal void MarkPaymentAuthorized(string paymentAuthorizationId, Guid correlationId)
    {
        RaiseEvent(new SagaPaymentAuthorized(new IntegrationId(Id.Value), correlationId,
            paymentAuthorizationId));
    }

    private void Apply(SagaPaymentAuthorized @event)
    {
        _paymentAuthorizationId = @event.PaymentAuthorizationId;
        TryRaiseReadyToConfirm(@event.MessageId);
    }

    internal void MarkPaymentAuthorizationFailed(string reason, Guid correlationId)
    {
        RaiseEvent(new SagaPaymentAuthorizationFailed(new IntegrationId(Id.Value), correlationId,
            reason));
    }

    private void Apply(SagaPaymentAuthorizationFailed @event)
    {
        _status = SagaState.Rejected;
    }

    internal void MarkStockReserved(string stockReservationId, IEnumerable<ItemRequested> rows,
        Guid correlationId)
    {
        RaiseEvent(new SagaStockReserved(new IntegrationId(Id.Value), correlationId,
            stockReservationId, rows));
    }

    private void Apply(SagaStockReserved @event)
    {
        _stockReservationId = @event.StockReservationId;
        TryRaiseReadyToConfirm(@event.MessageId);
    }

    internal void MarkStockReservationFailed(string reason, Guid correlationId)
    {
        RaiseEvent(new SagaStockReservationFailed(new IntegrationId(Id.Value), correlationId,
            reason));
    }

    private void Apply(SagaStockReservationFailed @event)
    {
        _status = SagaState.Rejected;
    }

    private void Apply(SagaSalesOrderReadyToConfirm @event)
    {
        // State is already set; orchestrator reacts to this event to dispatch ConfirmSalesOrder.
    }

    /// <summary>
    /// Raises <see cref="SagaSalesOrderReadyToConfirm"/> only when BOTH external references are present.
    /// Implements BC-003: payment must be authorized AND stock must be reserved before confirmation.
    /// </summary>
    private void TryRaiseReadyToConfirm(Guid correlationId)
    {
        if (string.IsNullOrWhiteSpace(_paymentAuthorizationId) ||
            string.IsNullOrWhiteSpace(_stockReservationId))
            return;

        RaiseEvent(new SagaSalesOrderReadyToConfirm(new IntegrationId(Id.Value), correlationId,
            _salesOrderId, _paymentAuthorizationId, _stockReservationId));
    }
}