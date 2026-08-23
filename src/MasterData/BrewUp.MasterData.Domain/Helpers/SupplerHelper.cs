using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.Messages.Events.MasterData;

namespace BrewUp.MasterData.Domain.Helpers;

internal static class SupplerHelper
{
    private static Indirizzo ToIndirizzo(this IndirizzoJson indirizzoJson) =>
        new(new Via(indirizzoJson.Via),
            new NumeroCivico(indirizzoJson.NumeroCivico),
            new Cap(indirizzoJson.Cap),
            new Citta(indirizzoJson.Citta),
            new Provincia(indirizzoJson.Provincia),
            new Nazione(indirizzoJson.Nazione));
    
    internal static SupplierCreated ToSupplierCreated(this Supplier supplier) => 
        new (new SupplierId(supplier.Id), new RagioneSociale(supplier.RagioneSociale),
        new PartitaIva(supplier.PartitaIva),
        supplier.Indirizzo.ToIndirizzo());
}