using BrewUp.MasterData.Entities.Dtos;
using BrewUp.MasterData.ReadModel.Queries;
using BrewUp.MasterData.ReadModel.Services;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.MasterData.ReadModel;

public static class ReadModelHelper
{
    public static IServiceCollection AddMasterDataReadModel(this IServiceCollection services)
    {
        services.AddScoped<IQueries<Beer>, BeerQueries>();
        services.AddScoped<IQueries<Customer>, CustomerQueries>();
        services.AddScoped<IQueries<Supplier>, SupplierQueries>();
        services.AddScoped<IQueries<Warehouse>, WarehouseQueries>();
        
        services.AddScoped<IBeerQueryService, BeerQueryService>();
        services.AddScoped<ICustomerQueryService, CustomerQueryService>();
        services.AddScoped<ISupplierQueryService, SupplierQueryService>();
        services.AddScoped<IWarehouseQueryService, WarehouseQueryService>();
        
        return services;
    }
}