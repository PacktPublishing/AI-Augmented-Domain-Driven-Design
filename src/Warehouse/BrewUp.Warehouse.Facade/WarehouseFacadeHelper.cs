using BrewUp.Warehouse.Domain;
using BrewUp.Warehouse.Facade.Acl;
using BrewUp.Warehouse.Facade.EventHandlers;
using BrewUp.Warehouse.Infrastructure;
using BrewUp.Warehouse.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Warehouse.Facade;

public static class WarehouseFacadeHelper
{
    public static IServiceCollection AddWarehouse(this IServiceCollection services)
    {
        services.AddScoped<IWarehouseFacade, WarehouseFacade>();
        
        services.AddInfrastructure();
        services.AddReadModel();
        services.AddDomain();

        services.AddIntegrationEventHandler<WarehouseCreatedEventHandler>();
        services.AddIntegrationEventHandler<SalesOrderCreatedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<BeerCreatedEventHandler>();
        services.AddIntegrationEventHandler<RequestBeerAvailablityRaisedEventHandler>();
        services.AddIntegrationEventHandler<StockReservationRequestedIntegrationEventHandler>();

        services.AddDomainEventHandler<StockReservedIntegrationEventPublisher>();
        services.AddDomainEventHandler<StockReservationFailedIntegrationEventPublisher>();
        
        return services;
    }
}
