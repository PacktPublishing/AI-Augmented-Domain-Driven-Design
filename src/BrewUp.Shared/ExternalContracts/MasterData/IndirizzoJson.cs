using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.MasterData;

public class IndirizzoJson
{
    [Required]
    public string Via { get; set; } = string.Empty;
    [Required]
    public string NumeroCivico { get; set; } = string.Empty;
    [Required]
    public string Citta { get; set; } = string.Empty;
    [Required]
    public string Provincia { get; set; } = string.Empty;
    [Required]
    public string Cap { get; set; } = string.Empty;
    [Required]
    public string Nazione { get; set; } = string.Empty;
}