using BrewUp.Purchases.ReadModel.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.Helpers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Purchases.ReadModel.Services;

internal sealed class SupplierService([FromKeyedServices("purchases")] IPersister persister,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory), ISupplierService
{
    public async Task<Result<bool>> AddSupplierAsync(SupplierId supplierId, RagioneSociale ragioneSociale, PartitaIva partitaIva, Indirizzo indirizzo,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var supplier = Supplier.Create(supplierId, ragioneSociale, partitaIva, indirizzo.ToIndirizzoJson());
        var insertResult = await Persister.InsertAsync(supplier, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating supplier: {Error}", error);
                return Result<bool>.Error($"Error creating supplier: {error}");
            });
    }

    public async Task<Result<SupplierJson>> GetSupplierByIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var supplierResult = await Persister.GetByIdAsync<Supplier>(supplierId.Value, cancellationToken);
        if (supplierResult.IsError)
        {
            Logger.LogError("Error retrieving supplier with ID {SupplierId}", supplierId);
            return Result<SupplierJson>.Error($"Error retrieving supplier: {supplierId.Value}");
        }
        
        supplierResult.TryGetValue(out var supplier);
        return Result<SupplierJson>.Success(supplier.ToJson());
    }
}