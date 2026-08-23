using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Helpers;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.MasterData;

public sealed class CustomerCreated(CustomerId aggregateId, 
    RagioneSociale ragioneSociale,
    PartitaIva partitaIva,
    BeerConsumerLevel beerConsumerLevel,
    Indirizzo indirizzo) : IntegrationEvent(aggregateId)
{
    public RagioneSociale RagioneSociale { get; private set; } = ragioneSociale;
    public PartitaIva PartitaIva { get; private set; } = partitaIva;
    public BeerConsumerLevel BeerConsumerLevel { get; private set; } = beerConsumerLevel;
    public Indirizzo Indirizzo { get; private set; } = indirizzo;
}