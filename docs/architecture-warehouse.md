# Warehouse Architecture

## Summary

Warehouse owns inventory availability and shipment preparation. `Availability` is an event-sourced aggregate for on-hand quantity; MongoDB projections serve availability queries. `Shipment` is a separate event-sourced aggregate.

## Feature-Relevant Behavior

- Availability checks currently read projections and publish requested/available quantities.
- There is no atomic stock reservation command or aggregate behavior.
- The availability integration handler returns after the first successfully found beer.
- Shipment preparation currently reacts to order creation rather than confirmed reservation.

## Required Direction

Reservation must be expressed as Warehouse behavior that validates and reduces/allocates stock atomically enough for the repository's event-driven model.

