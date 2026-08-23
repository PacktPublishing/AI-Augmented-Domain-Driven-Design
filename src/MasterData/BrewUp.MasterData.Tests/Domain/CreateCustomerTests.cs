using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.Messages.Events;
using BrewUp.Shared.Messages.Events.MasterData;
using Muflone.Persistence;

namespace BrewUp.MasterData.Tests.Domain;

public class CreateCustomerTests
{
    [Fact]
    public async Task Can_Serialized_And_Deserialized_IntegrationEvent()
    {
        Serializer serializer = new ();
        CustomerId aggregateId = new CustomerId(Guid.CreateVersion7().ToString());
        RagioneSociale ragioneSociale = new RagioneSociale("Il Grottino del Muflone");
        PartitaIva partitaIva = new PartitaIva("IT12345678901");
        Indirizzo indirizzo = new Indirizzo(new Via("Via Del Bosco"),
            new NumeroCivico("42"),
            new Cap("20100"),
            new Citta("Roma"),
            new Provincia("RM"),
            new Nazione("ITA"));
        
        CustomerCreated @event = new CustomerCreated(aggregateId, ragioneSociale, partitaIva,
            BeerConsumerLevel.Teetotaler, indirizzo);
        
        var serializedMessage =
            await serializer.SerializeAsync(@event, CancellationToken.None);
        
        var deserializedMessage =
            await serializer.DeserializeAsync<CustomerCreated>(serializedMessage, CancellationToken.None);
        
        Assert.Equal(@event.BeerConsumerLevel, deserializedMessage!.BeerConsumerLevel);
    }
}