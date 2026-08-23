using Muflone.Core;

namespace BrewUp.Payment.SharedKernel.DomainIds;

public sealed class PaymentAuthorizationId(string value) : DomainId(value);
