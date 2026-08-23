# Dashboards Architecture

## Summary

Dashboards consumes sales integration events and maintains reporting projections for sales by customer and product. It exposes read-only minimal API endpoints.

## Feature Impact

Confirmation may eventually require reporting changes, but no reporting requirement is present in the feature request. Existing sales-created projections remain unchanged unless a later PRD adds confirmed-order reporting.

