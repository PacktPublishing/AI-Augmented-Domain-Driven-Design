using BrewUp.Shared.DomainIds;

namespace BrewUp.Shared.CustomTypes;

public record ItemRequested(BeerId BeerId, Quantity QuantityOrdered, Quantity QuantityAvailable);