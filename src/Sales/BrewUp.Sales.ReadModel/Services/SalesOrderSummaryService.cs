using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesOrderSummary = BrewUp.Sales.ReadModel.Dtos.SalesOrderSummary;

namespace BrewUp.Sales.ReadModel.Services;

internal sealed class SalesOrderSummaryService([FromKeyedServices("sales")] IPersister persister,
    IQueries<SalesOrderSummary> orderSummaryQueries,
    ILoggerFactory loggerFactory) : ServiceBase(persister, loggerFactory), ISalesOrderSummaryService
{
    public async Task<Result<bool>> CreateSalesOrderAsync(SalesOrderId salesOrderId, SalesOrderNumber salesOrderNumber, CustomerId customerId,
        CustomerName customerName, SalesOrderDate orderDate, Price totalAmount, SalesOrderStatus status,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var salesOrder =
            SalesOrderSummary.Create(salesOrderId, salesOrderNumber, customerId, customerName, orderDate,
                totalAmount, status);
        var insertResult = await Persister.InsertAsync(salesOrder, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating sales order sumary: {Error}", error);
                return Result<bool>.Error($"Error creating sales order summary: {error}");
            });
    }

    public async Task<Result<PagedResult<SalesOrderSummaryJson>>> GetSalesOrdersAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await orderSummaryQueries.GetByFilterAsync(null, page, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<SalesOrderSummary> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SalesOrderSummaryJson>>.Success(new PagedResult<SalesOrderSummaryJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SalesOrderSummaryJson>>.Success(new PagedResult<SalesOrderSummaryJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SalesOrderSummaryJson>>.Error("Error retrieving sales orders"));
    }
}