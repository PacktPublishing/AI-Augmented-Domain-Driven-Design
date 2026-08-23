# Sales Order Confirmation PRD

`SalesOrder.Confirm` requires `PaymentAuthorizationId` and
`StockReservationId` as external decision evidence. Sales records those IDs;
Payment owns authorization and Warehouse owns reservation.

SalesOrder.Confirm requires Payment to authorize payment and Warehouse to reserve stock.
