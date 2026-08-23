using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.ExternalContracts.MasterData;

namespace BrewUp.Shared.Helpers;

public static class IndirizzoHelper
{
    public static IndirizzoJson ToIndirizzoJson(this Indirizzo indirizzo) =>
        new()
        {
            Via = indirizzo.Via.Value,
            Citta = indirizzo.Citta.Value,
            Cap = indirizzo.Cap.Value,
            Provincia = indirizzo.Provincia.Value,
            Nazione = indirizzo.Nazione.Value
        };
}