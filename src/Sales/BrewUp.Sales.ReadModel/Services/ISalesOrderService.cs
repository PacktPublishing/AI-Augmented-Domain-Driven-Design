using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Sales.ReadModel.Services;

public interface ISalesOrderService
{
    Task<Result<bool>> CreateSalesOrderAsync(SalesOrderId salesOrderId, SalesOrderNumber salesOrderNumber, CustomerId customerId,
        CustomerName customerName, SalesOrderDate orderDate, IEnumerable<SalesOrderRowJson> rows, CancellationToken cancellationToken);
    
    Task<Result<PagedResult<SalesOrderJson>>> GetSalesOrdersAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Result<SalesOrderJson>> GetSalesOrderByIdAsync(string salesOrderId, CancellationToken cancellationToken);

    Task<Result<string>> AddBeersToSalesOrderAsync(SalesOrderId salesOrderId, IEnumerable<SalesOrderRowJson> rows,
        CancellationToken cancellationToken);

    Task<Result<string>> ConfirmSalesOrderAsync(SalesOrderId salesOrderId,
        PaymentAuthorizationId paymentAuthorizationId, StockReservationId stockReservationId,
        CancellationToken cancellationToken);
    
    Task<Result<CustomerTotalPurchased>> GetCustomerTotalPurchasedAsync(CustomerId customerId, CancellationToken cancellationToken);

    Task<Result<PagedResult<SalesOrderTotalQuantity>>> GetSalesOrderTotalQuantitiesAsync(string salesOrderId, CancellationToken cancellationToken);
    
    Task<Result<bool>> ChkAvailabilityForSagaRowsAsync(IEnumerable<ItemRequested> items, CancellationToken cancellationToken);
}
