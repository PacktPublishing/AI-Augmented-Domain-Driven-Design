using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using Lena.Core;

namespace BrewUp.MasterData.Domain.Services;

public interface ICustomerDomainService
{
    Task<Result<string>> RegisterCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale,
        PartitaIva partitaIva, Indirizzo indirizzo, CancellationToken cancellationToken = default);

    Task<Result<bool>> SetCustomerPropertiesAsync(CustomerPropertiesJson customerProperties, CancellationToken cancellationToken);
    
    Task<Result<bool>> SaveCustomerAsync(CustomerId customerId, RagioneSociale ragioneSociale, PartitaIva partitaIva,
        Indirizzo indirizzo, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteCustomerAsync(CustomerId customerId, CancellationToken cancellationToken);
}