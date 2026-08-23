# Sagas Architecture

## Summary

The Sagas module owns long-running order-process coordination. `SalesOrderSaga` is event sourced; `SalesOrderSagaOrchestrator` consumes cross-module outcomes and persists saga transitions.

## Current Flow

The saga coordinates customer-budget verification, Sales placement, Warehouse availability, Sales acceptance, and final completion.

## Feature-Relevant Gap

It does not coordinate a payment-authorization outcome or a Warehouse reservation outcome. Its state machine must distinguish requested, successful, and failed outcomes and avoid confirming twice.

