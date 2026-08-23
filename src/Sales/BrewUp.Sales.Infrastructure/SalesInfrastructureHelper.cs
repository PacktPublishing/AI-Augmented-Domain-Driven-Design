using BrewUp.Sales.Infrastructure.MongoDb;
using BrewUp.Shared.Configuration;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Sales.Infrastructure;

public static class SalesInfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        MongoDbSettings mongoDbSettings = new();
        configurationManager.GetSection("BrewUp:MongoDbSettings").Bind(mongoDbSettings);
        services.AddSalesMongoDb(mongoDbSettings);

        services.AddKeyedScoped<IPersister, SalesPersister>("sales");

        return services;
    }
}