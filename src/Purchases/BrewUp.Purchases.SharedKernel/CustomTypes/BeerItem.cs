using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;

namespace BrewUp.Purchases.SharedKernel.CustomTypes;

public record BeerType(BeerId BeerId, BeerName BeerName, Quantity Quantity, Price Price);