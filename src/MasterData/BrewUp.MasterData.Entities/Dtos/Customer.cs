using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.ReadModel;

namespace BrewUp.MasterData.Entities.Dtos;

public class Customer : DtoBase
{
    public string RagioneSociale { get; private set; } = string.Empty;
    public string PartitaIva { get; private set; } = string.Empty;
    
    public string ConsumerLevel { get; private set; } = string.Empty;
    
    public IndirizzoJson Indirizzo { get; private set; } = new ();
    
    public decimal BudgetLimit { get; private set; } = 0;
    public bool IsEnabled { get; private set; } = true;
    
    protected Customer() 
    { }

    public static Customer
        Create(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva, IndirizzoJson indirizzo) =>
            new (customerId.Value, ragioneSociale.Value, partitaIva.Value, indirizzo);

    private Customer(string customerId, string ragioneSociale, string partitaIva, IndirizzoJson indirizzo)
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
    
    public void UpdateConsumerLevel(string consumerLevel)
    {
        if (ConsumerLevel == consumerLevel)
            return;
        
        ConsumerLevel = consumerLevel;
    }
    
    public void UpdateBudgetLimit(decimal budgetLimit)
    {
        if (BudgetLimit == budgetLimit)
            return;
        
        BudgetLimit = budgetLimit;
    }
    
    public void UpdateIsEnabled(bool isEnabled) => IsEnabled = isEnabled;
    
    public CustomerJson ToJson() =>
        new (Id, RagioneSociale, PartitaIva, ConsumerLevel, Indirizzo, BudgetLimit, IsEnabled);
}