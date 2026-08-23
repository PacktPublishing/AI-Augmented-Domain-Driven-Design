# Phase 1 Data Model: Sales Order Confirmation

Models are expressed in domain terms aligned with the existing Muflone CQRS/ES codebase. Field types reference existing shared types (`Quantity`, `Price`, `SalesOrderRowJson`, `ItemRequested`, `DomainId`).

## Aggregate: Sales Order *(Sales-owned — `BrewUp.Sales.Domain.Entities.SalesOrder`)*

Existing aggregate, extended with confirmation evidence and a new status.

### New / changed state

| Field | Type | Notes |
|-------|------|-------|
| `_salesOrderStatus` | `SalesOrderStatus` | **Add `Confirmed` value** to the enum. |
| `_paymentAuthorizationId` | `PaymentAuthorizationId?` | External decision reference (evidence). Empty until recorded. |
| `_stockReservationId` | `StockReservationId?` | External decision reference (evidence). Empty until recorded. |

### New behavior

- `internal void Confirm(PaymentAuthorizationId paymentAuthorizationId, StockReservationId stockReservationId, Guid correlationId)`
  - **Precondition (BC-010 / FR-002)**: both references MUST be present (non-null, non-empty). If either is missing → do **not** raise `SalesOrderConfirmed` (invalid state never produced).
  - **Idempotency (FR-009 / D5)**: if `_salesOrderStatus == Confirmed`, no-op (no second event).
  - **Effect**: raises `SalesOrderConfirmed(salesOrderId, paymentAuthorizationId, stockReservationId, correlationId)`.
- `private void Apply(SalesOrderConfirmed @event)`
  - Sets `_paymentAuthorizationId`, `_stockReservationId`; sets `_salesOrderStatus = SalesOrderStatus.Confirmed`.

### Invariants

- **INV-1**: `Status == Confirmed ⇒ _paymentAuthorizationId` present AND `_stockReservationId` present. (BC-010, SC-002)
- **INV-2**: Confirmation is idempotent — at most one `SalesOrderConfirmed` per aggregate. (FR-009, SC-004)
- **INV-3**: The aggregate stores only id references; it never holds Payment or Warehouse models. (BC-002/BC-009, FR-003)

### State transitions

```text
Accepted ──Confirm(both references present)──▶ Confirmed
Accepted ──Confirm(reference missing)───────▶ Accepted (unchanged; no event)
Confirmed ─Confirm(...)──────────────────────▶ Confirmed (idempotent no-op)
```

> Failure outcomes (`PaymentAuthorizationFailed`, `StockReservationFailed`) do not transition the Sales Order; it remains in its pre-confirmation status (Q1/Q3, FR-014).

## Value Objects

### `PaymentAuthorizationId` *(external decision reference)*

| Property | Type | Notes |
|----------|------|-------|
| `Value` | `string` | Identifier of the Payment authorization outcome. Immutable, value-compared. |

- Shared id: `BrewUp.Shared.DomainIds.PaymentAuthorizationId : DomainId`.
- Sales-local wrapper (if a context-local custom type is needed for the aggregate) lives in `BrewUp.Sales.SharedKernel.CustomTypes`.
- **Owner**: Payment. Sales stores it only as evidence (BC-004/BC-009).

### `StockReservationId` *(external decision reference)*

| Property | Type | Notes |
|----------|------|-------|
| `Value` | `string` | Identifier of the Warehouse stock-reservation outcome. Immutable, value-compared. |

- Shared id: `BrewUp.Shared.DomainIds.StockReservationId : DomainId`.
- Sales-local wrapper in `BrewUp.Sales.SharedKernel.CustomTypes` if required by the aggregate.
- **Owner**: Warehouse. Sales stores it only as evidence (BC-006/BC-009).

## Saga State: Sales Order Saga *(`BrewUp.Sagas.Domain.Entities.SalesOrderSaga`)*

Existing saga, extended to gather confirmation evidence and emit the confirmation command.

### New / changed state

| Field | Type | Notes |
|-------|------|-------|
| `_paymentAuthorizationId` | `string` (empty until set) | Recorded when `PaymentAuthorized` arrives. |
| `_stockReservationId` | `string` (empty until set) | Recorded when `StockReserved` arrives. |

### New behavior (raises saga events; mirrors existing `Mark*` pattern)

- `MarkPaymentAuthorized(string paymentAuthorizationId, Guid correlationId)` — records payment evidence.
- `MarkStockReserved(string stockReservationId, Guid correlationId)` — records reservation evidence.
- `MarkConfirmationFailed(string reason, Guid correlationId)` — records a failure; no Sales Order transition.
- **Coordination rule**: when **both** `_paymentAuthorizationId` and `_stockReservationId` are present, the orchestrator sends `ConfirmSalesOrder` to Sales (once — idempotent guard).

### Orchestrator additions (`SalesOrderSagaOrchestrator`)

- `IIntegrationEventHandlerAsync<PaymentAuthorized>` → `MarkPaymentAuthorized` → maybe-confirm.
- `IIntegrationEventHandlerAsync<StockReserved>` → `MarkStockReserved` → maybe-confirm.
- `IIntegrationEventHandlerAsync<PaymentAuthorizationFailed>` / `IIntegrationEventHandlerAsync<StockReservationFailed>` → `MarkConfirmationFailed`.

## Entity: Sales Order Row *(existing — unchanged)*

`SalesOrderRowJson { BeerId, BeerName, Quantity, Price }` — used to express the requested beers when the saga asks Warehouse to reserve stock. No change required.

## Cross-reference to contracts

See [contracts/commands.md](contracts/commands.md), [contracts/domain-events.md](contracts/domain-events.md), and [contracts/integration-events.md](contracts/integration-events.md) for the exact message shapes.
