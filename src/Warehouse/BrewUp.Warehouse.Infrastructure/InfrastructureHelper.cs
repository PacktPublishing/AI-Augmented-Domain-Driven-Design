using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Warehouse.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddKeyedScoped<IPersister, WarehousePersister>("warehouse");
        
        return services;
    }
        
}