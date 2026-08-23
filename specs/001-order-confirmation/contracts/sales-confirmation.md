# Contract: Sales Order confirmation (Sales module)

Sales owns the Sales Order lifecycle and the `Confirmed` transition (BC-001). It stores external decision references as evidence only (BC-009) and never performs Payment or Warehouse decisions.

## Command — `ConfirmSalesOrder`

Location: `src/Sales/BrewUp.Sales.SharedKernel/Messages/Commands/ConfirmSalesOrder.cs`

```csharp
public sealed class ConfirmSalesOrder(SalesOrderId aggregateId,
    Guid correlationId,
    PaymentAuthorizationReference paymentAuthorizationReference,
    StockReservationReference stockReservationReference) : Command(aggregateId, correlationId)
{
    public PaymentAuthorizationReference PaymentAuthorizationReference { get; } = paymentAuthorizationReference;
    public StockReservationReference StockReservationReference { get; } = stockReservationReference;
}
```

Handler `ConfirmSalesOrderCommandHandler` (Sales.Domain) — thin:

```text
load SalesOrder → aggregate.ConfirmOrder(paymentRef, stockRef, command.MessageId) → save
```

## Value objects (Sales.SharedKernel/CustomTypes)

```csharp
public sealed class PaymentAuthorizationReference(string value) : DomainId(value);
public sealed class StockReservationReference(string value) : DomainId(value);
```

These keep Sales decoupled from `Payment.*` and `Warehouse.*` (AR-015) while remaining strongly typed.

## Domain event — `SalesOrderConfirmed`

Location: `src/Sales/BrewUp.Sales.SharedKernel/Messages/Events/SalesOrderConfirmed.cs`

```csharp
public sealed class SalesOrderConfirmed(SalesOrderId aggregateId,
    Guid correlationId,
    PaymentAuthorizationReference paymentAuthorizationReference,
    StockReservationReference stockReservationReference) : DomainEvent(aggregateId, correlationId)
{
    public PaymentAuthorizationReference PaymentAuthorizationReference { get; private set; } = paymentAuthorizationReference;
    public StockReservationReference StockReservationReference { get; private set; } = stockReservationReference;
}
```

> Note: a `SalesOrderConfirmed` **integration** event already exists in `BrewUp.Shared/Messages/Events/Sagas`; the Sales-owned **domain** event above is distinct and stays in Sales.SharedKernel.

## Invariant (enforced in `SalesOrder.ConfirmOrder`)

- Confirm only when **both** references are present (BC-010). If either is missing → not confirmed (no compensation invented, FR-011).
- Idempotent: already `Confirmed` ⇒ no-op (FR-009).

## Status

Add `Confirmed` to `SalesOrderStatus` (id `6`).
