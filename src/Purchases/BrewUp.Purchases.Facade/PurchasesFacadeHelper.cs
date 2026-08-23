using BrewUp.Purchases.Domain;
using BrewUp.Purchases.Facade.Acl;
using BrewUp.Purchases.Infrastructure;
using BrewUp.Purchases.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Purchases.Facade;

public static class PurchasesFacadeHelper
{
    public static IServiceCollection AddPurchases(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddScoped<IPurchasesFacade, PurchasesFacade>();
        
        services.AddDomain();
        services.AddReadModel();
        services.AddInfrastructure(configurationManager);
        
        services.AddIntegrationEventHandler<SupplierCreatedEventHandler>();
        services.AddIntegrationEventHandler<BeerCreatedEventHandler>();
        
        return services;
    }
}