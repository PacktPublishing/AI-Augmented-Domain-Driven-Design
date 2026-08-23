using BrewUp.MasterData.Domain.Helpers;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;
using Lena.Asyncs;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.MasterData.Domain.Services;

internal sealed class SupplierDomainService([FromKeyedServices("masterdata")] IPersister persister,
    IIntegrationEventPublisher integrationEventPublisher) : ISupplierDomainService
{
    public async Task<Result<string>> RegisterSupplierAsync(SupplierId supplierId, RagioneSociale ragioneSociale, PartitaIva partitaIva,
        Indirizzo indirizzo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Supplier supplier = Supplier.Create(supplierId, ragioneSociale, partitaIva,
            indirizzo.ToIndirizzoJson());

        // Railway-Oriented Programming pattern
        return (await (await persister.InsertAsync(supplier, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(supplier.ToSupplierCreated(), cancellationToken)))
            .Match(
                _ => Result<string>.Success(supplierId.Value),
                Result<string>.Error);
    }
}