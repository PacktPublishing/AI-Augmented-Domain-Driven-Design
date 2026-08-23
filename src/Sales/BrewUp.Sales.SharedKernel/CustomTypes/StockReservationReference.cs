using Muflone.Core;

namespace BrewUp.Sales.SharedKernel.CustomTypes;

public sealed class StockReservationReference(string value) : DomainId(value);
