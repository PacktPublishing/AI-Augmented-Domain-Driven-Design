using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.ReadModel.Dtos;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrewUp.Warehouse.ReadModel.Services;

internal sealed class ShipmentService([FromKeyedServices("warehouse")] IPersister persister,
    IQueries<Shipment> shipmentQueries,
    ILoggerFactory loggerFactory) 
    : ServiceBase(persister, loggerFactory), IShipmentService
{
    public async Task<Result<bool>> AddShipmentAsync(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId,
        DeliveryDate deliveryDate, IEnumerable<OrderRowDto> rows, ShipmentState shipmentState, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var shipment = Shipment.Create(shipmentId, salesOrderId, customerId, deliveryDate, shipmentState, rows);
        var insertResult = await Persister.InsertAsync(shipment, cancellationToken);
        
        return insertResult.Match(
            _ => Result<bool>.Success(true),
            error =>
            {
                Logger.LogError("Error creating shipment: {Error}", error);
                return Result<bool>.Error($"Error creating shipment: {error}");
            });
    }

    public async Task<Result<PagedResult<ShipmentJson>>> GetShipmentsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult = await shipmentQueries.GetByFilterAsync(null, page, pageSize, cancellationToken);
        
        return queryResult.Match(
            _ =>
            {
                queryResult.TryGetValue(out PagedResult<Shipment> pagedResult);

                return pagedResult.TotalRecords > 0
                    ? Result<PagedResult<ShipmentJson>>.Success(new PagedResult<ShipmentJson>(
                        pagedResult.Results.Select(r => r.ToJson()),
                        pagedResult.Page,
                        pagedResult.PageSize,
                        pagedResult.TotalRecords))
                    : Result<PagedResult<ShipmentJson>>.Success(new PagedResult<ShipmentJson>([], 0, 0, 0));
            },
            _ => Result<PagedResult<ShipmentJson>>.Error("Error retrieving shipment orders"));
    }
}