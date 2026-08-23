using Muflone.Core;

namespace BrewUp.Warehouse.SharedKernel.CustomTypes;

public sealed class StockReservationId(string value) : DomainId(value);
