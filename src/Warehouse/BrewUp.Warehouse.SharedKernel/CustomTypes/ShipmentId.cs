using Muflone.Core;

namespace BrewUp.Warehouse.SharedKernel.CustomTypes;

public sealed class ShipmentId(string value) : DomainId(value);