# Contract: Saga coordination (Sagas module)

The existing `SalesOrderSaga` / `SalesOrderSagaOrchestrator` is extended to coordinate confirmation. The saga coordinates only; it does not own Payment or Warehouse decisions (AR-018, BC-010).

## Orchestrator — new integration-event handlers

`SalesOrderSagaOrchestrator` additionally implements:

```csharp
IIntegrationEventHandlerAsync<PaymentAuthorizedIntegrationEvent>
IIntegrationEventHandlerAsync<PaymentDeclinedIntegrationEvent>
IIntegrationEventHandlerAsync<StockReservedIntegrationEvent>
IIntegrationEventHandlerAsync<StockReservationRejectedIntegrationEvent>
```

Each handler follows the existing pattern:

```text
correlationId = MessageHelpers.GetCorrelationId(@event)
aggregate = repository.GetByIdAsync<SalesOrderSaga>(new SagaId(correlationId.ToString()))
aggregate!.Mark...(...)
repository.SaveAsync(aggregate, Guid.CreateVersion7(), ct)
```

## Requests (in parallel — OQ-5)

When the saga is ready to seek confirmation evidence, it dispatches **both** requests with no ordering dependency:

- `AuthorizePayment` → Payment (via Payment ACL / command bus).
- `ReserveStock` → Warehouse (via Warehouse ACL / command bus).

The dispatch follows the existing BrewUp pattern: the saga raises a gate/request domain event; `Sagas.ReadModel` publishes the corresponding integration event; the target module's ACL handler dispatches the module command.

## Gate event — `SagaSalesOrderReadyToConfirm`

Location: `src/Sagas/BrewUp.Sagas.SharedKernel/Messages/Events/SagaSalesOrderReadyToConfirm.cs`

```csharp
public sealed class SagaSalesOrderReadyToConfirm(SagaId aggregateId,
    Guid correlationId,
    string salesOrderId,
    string paymentAuthorizationId,
    string stockReservationId) : DomainEvent(aggregateId, correlationId)
{ /* salesOrderId, paymentAuthorizationId, stockReservationId */ }
```

Raised by `SalesOrderSaga` **exactly once**, only when both `_paymentAuthorized` and `_stockReserved` are true.

## Saga → Sales integration event

`Sagas.ReadModel` publishes:

```csharp
public sealed class SagaSalesOrderReadyToConfirmIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId, string salesOrderId, string paymentAuthorizationId, string stockReservationId)
    : IntegrationEvent(aggregateId, correlationId);
```

The Sales Facade ACL handler `SagaSalesOrderReadyToConfirmIntegrationEventHandler` dispatches `ConfirmSalesOrder` (wrapping the ids in `PaymentAuthorizationReference` / `StockReservationReference`).

## Negative outcomes (FR-011 / OQ-1)

On `PaymentDeclinedIntegrationEvent` or `StockReservationRejectedIntegrationEvent`, the saga records the negative outcome and the Sales Order stays unconfirmed. **No** release, void, refund, retry, or cancellation is dispatched (not owned by Sales; not in scope).

## Flow (happy path)

```text
Saga → AuthorizePayment ─┐ (parallel)
Saga → ReserveStock ─────┘
Payment → PaymentAuthorizedIntegrationEvent → Saga.MarkPaymentAuthorized
Warehouse → StockReservedIntegrationEvent → Saga.MarkStockReserved
Saga gate (both true) → SagaSalesOrderReadyToConfirm → ReadModel → SagaSalesOrderReadyToConfirmIntegrationEvent
Sales ACL → ConfirmSalesOrder → SalesOrder.ConfirmOrder → SalesOrderConfirmed (Status = Confirmed)
```
