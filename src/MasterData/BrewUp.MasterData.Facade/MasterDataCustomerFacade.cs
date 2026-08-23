using BrewUp.MasterData.Domain.Services;
using BrewUp.MasterData.ReadModel.Services;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.MasterData.Facade;

internal sealed class MasterDataCustomerFacade(ICustomerDomainService masterDataDomainService,
    ICustomerQueryService masterDataQueryService) : IMasterDataCustomerFacade
{
    public Task<Result<string>> RegisterCustomerAsync(RegisterCustomerJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return masterDataDomainService.RegisterCustomerAsync(new CustomerId(Guid.CreateVersion7().ToString()),
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
    
    public Task<Result<bool>> SaveCustomerAsync(EditCustomerJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return masterDataDomainService.SaveCustomerAsync(new CustomerId(body.CustomerId),
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

    public Task<Result<bool>> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        masterDataDomainService.DeleteCustomerAsync(new CustomerId(customerId), cancellationToken);

    public Task<Result<bool>> SetCustomerPropertiesAsync(CustomerPropertiesJson body, CancellationToken cancellationToken)
        => masterDataDomainService.SetCustomerPropertiesAsync(body, cancellationToken);

    public Task<Result<PagedResult<CustomerJson>>> GetCustomersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken) =>
        masterDataQueryService.GetCustomersAsync(pageNumber, pageSize, cancellationToken);

    public Task<Result<CustomerJson>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken) =>
        masterDataQueryService.GetCustomerByIdAsync(customerId, cancellationToken);
}