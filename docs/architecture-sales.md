# Sales Architecture

## Summary

Sales owns the `SalesOrder` aggregate, order commands/events, and MongoDB projections. The Facade receives HTTP requests and integration outcomes. Command handlers load aggregates from the repository and persist emitted events.

## Feature-Relevant Behavior

- `AcceptSalesOrder` invokes `SalesOrder.AcceptOrder`.
- `SalesOrderAccepted` sets status to `Accepted` and signals saga completion.
- The availability ACL checks projected quantities but currently sends acceptance even when that check fails.
- Sales contains no payment authorization or stock-reservation state.

## Risks

The terms `Accepted`, `Closed`, and requested `Confirmed` need explicit lifecycle semantics before implementation.

