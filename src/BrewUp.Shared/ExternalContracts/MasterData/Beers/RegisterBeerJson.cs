using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.Validators;

namespace BrewUp.Shared.ExternalContracts.MasterData.Beers;

public class RegisterBeerJson
{
    [Required]
    public string BeerName { get; set; } = string.Empty;
    [Required]
    public string BeerStyle { get; set; } = string.Empty;
    public decimal AlcoholByVolume { get; set; }
    public string Packaging { get; set; } = string.Empty;
    [Required]
    [PriceGreaterThanZero(ErrorMessage = "Price must be greater than 0")]
    [AttributeIsMandatory("Currency", ErrorMessage = "Currency is mandatory")]
    public Price Price { get; set; } = new(0, string.Empty);
    public bool IsActive { get; set; } = true;
}