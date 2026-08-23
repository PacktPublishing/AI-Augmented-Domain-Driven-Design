using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.ReadModel;

namespace BrewUp.Sales.ReadModel.Dtos;

public class Customer : DtoBase
{
    public string RagioneSociale { get; private set; } = string.Empty;
    public string PartitaIva { get; private set; } = string.Empty;
    
    public string ConsumerLevel { get; private set; } = string.Empty;
    
    public IndirizzoJson Indirizzo { get; private set; } = new ();
    
    protected Customer() 
    { }

    public static Customer
        Create(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva, BeerConsumerLevel consumerLevel, IndirizzoJson indirizzo) =>
            new (customerId.Value, ragioneSociale.Value, partitaIva.Value, consumerLevel.Name, indirizzo);

    private Customer(string customerId, string ragioneSociale, string partitaIva, string consumerLevel, IndirizzoJson indirizzo)
    {
        Id = customerId;
        RagioneSociale = ragioneSociale;
        PartitaIva = partitaIva;

        ConsumerLevel = BeerConsumerLevel.Teetotaler.Name;

        Indirizzo = indirizzo;
    }
    
    public void UpdateRagioneSociale(RagioneSociale ragioneSociale)
    {
        if (RagioneSociale == ragioneSociale.Value)
            return;
        
        RagioneSociale = ragioneSociale.Value;
    }

    public void UpdatePartitaIva(PartitaIva partitaIva)
    {
        if (PartitaIva == partitaIva.Value)
            return;
        
        PartitaIva = partitaIva.Value;
    }
    
    public void UpdateIndirizzo(IndirizzoJson indirizzo)
    {
        if (Indirizzo.Equals(indirizzo))
            return;
        
        Indirizzo = indirizzo;
    }
}