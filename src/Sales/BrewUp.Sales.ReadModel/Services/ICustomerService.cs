using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.Helpers;
using Lena.Core;

namespace BrewUp.Sales.ReadModel.Services;

public interface ICustomerService
{
    Task<Result<bool>> AddCustomerAsync(CustomerId customerId,
        RagioneSociale ragioneSociale,
        PartitaIva partitaIva,
        BeerConsumerLevel consumerLevel,
        Indirizzo indirizzo,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCustomerAsync(CustomerId customerId,
        RagioneSociale ragioneSociale,
        PartitaIva partitaIva,
        BeerConsumerLevel consumerLevel,
        Indirizzo indirizzo,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteCustomerAsync(CustomerId eventAggregateId, CancellationToken cancellationToken);
}