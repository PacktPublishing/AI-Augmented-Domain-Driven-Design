using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Lena.Core;
using Muflone.CustomTypes;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain;

internal class SalesDomainService(IServiceBus serviceBus) : ISalesDomainService
{
    public async Task<Result<string>> CreateSalesOrderAsync(CreateSalesOrderJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var salesOrderId = Guid.NewGuid().ToString();
        
        CreateSalesOrder command = new (new SalesOrderId(salesOrderId),
            new SalesOrderNumber(body.OrderNumber),
            new SalesOrderDate(body.OrderDate),
            new Customer(new CustomerId(body.CustomerId), new CustomerName(body.CustomerName), CustomerType.Gold),
            new SalesOrderDeliveryDate(body.DeliveryDate),
            body.Rows, Guid.NewGuid());

        await serviceBus.SendAsync(command, cancellationToken);
        
        return Result<string>.Success(salesOrderId);
    }

    public async Task<Result<bool>> CloseSalesOrderAsync(string orderId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CloseSalesOrder command = new(new SalesOrderId(orderId),
            new SalesOrderDeliveryDate(DateTime.UtcNow),
            new Account(Guid.CreateVersion7().ToString(), "Muflone"),
            Guid.CreateVersion7());
        
        await serviceBus.SendAsync(command, cancellationToken);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> AddBeersToSalesOrderAsync(AddBeersToCartJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        AddBeersToCart command = new(new SalesOrderId(body.OrderId),
            body.Rows);
        
        await serviceBus.SendAsync(command, cancellationToken);
        
        return Result<string>.Success(body.OrderId);
    }
}