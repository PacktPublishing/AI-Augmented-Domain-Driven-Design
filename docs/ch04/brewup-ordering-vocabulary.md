# BrewUp ordering vocabulary

## Order submitted

A domain event raised when the customer sends an order request to BrewUp for evaluation.

It does not imply that the order has been accepted or confirmed.

## Payment authorized

A fact reported when the payment provider authorizes the requested amount.

Authorization does not imply that the payment has been completed or that BrewUp has committed to fulfilling the order.

## Stock reserved

A domain event raised by the warehouse responsibility when the inventory required for the order has been successfully reserved.

It does not imply that the sales order has already been confirmed.

## Order confirmed

A domain event raised when BrewUp commits to fulfilling the order after the required business conditions have been satisfied.

## Vocabulary exclusions

- Do not use "Order received" as a synonym for "Order submitted".
- Do not use "Payment completed" when the domain fact is only authorization.
- Do not use "Inventory allocated" as a synonym for "Stock reserved" unless the warehouse domain explicitly adopts that term.
- Do not use "Order approved" as a replacement for "Order confirmed".
- Do not treat payment authorization as sufficient evidence that the order has been confirmed.
