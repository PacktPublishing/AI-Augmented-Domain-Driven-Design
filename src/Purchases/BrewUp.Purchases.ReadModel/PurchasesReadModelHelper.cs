using BrewUp.Purchases.ReadModel.Dtos;
using BrewUp.Purchases.ReadModel.Queries;
using BrewUp.Purchases.ReadModel.Services;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Purchases.ReadModel;

public static class PurchasesReadModelHelper
{
    public static IServiceCollection AddReadModel(this IServiceCollection services)
    {
        services.AddScoped<IQueries<Supplier>, SuppliersQueries>();
        services.AddScoped<IQueries<Beer>, BeersQueries>();
        
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IBeerService, BeerService>();
        
        return services;
    }
}