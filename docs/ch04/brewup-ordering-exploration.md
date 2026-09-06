# BrewUp Ordering Exploration

## Scope

This artifact describes the early exploration of the BrewUp sales-order flow. It is not yet a final domain model.

## Candidate events

- Order submitted
- Payment authorized
- Order stored
- Stock reserved
- Confirmation email sent
- Order confirmed

## Known distinctions

- Payment authorization does not automatically imply order confirmation.
- Ordering and warehouse responsibilities must remain distinguishable.
- UI actions and persistence details are not automatically domain events.

## Open questions

- Which conditions must be satisfied before the order can be confirmed?
- Who owns a stock reservation?
- Is confirmation a business commitment to the customer?
- Which items in the candidate list are technical side effects?
