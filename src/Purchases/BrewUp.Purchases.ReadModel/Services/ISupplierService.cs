using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using Lena.Core;

namespace BrewUp.Purchases.ReadModel.Services;

public interface ISupplierService
{
    Task<Result<bool>> AddSupplierAsync(SupplierId supplierId,
        RagioneSociale ragioneSociale,
        PartitaIva partitaIva,
        Indirizzo indirizzo,
        CancellationToken cancellationToken = default);
    
    Task<Result<SupplierJson>> GetSupplierByIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
}