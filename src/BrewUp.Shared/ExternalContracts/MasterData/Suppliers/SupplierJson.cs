namespace BrewUp.Shared.ExternalContracts.MasterData.Suppliers;

public class SupplierJson
{
    public string SupplierId { get; set; } = string.Empty;
    public string RagioneSociale { get; set; } = string.Empty;
    public string PartitaIva { get; set; } = string.Empty;
    
    public IndirizzoJson Indirizzo { get; set; } = new ();
    
    public bool IsEnabled { get; set; } = true;
}