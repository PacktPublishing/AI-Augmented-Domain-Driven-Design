---
title: BrewUp Order Confirmation
status: final
created: 2026-07-27
updated: 2026-07-27
---

# BrewUp Order Confirmation

## Product outcome

BrewUp must confirm a Sales Order only after two independently owned decisions
exist: Payment has authorized payment and Warehouse has reserved every requested
beer. The change closes the gap between order placement and a trustworthy
`Confirmed` state without moving Payment or Warehouse authority into Sales.

The outcome is a deterministic cross-context flow:

```text
SalesOrderSaga
  -> Payment authorization
  -> complete Warehouse reservation
  -> Sales confirmation with both evidence IDs
```

## Users and value

- Sales operations can treat `Confirmed` as evidence-backed rather than
  optimistic.
- Warehouse operations retain authority over physical stock and reservation.
- Payment operations retain authority over authorization outcomes.
- Developers receive explicit module and dependency boundaries that prevent
  ownership drift.

## Governing product decisions

The approved design,
`docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`,
is the authoritative decision input for this brief. Its **Confirmed Domain
Decisions** section resolves the BC-011 questions for this increment:
**Reservation semantics** selects all-or-nothing reservation;
**Payment authorized, reservation failed** selects an unconfirmed order,
retained authorization, terminal saga failure, and no automatic compensation;
and **Coordination** selects the existing `SalesOrderSaga` and the
Payment -> Warehouse -> Sales order. The governance files remain authoritative
for ownership and constrain how those approved decisions are expressed.

- Sales owns the Sales Order lifecycle and the transition to `Confirmed`.
- Payment owns `PaymentAuthorization`, begins it in a pending state, and alone
  records the simulated provider's authorized outcome.
- Warehouse owns physical stock and an all-or-nothing `StockReservation` for
  the complete Sales Order.
- Sales stores only `PaymentAuthorizationId` and `StockReservationId` as
  external evidence. It does not embed Payment or Warehouse models.
- The existing `SalesOrderSaga` coordinates Payment -> Warehouse -> Sales. It
  retains evidence but does not produce any of the three domain decisions.
- If reservation fails after authorization, the order remains unconfirmed, the
  authorization remains recorded, and the saga records a terminal reservation
  failure.
- Stable external IDs and aggregate state guards prevent duplicate reservation,
  confirmation, authorization recording, or saga completion.

## Scope

### In scope

- A complete six-project Payment module and REST-host registration.
- Pending Payment authorization and a simulated provider callback that records
  an authorized outcome.
- Warehouse-owned, all-or-nothing reservation of all requested rows.
- Sales confirmation that requires both external evidence IDs.
- Saga coordination in the fixed Payment -> Warehouse -> Sales order.
- Read projections for Payment, reservations, and confirmed Sales Orders.
- Domain specification tests, integration-boundary tests, saga tests, and
  architecture fitness tests.

### Explicitly out of scope

- Real payment-provider execution, decline, or timeout semantics.
- Authorization void, refund, compensation, retry, notification, or expiry.
- Partial reservation, reservation release, or reservation expiration.
- Shipment-timing changes, invoicing, and unrelated refactoring.
- Stronger cross-order concurrency guarantees than the repository's current
  event/projection consistency model.

These exclusions are boundaries, not implied future behavior. No excluded
policy may be inferred during implementation.

## Success measures

- A Sales Order cannot become `Confirmed` without both evidence IDs.
- A complete stock request reserves every row or none.
- Reservation failure after authorization sends no confirmation or
  compensation command.
- Replayed commands and outcomes do not repeat authorization, reservation,
  confirmation, or saga completion.
- The Payment module contains the exact SharedKernel, Domain, ReadModel,
  Infrastructure, Facade, and Tests projects and preserves allowed references.
- Targeted module, saga, architecture, and harness checks pass.

## Open decisions

No unresolved decision blocks this increment. Payment decline and provider
timeout meaning, void/refund policy, retry, notification, reservation
release/expiry, shipment timing, invoicing, payment terms, and stronger
concurrency policy remain explicitly outside scope.

## Governance traceability

| Brief decision | Governing rules |
|---|---|
| Separate Sales, Payment, and Warehouse authority | BC-001 through BC-008; AR-001; AR-016 |
| External evidence IDs and confirmation invariant | BC-004; BC-006; BC-009; BC-010; AR-017 |
| Preserve unknown or excluded policy | BC-000; BC-003; BC-005; BC-011 |
| Six-project Payment module and host/solution wiring | AR-001; AR-002; AR-004 through AR-014 |
| Allowed dependencies and explicit integration contracts | AR-015; AR-017 |
| Saga coordinates without stealing authority | BC-008; AR-018 |
| Test-first behavior and fitness coverage | AR-012 |

Confirmed product decisions above trace to the approved design's **Confirmed
Domain Decisions**, **Architecture**, **Contracts**, and **Data and Error
Flow** sections. They are resolved inputs, not inferences from BC-011.
