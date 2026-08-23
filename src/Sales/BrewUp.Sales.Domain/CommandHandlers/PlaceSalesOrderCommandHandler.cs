using BrewUp.Sales.Domain.Entities;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Shared.DomainIds;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class PlaceSalesOrderCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<PlaceSalesOrder>(repository, loggerFactory)
{
    public override async Task HandleAsync(PlaceSalesOrder command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var aggregate = SalesOrder.Create(new SalesOrderId(command.MessageId.ToString()),
            new SalesOrderNumber(command.SalesOrder.OrderNumber),
            new SalesOrderDate(command.SalesOrder.OrderDate),
            new Customer(new CustomerId(command.Customer.CustomerId),
                new CustomerName(command.Customer.RagioneSociale),
                CustomerType.FromName(command.Customer.ConsumerLevel.ToLowerInvariant())),
            new SalesOrderDeliveryDate(command.SalesOrder.DeliveryDate),
            command.SalesOrder.Rows, command.MessageId);
        
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken).ConfigureAwait(false);
            
    }
}