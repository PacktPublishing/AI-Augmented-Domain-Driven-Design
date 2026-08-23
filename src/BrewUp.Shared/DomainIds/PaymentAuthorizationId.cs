using Muflone.Core;

namespace BrewUp.Shared.DomainIds;

public sealed class PaymentAuthorizationId(string value) : DomainId(value);
