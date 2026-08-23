using BrewUp.Shared.Configuration;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Purchases.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        MongoDbSettings mongoDbSettings = new();
        configurationManager.GetSection("BrewUp:MongoDbSettings").Bind(mongoDbSettings);

        services.AddKeyedScoped<IPersister, PurchasesPersister>("purchases");
        
        return services;
    }
}