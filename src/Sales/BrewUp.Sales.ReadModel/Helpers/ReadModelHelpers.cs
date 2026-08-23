using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts;
using BrewUp.Shared.ExternalContracts.MasterData;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Sales.ReadModel.Helpers;

public static class ReadModelHelpers
{
    public static IEnumerable<SalesOrderRow> ToReadModelEntities(this IEnumerable<SalesOrderRowJson> dtos)
    {
        return dtos.Select(dto =>
            SalesOrderRow.Create(new BeerId(dto.BeerId),
                new BeerName(dto.BeerName),
                dto.Quantity,
                dto.Price));
    }
}