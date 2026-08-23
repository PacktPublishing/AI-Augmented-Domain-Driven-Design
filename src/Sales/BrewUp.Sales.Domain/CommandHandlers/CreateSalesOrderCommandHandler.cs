using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Domain.CommandHandlers;

public sealed class CreateSalesOrderCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<CreateSalesOrder>(repository, loggerFactory)
{
    private readonly IRepository _repository = repository;

    public override async Task HandleAsync(CreateSalesOrder command, CancellationToken cancellationToken = new ())
    {
        // Verifica sconto
        // Leggere aggretate Customer
        // Acquisire classe
        
        var aggregate = Entities.SalesOrder.Create(new SalesOrderId(command.AggregateId.Value),
            command.SalesOrderNumber,
            command.SalesOrderDate,
            command.Customer,
            command.SalesOrderDeliveryDate,
            command.Rows,
            command.MessageId);
        
        await _repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken);
    }
}