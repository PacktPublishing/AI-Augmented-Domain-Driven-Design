using BrewUp.Shared.CustomTypes;

namespace BrewUp.Shared.ExternalContracts.Sales;

public class OrderRowWithPriceDto
{
    public string BeerId { get; init; } = string.Empty;
    public string BeerName { get; init; } = string.Empty;
    public Quantity Quantity { get; init; } = new (0, string.Empty);
    public Price Price { get; init; } = new(0, string.Empty);
}