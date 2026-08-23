namespace BrewUp.Shared.ExternalContracts.MasterData.Customers;

public record CustomerJson(string CustomerId,
    string RagioneSociale,
    string PartitaIva,
    string ConsumerLevel,
    IndirizzoJson Indirizzo,
    decimal BudgetLimit,
    bool IsEnabled);