using BrewUp.Shared.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Sales.Infrastructure.MongoDb;

public static class MongoDbHelper
{
    public static IServiceCollection AddSalesMongoDb(this IServiceCollection services,
        MongoDbSettings mongoDbSettings)
    {
        return services;
    }
}