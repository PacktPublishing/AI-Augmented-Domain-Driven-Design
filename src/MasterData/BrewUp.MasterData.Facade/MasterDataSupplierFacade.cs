using BrewUp.MasterData.Domain.Services;
using BrewUp.MasterData.ReadModel.Services;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

internal sealed class MasterDataSupplierFacade(ISupplierDomainService supplierDomainService,
    ISupplierQueryService supplierQueryService) : IMasterDataSupplierFacade
{
    public Task<Result<string>> RegisterSupplierAsync(RegisterSupplierJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return supplierDomainService.RegisterSupplierAsync(new SupplierId(Guid.CreateVersion7().ToString()),
            new RagioneSociale(body.RagioneSociale),
            new PartitaIva(body.PartitaIva),
            new Indirizzo(new Via(body.Indirizzo.Via),
                new NumeroCivico(body.Indirizzo.NumeroCivico),
                new Cap(body.Indirizzo.Cap),
                new Citta(body.Indirizzo.Citta),
                new Provincia(body.Indirizzo.Provincia),
                new Nazione(body.Indirizzo.Nazione)),
            cancellationToken);
    }

    public Task<Result<PagedResult<SupplierJson>>> GetSuppliersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        => supplierQueryService.GetSuppliersAsync(pageNumber, pageSize, cancellationToken);

    public Task<Result<SupplierJson>> GetSupplierByIdAsync(string supplierId, CancellationToken cancellationToken) =>
        supplierQueryService.GetSupplierByIdAsync(supplierId, cancellationToken);
}