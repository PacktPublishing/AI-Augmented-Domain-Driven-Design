using BrewUp.Shared.CustomTypes;

namespace BrewUp.Shared.ExternalContracts.MasterData.Beers;

public class BeerJson
{
    public string BeerId { get; set; } = string.Empty;
    public string BeerName { get; set; } = string.Empty;
    public string BeerStyle { get; set; } = string.Empty;
    public decimal AlcoholByVolume { get; set; }
    public string Packaging { get; set; } = string.Empty;
    public Price Price { get; set; } = new(0, string.Empty);
    public bool IsActive { get; set; } = true;
}