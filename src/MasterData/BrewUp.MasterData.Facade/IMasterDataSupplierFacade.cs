using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

public interface IMasterDataSupplierFacade
{
    Task<Result<string>> RegisterSupplierAsync(RegisterSupplierJson body, CancellationToken cancellationToken);
    Task<Result<PagedResult<SupplierJson>>> GetSuppliersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);
    Task<Result<SupplierJson>> GetSupplierByIdAsync(string supplierId, CancellationToken cancellationToken);
}