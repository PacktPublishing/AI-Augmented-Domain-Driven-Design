using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.Messages.Events.MasterData;

namespace BrewUp.MasterData.Domain.Helpers;

internal static class CustomerHelper
{
    internal static IndirizzoJson ToIndirizzoJson(this Indirizzo indirizzo) =>
        new()
        {
            Via = indirizzo.Via.Value,
            Citta = indirizzo.Citta.Value,
            Cap = indirizzo.Cap.Value,
            Provincia = indirizzo.Provincia.Value,
            Nazione = indirizzo.Nazione.Value
        };

    private static Indirizzo ToIndirizzo(this IndirizzoJson indirizzoJson) =>
        new(new Via(indirizzoJson.Via),
            new NumeroCivico(indirizzoJson.NumeroCivico),
            new Cap(indirizzoJson.Cap),
            new Citta(indirizzoJson.Citta),
            new Provincia(indirizzoJson.Provincia),
            new Nazione(indirizzoJson.Nazione));

    internal static CustomerCreated ToCustomerCreated(this Customer customer) => 
        new (new CustomerId(customer.Id), new RagioneSociale(customer.RagioneSociale),
        new PartitaIva(customer.PartitaIva),
        BeerConsumerLevel.FromName(customer.ConsumerLevel),
        customer.Indirizzo.ToIndirizzo());
    
    internal static CustomerUpdated ToCustomerUpdated(this Customer customer) => 
        new (new CustomerId(customer.Id), new RagioneSociale(customer.RagioneSociale),
            new PartitaIva(customer.PartitaIva),
            BeerConsumerLevel.FromName(customer.ConsumerLevel),
            customer.Indirizzo.ToIndirizzo());
    
    public static CustomerDeleted ToCustomerDeleted(this Customer customer) => 
        new (new CustomerId(customer.Id));
}