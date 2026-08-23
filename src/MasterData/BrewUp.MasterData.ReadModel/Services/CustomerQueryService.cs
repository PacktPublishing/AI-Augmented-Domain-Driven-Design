using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ExternalContracts.MasterData.Customers;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging;

namespace BrewUp.MasterData.ReadModel.Services;

internal sealed class CustomerQueryService(ILoggerFactory loggerFactory, 
    IQueries<Customer> customerQueries)
    : ServiceBase(loggerFactory), ICustomerQueryService
{
    /// <summary>
    /// Railway-Orineted Programming pattern
    /// Benefits:
    /// Consistency: The code follows the same Railway-Oriented Programming pattern
    /// Declarative: The code is more declarative - you declare what should happen in each case (success/error) rather than imperatively checking conditions
    ///     Type-safe: The Railway pattern ensures all code paths are handled through the type system
    ///     No imperative checks: Eliminates the manual IsSuccess check and early return
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<PagedResult<CustomerJson>>> GetCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await customerQueries.GetByFilterAsync(null, pageNumber, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<Customer> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<CustomerJson>>.Success(new PagedResult<CustomerJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<CustomerJson>>.Success(new PagedResult<CustomerJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<CustomerJson>>.Error("Error retrieving customers"));
    }

    public async Task<Result<CustomerJson>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await customerQueries.GetByIdAsync(customerId, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out Customer result);
                
                return Result<CustomerJson>.Success(result.ToJson());
            },
            _ => Result<CustomerJson>.Error($"Error retrieving customer with ID {customerId}"));
    }
}