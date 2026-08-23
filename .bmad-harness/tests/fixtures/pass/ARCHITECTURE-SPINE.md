# Sales Order Confirmation Architecture

Payment owns payment authorization and is implemented as a separate module:

- `src/Payment/BrewUp.Payment.SharedKernel/`
- `src/Payment/BrewUp.Payment.Domain/`
- `src/Payment/BrewUp.Payment.ReadModel/`
- `src/Payment/BrewUp.Payment.Infrastructure/`
- `src/Payment/BrewUp.Payment.Facade/`
- `src/Payment/BrewUp.Payment.Tests/`

Warehouse reserves stock in the Warehouse module.
