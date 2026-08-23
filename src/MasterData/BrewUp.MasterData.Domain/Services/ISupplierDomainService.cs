using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Lena.Core;

namespace BrewUp.MasterData.Domain.Services;

public interface ISupplierDomainService
{
    Task<Result<string>> RegisterSupplierAsync(SupplierId supplierId, RagioneSociale ragioneSociale,
        PartitaIva partitaIva, Indirizzo indirizzo, CancellationToken cancellationToken = default);
}