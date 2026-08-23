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
    
    private DateTime _startDate;
    private DateTime _endDate;

    // Confirmation gate evidence (US2)
    private bool _paymentAuthorized;
    private bool _stockReserved;
    private string _paymentAuthorizationId = string.Empty;
    private string _stockReservationId = string.Empty;
    
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

    /// <summary>
    /// Initiates parallel requests for payment authorization and stock reservation (US2, AR-010).
    /// Saga coordinates — it does not own either decision.
    /// </summary>
    internal void InitiateConfirmationRequests(Price amount, Guid correlationId)
    {
        var integrationId = new IntegrationId(Id.Value);

        RaiseEvent(new SagaRequestsPaymentAuthorization(integrationId, correlationId,
            _salesOrderId, amount));

        RaiseEvent(new SagaRequestsStockReservation(integrationId, correlationId,
            _salesOrderId, _warehouseId, _rows));
    }

    private void Apply(SagaRequestsPaymentAuthorization @event)
    {
        // coordination state only
    }

    private void Apply(SagaRequestsStockReservation @event)
    {
        // coordination state only
    }

    /// <summary>
    /// Records that Payment BC authorized the payment. Fires confirmation gate if both evidences present.
    /// </summary>
    internal void MarkPaymentAuthorized(string paymentAuthorizationId, Guid correlationId)
    {
        _paymentAuthorized = true;
        _paymentAuthorizationId = paymentAuthorizationId;

        CheckConfirmationGate(correlationId);
    }

    private void Apply(SagaSalesOrderReadyToConfirm @event)
    {
        _status = SagaState.Accepted;
    }

    /// <summary>
    /// Records that Warehouse BC reserved the stock. Fires confirmation gate if both evidences present.
    /// </summary>
    internal void MarkStockReserved(string stockReservationId, Guid correlationId)
    {
        _stockReserved = true;
        _stockReservationId = stockReservationId;

        CheckConfirmationGate(correlationId);
    }

    private void CheckConfirmationGate(Guid correlationId)
    {
        if (!_paymentAuthorized || !_stockReserved)
            return;

        // Both evidences present — gate fires exactly once (idempotent: aggregate status guards re-raise)
        if (_status == SagaState.Closed)
            return;

        RaiseEvent(new SagaSalesOrderReadyToConfirm(new IntegrationId(Id.Value), correlationId,
            _salesOrderId, _paymentAuthorizationId, _stockReservationId));
    }

    /// <summary>
    /// Records that Payment BC declined payment. No compensation in scope (OQ-4).
    /// </summary>
    internal void MarkPaymentDeclined(string reason, Guid correlationId)
    {
        RaiseEvent(new SagaPaymentDeclined(new IntegrationId(Id.Value), correlationId,
            _salesOrderId, reason));
    }

    private void Apply(SagaPaymentDeclined @event)
    {
        _status = SagaState.Rejected;
    }

    /// <summary>
    /// Records that Warehouse BC rejected stock reservation. No compensation in scope (OQ-7).
    /// </summary>
    internal void MarkStockReservationRejected(string reason, Guid correlationId)
    {
        RaiseEvent(new SagaStockReservationRejected(new IntegrationId(Id.Value), correlationId,
            _salesOrderId, reason));
    }

    private void Apply(SagaStockReservationRejected @event)
    {
        _status = SagaState.Rejected;
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
}