using BrewUp.MasterData.Domain.Helpers;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using Lena.Asyncs;
using Lena.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.MasterData.Domain.Services;

internal sealed class WarehouseDomainService([FromKeyedServices("masterdata")] IPersister persister,
    IIntegrationEventPublisher integrationEventPublisher) : IWarehouseDomainService
{
    public async Task<Result<string>> CreateWarehouseAsync(CreateWarehouseJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        WarehouseId warehouseId = new(Guid.CreateVersion7().ToString());
        Warehouse warehouse = Warehouse.Create(warehouseId, new WarehouseName(body.Name));

        return (await (await persister.InsertAsync(warehouse, cancellationToken))
                .BindAsync(_ => integrationEventPublisher.PublishAsync(warehouse.ToWarehouseCreated(), cancellationToken)))
            .Match(
                _ => Result<string>.Success(warehouseId.Value),
                Result<string>.Error);
    }
}