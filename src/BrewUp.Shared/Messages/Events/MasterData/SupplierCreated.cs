using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.MasterData;

public sealed class SupplierCreated(SupplierId aggregateId, 
    RagioneSociale ragioneSociale,
    PartitaIva partitaIva,
    Indirizzo indirizzo) : IntegrationEvent(aggregateId)
{
    public RagioneSociale RagioneSociale { get; private set; } = ragioneSociale;
    public PartitaIva PartitaIva { get; private set; } = partitaIva;
    public Indirizzo Indirizzo { get; private set; } = indirizzo;
}