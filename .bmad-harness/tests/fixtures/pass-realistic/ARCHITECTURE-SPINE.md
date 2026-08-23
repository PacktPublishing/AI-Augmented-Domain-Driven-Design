# Sales Order Confirmation Architecture

Sales behavior remains under `src/Sales/BrewUp.Sales.Domain/`.
Sales stores the external authorization reference at
`src/Sales/BrewUp.Sales.Domain/ValueObjects/PaymentAuthorizationReference.cs`.

Payment owns payment authorization and is implemented as a separate module:

- `src/Payment/BrewUp.Payment.SharedKernel/`
- `src/Payment/BrewUp.Payment.Domain/`
- `src/Payment/BrewUp.Payment.ReadModel/`
- `src/Payment/BrewUp.Payment.Infrastructure/`
- `src/Payment/BrewUp.Payment.Facade/`
- `src/Payment/BrewUp.Payment.Tests/`

Warehouse reservation behavior remains under
`src/Warehouse/BrewUp.Warehouse.Domain/`.
