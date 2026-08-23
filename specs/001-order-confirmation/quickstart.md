# Quickstart — Validate Sales Order Confirmation

This guide describes runnable validation scenarios that prove the feature works end-to-end. It references [data-model.md](data-model.md) and [contracts/](contracts/) rather than duplicating shapes. Implementation bodies belong in `tasks.md` / the implementation phase.

## Prerequisites

- .NET 10 SDK installed.
- Solution builds: `dotnet build src/BrewUp.slnx`.
- Tests run with xUnit; aggregate tests use `Muflone.SpecificationTests` (`CommandSpecification` Given/When/Expect), matching existing specs in `BrewUp.Sales.Tests/Domain`.

## Build & test commands

```pwsh
# Build the whole solution
dotnet build src/BrewUp.slnx

# Run Sales context tests (aggregate confirmation specs + architecture fitness)
dotnet test src/Sales/BrewUp.Sales.Tests/BrewUp.Sales.Tests.csproj

# Run Sagas context tests (orchestrator confirmation flow)
dotnet test src/Sagas/BrewUp.Sagas.Tests/BrewUp.Sagas.Tests.csproj
```

## Scenario 1 — Confirm when both evidences present (US1 / FR-002, FR-008)

**Goal**: A Sales Order with both references confirms and raises the Sales-owned event.

- **Given**: an existing `SalesOrder` (`SalesOrderCreated` applied → status `Accepted`).
- **When**: `ConfirmSalesOrder` is handled with a `PaymentAuthorizationId` and a `StockReservationId`.
- **Then (Expect)**: a single `SalesOrderConfirmed` domain event carrying both references; status becomes `Confirmed`.
- **Test**: `BrewUp.Sales.Tests/Domain/ConfirmSalesOrderSuccessfully.cs` (`CommandSpecification<ConfirmSalesOrder>`).

## Scenario 2 — Withhold confirmation when evidence is missing (US3 / FR-002, FR-010, INV-1)

**Goal**: Missing either reference must not produce a confirmation.

- **Given**: an existing `SalesOrder` in `Accepted`.
- **When**: `ConfirmSalesOrder` is handled with only one reference (or the aggregate's `Confirm` is called with a missing id).
- **Then (Expect)**: **no** `SalesOrderConfirmed` event; status unchanged; no invalid state persisted.
- **Test**: `BrewUp.Sales.Tests/Domain/ConfirmRejectedWhenEvidenceMissing.cs` (assert empty `Expect()` / no confirmed event for each missing-reference case).

## Scenario 3 — Confirmation is idempotent (FR-009 / INV-2 / SC-004)

**Goal**: Re-confirming an already-confirmed order produces no second event.

- **Given**: a `SalesOrder` with `SalesOrderConfirmed` already applied (status `Confirmed`).
- **When**: `ConfirmSalesOrder` is handled again.
- **Then (Expect)**: no further `SalesOrderConfirmed`.
- **Test**: `BrewUp.Sales.Tests/Domain/ConfirmIsIdempotent.cs`.

## Scenario 4 — Saga drives confirmation from outcomes (US1+US2 / FR-012, D4)

**Goal**: The saga confirms only after both outcomes, requesting reservation and reacting to evidence.

- **Given**: a started `SalesOrderSaga`.
- **When**: `PaymentAuthorized` then `StockReserved` integration events are handled (in either order).
- **Then**: after the second outcome, the orchestrator sends exactly one `ConfirmSalesOrder` to Sales with both references; with only one outcome present, no `ConfirmSalesOrder` is sent.
- **And (failure)**: handling `StockReservationFailed` (or `PaymentAuthorizationFailed`) records the failure and sends no `ConfirmSalesOrder`; the Sales Order stays pre-confirmation (Q1/Q3, FR-014).
- **Test**: `BrewUp.Sagas.Tests/Orchestrators/SalesOrderSagaConfirmationTests.cs`. Use test doubles to emit the outcome integration events (producers are out of scope — D1/D2).

## Architecture fitness (Principle IV)

- `dotnet test` must keep `SalesArchitectureTests` and `SagasArchitectureTests` green: the Sales domain must not depend on Warehouse/Payment internals, and no cross-module internal coupling is introduced. New evidence types live in `BrewUp.Shared`/context `SharedKernel`, not as embedded foreign models.

## Expected outcomes checklist

- [ ] Confirm-success raises exactly one `SalesOrderConfirmed` with both references (SC-001).
- [ ] No path yields a `Confirmed` status with a missing reference (SC-002).
- [ ] Saga sends `ConfirmSalesOrder` at most once per order (SC-004).
- [ ] Failure outcomes leave the order pre-confirmation, with no void/refund/release by Sales (SC-005, FR-014).
- [ ] Architecture fitness tests remain green.
