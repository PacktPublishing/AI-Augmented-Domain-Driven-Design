using Muflone.Core;

namespace BrewUp.Warehouse.SharedKernel.DomainIds;

public sealed class StockReservationId(string value) : DomainId(value);
