using System.Linq.Expressions;
using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Sales.ReadModel.Services;

internal sealed class SalesOrderService([FromKeyedServices("sales")] IPersister persister,
    IQueries<SalesOrder> orderQueries,
    ILoggerFactory loggerFactory) : ServiceBase(persister, loggerFactory), ISalesOrderService
{
    public async Task<Result<bool>> CreateSalesOrderAsync(SalesOrderId salesOrderId, SalesOrderNumber salesOrderNumber, CustomerId customerId,
        CustomerName customerName, SalesOrderDate orderDate, IEnumerable<SalesOrderRowJson> rows, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var salesOrder = SalesOrder.CreateSalesOrder(salesOrderId, salesOrderNumber, customerId, customerName, orderDate, rows);
        var insertResult = await Persister.InsertAsync(salesOrder, cancellationToken);

        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating sales order: {Error}", error);
                return Result<bool>.Error($"Error creating sales order: {error}");
            });
    }

    public async Task<Result<PagedResult<SalesOrderJson>>> GetSalesOrdersAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await orderQueries.GetByFilterAsync(null, page, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<SalesOrder> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SalesOrderJson>>.Success(new PagedResult<SalesOrderJson>(
                        pagedResult.Results.Select(r => r.ToJson()), 
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SalesOrderJson>>.Success(new PagedResult<SalesOrderJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SalesOrderJson>>.Error("Error retrieving sales orders"));
    }

    public async Task<Result<SalesOrderJson>> GetSalesOrderByIdAsync(string salesOrderId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await orderQueries.GetByIdAsync(salesOrderId, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out SalesOrder salesOrder);
                return Result<SalesOrderJson>.Success(salesOrder.ToJson());
            },
            _ => Result<SalesOrderJson>.Error("Error retrieving sales order"));
    }

    public async Task<Result<string>> AddBeersToSalesOrderAsync(SalesOrderId salesOrderId, 
        IEnumerable<SalesOrderRowJson> rows,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var persisterResult = await Persister.GetByIdAsync<SalesOrder>(salesOrderId.Value, cancellationToken);
        if (!persisterResult.IsSuccess)
            return Result<string>.Error("Error retrieving sales order");

        persisterResult.TryGetValue(out SalesOrder salesOrder);
        salesOrder.AddBeers(rows);

        var updateResult = await Persister.UpdateAsync(salesOrder, cancellationToken);
        return updateResult.Match(
            _ => Result<string>.Success(salesOrderId.Value),
            _ => Result<string>.Error("Error updating sales order"));
    }

    public async Task<Result<string>> ConfirmSalesOrderAsync(SalesOrderId salesOrderId,
        PaymentAuthorizationId paymentAuthorizationId, StockReservationId stockReservationId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var persisterResult = await Persister.GetByIdAsync<SalesOrder>(salesOrderId.Value, cancellationToken);
        if (!persisterResult.IsSuccess)
            return Result<string>.Error("Error retrieving sales order");

        persisterResult.TryGetValue(out SalesOrder salesOrder);
        salesOrder.ConfirmOrder(paymentAuthorizationId, stockReservationId);

        var updateResult = await Persister.UpdateAsync(salesOrder, cancellationToken);
        return updateResult.Match(
            _ => Result<string>.Success(salesOrderId.Value),
            _ => Result<string>.Error("Error updating sales order"));
    }

    public async Task<Result<CustomerTotalPurchased>> GetCustomerTotalPurchasedAsync(CustomerId customerId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Expression<Func<SalesOrder, bool>> query = orders => orders.CustomerId == customerId.Value;
        var queryResult = await orderQueries.GetByFilterAsync(query, 1, 1000, cancellationToken);
        if (queryResult.IsError)
            return Result<CustomerTotalPurchased>.Error("Error retrieving sales orders for customer");
        
        queryResult.TryGetValue(out PagedResult<SalesOrder> pagedResult);

        if (pagedResult.TotalRecords <= 0) 
            return Result<CustomerTotalPurchased>.Error("Error retrieving sales orders");
        
        var totalPurchased =
            pagedResult.Results.Sum(order => order.Rows.Sum(row => row.Quantity.Value * row.Price.Value));
        return Result<CustomerTotalPurchased>.Success(new CustomerTotalPurchased(customerId.Value, pagedResult.Results.First().CustomerName, totalPurchased));
    }

    public async Task<Result<PagedResult<SalesOrderTotalQuantity>>> GetSalesOrderTotalQuantitiesAsync(
        string salesOrderId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Expression<Func<SalesOrder, bool>> query = orders => orders.Id == salesOrderId;
        var queryResult = await orderQueries.GetByFilterAsync(query, 1, 1000, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<SalesOrder> pagedResult);
                
                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<SalesOrderTotalQuantity>>.Success(new PagedResult<SalesOrderTotalQuantity>(
                        pagedResult.Results.Select(r => 
                                new SalesOrderTotalQuantity(r.Id, new Quantity(r.Rows.Sum(row => row.Quantity.Value), "Bottles"))),
                        pagedResult.Page, 
                        pagedResult.PageSize, 
                        pagedResult.TotalRecords))
                    : Result<PagedResult<SalesOrderTotalQuantity>>.Success(new PagedResult<SalesOrderTotalQuantity>([], 0, 0, 0));
            },
            _ => Result<PagedResult<SalesOrderTotalQuantity>>.Error("Error retrieving sales orders"));
    }

    public Task<Result<bool>> ChkAvailabilityForSagaRowsAsync(IEnumerable<ItemRequested> items,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        bool canConfirmSalesOrder = true;
        foreach (var row in items)   
        {
            if (row.QuantityOrdered.Value > row.QuantityAvailable.Value)
                canConfirmSalesOrder = false;
        }
        
        return canConfirmSalesOrder
            ? Task.FromResult(Result<bool>.Success(canConfirmSalesOrder))
            : Task.FromResult(Result<bool>.Error("Error charging sales orders"));
    }
}
