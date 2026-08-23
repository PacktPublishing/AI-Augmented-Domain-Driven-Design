using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.SharedKernel.CustomTypes;
using BrewUp.Warehouse.SharedKernel.Enums;
using Lena.Core;

namespace BrewUp.Warehouse.ReadModel.Services;

public interface IShipmentService
{
    Task<Result<bool>> AddShipmentAsync(ShipmentId shipmentId, SalesOrderId salesOrderId, CustomerId customerId,
        DeliveryDate deliveryDate, IEnumerable<OrderRowDto> rows, ShipmentState shipmentState,
        CancellationToken cancellationToken = default);
    
    Task<Result<PagedResult<ShipmentJson>>> GetShipmentsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}