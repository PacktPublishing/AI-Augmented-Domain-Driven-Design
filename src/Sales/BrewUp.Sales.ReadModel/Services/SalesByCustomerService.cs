using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Sales.ReadModel.Services;

internal sealed class SalesByCustomerService([FromKeyedServices("sales")] IPersister persister,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory), ISalesByCustomerService
{
    public async Task<Result<bool>> CreateSalesByCustermerAsync(CustomerId customerId, CustomerName customerName, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await Persister.GetByIdAsync<SalesByCustomer>(customerId.Value, cancellationToken);
        if (queryResult.IsSuccess)
        {
            queryResult.TryGetValue(out SalesByCustomer salesByCustomer1);
            if (!string.IsNullOrEmpty(salesByCustomer1.Id))
                return Result<bool>.Success(true);
        }
            
            
        
        var salesByCustomer = SalesByCustomer.Create(customerId, customerName);
        var insertResult = await Persister.InsertAsync(salesByCustomer, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating salesByCustomer: {Error}", error);
                return Result<bool>.Error($"Error creating salesByCustomer: {error}");
            });
    }

    public async Task<Result<bool>> IncreaseSalesAsync(CustomerId customerId, Price price, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await Persister.GetByIdAsync<SalesByCustomer>(customerId.Value, cancellationToken);
        if (queryResult.IsError)
            return Result<bool>.Error($"Failed to retrieve salesByCustomer for customerId: {customerId.Value}");
        
        queryResult.TryGetValue(out SalesByCustomer salesByCustomer);
        salesByCustomer.IncreaseSalesAmount(price);
        await Persister.UpdateAsync(salesByCustomer, cancellationToken);
        
        return Result<bool>.Success(true);
    }
}