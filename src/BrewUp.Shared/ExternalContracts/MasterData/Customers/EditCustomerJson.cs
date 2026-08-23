using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.MasterData.Customers;

public class EditCustomerJson
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    [Required]
    public string RagioneSociale { get; set; } = string.Empty;
    [Required]
    public string PartitaIva { get; set; } = string.Empty;
    public IndirizzoJson Indirizzo { get; set; } = new ();
}