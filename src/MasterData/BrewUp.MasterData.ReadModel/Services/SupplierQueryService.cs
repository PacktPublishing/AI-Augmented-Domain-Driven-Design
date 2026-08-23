using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.MasterData.ReadModel.Services;

internal sealed class SupplierQueryService(ILoggerFactory loggerFactory, 
    IQueries<Supplier> supplierQueries)
    : ServiceBase(loggerFactory), ISupplierQueryService
{
    public async Task<Result<PagedResult<SupplierJson>>> GetSuppliersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await supplierQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<Supplier> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SupplierJson>>.Success(new PagedResult<SupplierJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SupplierJson>>.Success(new PagedResult<SupplierJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SupplierJson>>.Error("Error retrieving suppliers"));
    }

    public async Task<Result<SupplierJson>> GetSupplierByIdAsync(string supplierId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await supplierQueries.GetByIdAsync(supplierId, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out Supplier result);
                
                return Result<SupplierJson>.Success(result.ToJson());
            },
            _ => Result<SupplierJson>.Error($"Error retrieving supplier with ID {supplierId}"));
    }
}