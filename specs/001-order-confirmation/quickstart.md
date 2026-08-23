# Quickstart: Sales Order Confirmation — validation guide

This guide validates the feature end-to-end. It references [contracts/](contracts/) and [data-model.md](data-model.md) rather than duplicating implementation detail.

## Prerequisites

- .NET 10 SDK
- EventStore (gRPC), MongoDB, and RabbitMQ reachable per `src/BrewUp.Rest/appsettings.json`
- Solution builds: `dotnet build src/BrewUp.slnx`

## Build & test commands

```powershell
# Build the whole solution (includes the new Payment module)
dotnet build src/BrewUp.slnx

# Run all tests
dotnet test src/BrewUp.slnx

# Run only the new/affected module tests
dotnet test src/Payment/BrewUp.Payment.Tests/BrewUp.Payment.Tests.csproj
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj
dotnet test src/Warehouse/BrewUp.Warehouse.Tests/BrewUp.Warehouse.Tests.csproj
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj

# Run the host
dotnet run --project src/BrewUp.Rest/BrewUp.Rest.csproj
```

## Domain validation scenarios (test-first — write these failing first)

These map to the spec's acceptance scenarios and are implemented as `CommandSpecification<T>` specs.

### S1 — Confirm with both evidences present (US1 #1) — Sales

- **Given**: `SalesOrderCreated` (placed order).
- **When**: `ConfirmSalesOrder` with a `PaymentAuthorizationReference` and a `StockReservationReference`.
- **Expect**: `SalesOrderConfirmed`; status becomes `Confirmed`; both references stored.

### S2 — No confirmation when payment evidence missing (US1 #2) — Sales

- **Given**: `SalesOrderCreated`.
- **When**: `ConfirmSalesOrder` with only the stock reference (payment reference empty).
- **Expect**: no `SalesOrderConfirmed` (invariant BC-010 holds).

### S3 — No confirmation when stock evidence missing (US1 #3) — Sales

- Symmetric to S2 with payment reference present, stock reference empty → no confirmation.

### S4 — Idempotent confirmation (US1 #4, FR-009) — Sales

- **Given**: `SalesOrderCreated`, `SalesOrderConfirmed`.
- **When**: `ConfirmSalesOrder` again with the same evidence.
- **Expect**: no second `SalesOrderConfirmed`.

### S5 — Partial reservation still confirms (US1 #5, OQ-2) — Warehouse + Sales

- **Warehouse**: `Given` availability for a subset; `When` `ReserveStock` for all rows; `Expect` `StockReserved` with the reserved subset and a `StockReservationId`.
- **Sales**: confirmation proceeds with the resulting `StockReservationReference` (as S1).

### S6 — Payment authorized / declined (US2/US3) — Payment

- `When` `AuthorizePayment` → `Expect` `PaymentAuthorized` (approve path) or `PaymentDeclined` (decline path). Declined ⇒ saga never reaches the gate ⇒ order stays unconfirmed.

### S7 — Saga gate fires once when both evidences arrive — Sagas

- **Given** saga started; `When` `MarkPaymentAuthorized` then `MarkStockReserved`; `Expect` exactly one `SagaSalesOrderReadyToConfirm`. Reversed arrival order yields the same single gate event (FR-009 / out-of-order).

### S8 — One-sided outcome stays unconfirmed (FR-011 / OQ-1) — Sagas

- `When` `MarkPaymentAuthorized` but `MarkStockReservationRejected`; `Expect` no gate event; no compensation command dispatched.

## Architecture fitness checks (Constitution IV)

- `Payment` module: `Domain` has no reference to `Infrastructure`, `ReadModel`, `Facade`, or any infra framework.
- `Sales` does not reference `Payment.Domain` or `Warehouse.Domain`.
- No module embeds another module's aggregate.

## End-to-end (manual, via host)

1. Place a Sales Order (existing endpoints).
2. Observe the saga dispatch `AuthorizePayment` and `ReserveStock` in parallel.
3. On `PaymentAuthorized` + `StockReserved`, observe `SagaSalesOrderReadyToConfirm` → `ConfirmSalesOrder` → Sales Order status `Confirmed` with both references recorded.

## Success signals (maps to spec Success Criteria)

- SC-001/SC-002: every `Confirmed` order has both references; none confirm without both (S1–S5 green).
- SC-003: idempotency holds (S4, S7 green).
- SC-004: architecture fitness checks pass (0 violations).
- SC-005: an unconfirmed order's outstanding evidence is determinable (saga flags / aggregate references).

## Out of scope (do not implement)

- Reservation expiry/lifetime (OQ-4 — Warehouse).
- Customer notification, shipment, invoicing (OQ-7).
- Compensation/release/void/refund/retry/cancel on negative outcomes (FR-011 / OQ-1).
