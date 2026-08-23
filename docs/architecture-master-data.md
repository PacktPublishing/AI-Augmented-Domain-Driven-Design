# MasterData Architecture

## Summary

MasterData manages customer, beer, supplier, and warehouse records through domain services and MongoDB projections. It exposes CRUD/query minimal APIs and publishes master-data integration events.

## Feature-Relevant Behavior

The current saga asks MasterData to verify a customer's enabled state and budget limit. This is a credit/budget check and is not evidence that a payment was authorized.

