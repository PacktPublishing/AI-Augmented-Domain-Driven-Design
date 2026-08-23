using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using Lena.Core;

namespace BrewUp.Sales.ReadModel.Services;

public interface ISalesOrderSummaryService
{
    Task<Result<bool>> CreateSalesOrderAsync(SalesOrderId salesOrderId, SalesOrderNumber salesOrderNumber,
        CustomerId customerId, CustomerName customerName, SalesOrderDate orderDate, Price totalAmount,
        SalesOrderStatus status, CancellationToken cancellationToken);

    Task<Result<PagedResult<SalesOrderSummaryJson>>> GetSalesOrdersAsync(int page, int pageSize,
        CancellationToken cancellationToken);
}