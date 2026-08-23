using BrewUp.Shared.ReadModel;
using BrewUp.Warehouse.Entities.Dtos;
using BrewUp.Warehouse.ReadModel.Dtos;
using BrewUp.Warehouse.ReadModel.EventHandlers;
using BrewUp.Warehouse.ReadModel.Queries;
using BrewUp.Warehouse.ReadModel.Services;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Warehouse.ReadModel;

public static class ReadModelHelper
{
    public static IServiceCollection AddReadModel(this IServiceCollection services)
    {
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IShipmentService, ShipmentService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IBeerService, BeerService>();

        services.AddScoped<IQueries<Shipment>, ShipmentQueries>();
        services.AddScoped<IQueries<Availability>, AvailabilityQueries>();
        services.AddScoped<IQueries<Dtos.Warehouse>, WarehouseQueries>();
        services.AddScoped<IQueries<Beer>, BeerQueries>();

        services.AddDomainEventHandler<ShipmentPendingForPreparationEventHandler>();
        
        return services;
    }

    public static IServiceCollection AddReadModelForMcp(this IServiceCollection services)
    {
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IShipmentService, ShipmentService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IBeerService, BeerService>();

        services.AddScoped<IQueries<Shipment>, ShipmentQueries>();
        services.AddScoped<IQueries<Availability>, AvailabilityQueries>();
        services.AddScoped<IQueries<Dtos.Warehouse>, WarehouseQueries>();
        services.AddScoped<IQueries<Beer>, BeerQueries>();
        
        return services;
    }
}