using Muflone.Core;

namespace BrewUp.Sales.SharedKernel.CustomTypes;

public sealed class PaymentAuthorizationReference(string value) : DomainId(value);
