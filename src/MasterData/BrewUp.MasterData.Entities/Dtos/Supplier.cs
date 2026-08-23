using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.ReadModel;

namespace BrewUp.MasterData.Entities.Dtos;

public class Supplier : DtoBase
{
    public string RagioneSociale { get; private set; } = string.Empty;
    public string PartitaIva { get; private set; } = string.Empty;
    
    public IndirizzoJson Indirizzo { get; private set; } = new ();
    
    public bool IsEnabled { get; private set; } = true;
    
    protected Supplier() 
    { }
    
    public static Supplier
        Create(SupplierId supplierId, RagioneSociale ragioneSociale, PartitaIva partitaIva, IndirizzoJson indirizzo) =>
            new (supplierId.Value, ragioneSociale.Value, partitaIva.Value, indirizzo);
    
    private Supplier(string supplierId, string ragioneSociale, string partitaIva, IndirizzoJson indirizzo)
    {
        Id = supplierId;
        RagioneSociale = ragioneSociale;
        PartitaIva = partitaIva;

        Indirizzo = indirizzo;
    }
    
    public SupplierJson ToJson() =>
        new ()
        {
            SupplierId = Id,
            RagioneSociale = RagioneSociale,
            PartitaIva = PartitaIva,
            Indirizzo = Indirizzo,
            IsEnabled = IsEnabled
        };
}