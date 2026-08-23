# Contract: Payment authorization (Payment module)

All messages use Muflone base types. IDs derive from `Muflone.Core.DomainId`. `Guid.CreateVersion7()` for all generated ids; `ConfigureAwait(false)` on awaits.

## Command — `AuthorizePayment`

Location: `src/Payment/BrewUp.Payment.SharedKernel/Messages/Commands/AuthorizePayment.cs`

```csharp
public sealed class AuthorizePayment(PaymentAuthorizationId aggregateId,
    Guid correlationId,
    string salesOrderId,
    Price amount) : Command(aggregateId, correlationId)
{
    public string SalesOrderId { get; } = salesOrderId;
    public Price Amount { get; } = amount;
}
```

Handler `AuthorizePaymentCommandHandler` (Payment.Domain): load/create `PaymentAuthorization` → call `Authorize(...)` → save. No business logic in the handler.

## Domain events (Payment.SharedKernel/Messages/Events)

```csharp
public sealed class PaymentAuthorized(PaymentAuthorizationId aggregateId,
    Guid correlationId, string salesOrderId) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
}

public sealed class PaymentDeclined(PaymentAuthorizationId aggregateId,
    Guid correlationId, string salesOrderId, string reason) : DomainEvent(aggregateId, correlationId)
{
    public string SalesOrderId { get; private set; } = salesOrderId;
    public string Reason { get; private set; } = reason;
}
```

## Saga-facing integration events (BrewUp.Shared/Messages/Events/Sagas)

Published by `Payment.ReadModel` event handlers; consumed by the saga.

```csharp
public sealed class PaymentAuthorizedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId, string paymentAuthorizationId, string salesOrderId)
    : IntegrationEvent(aggregateId, correlationId)
{ /* paymentAuthorizationId, salesOrderId */ }

public sealed class PaymentDeclinedIntegrationEvent(IntegrationId aggregateId,
    Guid correlationId, string salesOrderId, string reason)
    : IntegrationEvent(aggregateId, correlationId)
{ /* salesOrderId, reason */ }
```

**Ownership (BC-003/BC-004)**: Payment produces these outcomes. Sales never authorizes payment and never interprets timeouts; it reacts only to definitive outcomes.
