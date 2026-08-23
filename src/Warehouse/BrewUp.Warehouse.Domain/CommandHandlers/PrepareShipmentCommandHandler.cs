using BrewUp.Warehouse.Domain.Entities;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Warehouse.Domain.CommandHandlers;

public sealed class PrepareShipmentCommandHandler(IRepository repository,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<PrepareShipment>(repository, loggerFactory)
{
    public override async Task HandleAsync(PrepareShipment command, CancellationToken cancellationToken = new ())
    {
        cancellationToken.ThrowIfCancellationRequested();

        var aggregate = Shipment.Create(new ShipmentId(command.AggregateId.Value), command.SalesOrderId,
            command.CustomerId, command.DeliveryDate, command.Rows, command.MessageId);
        
        await Repository.SaveAsync(aggregate, Guid.CreateVersion7(), cancellationToken);
    }
}