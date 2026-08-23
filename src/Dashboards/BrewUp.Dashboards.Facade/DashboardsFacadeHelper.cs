using BrewUp.Dashboards.Domain;
using BrewUp.Dashboards.Facade.Acl;
using BrewUp.Dashboards.Infrastructure;
using BrewUp.Dashboards.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Dashboards.Facade;

public static class DashboardsFacadeHelper
{
    public static IServiceCollection AddDashboards(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddReadModel(configurationManager);
        services.AddInfrastructure(configurationManager);
        services.AddDomain();
        
        services.AddScoped<IDashboardsFacade, DashboardsFacade>();
        
        services.AddIntegrationEventHandler<SalesOrderCreatedIntegrationForBeerSummaryEventHandler>();
        services.AddIntegrationEventHandler<SalesOrderCreatedIntegrationForCustomerSummaryEventHandler>();
        
        return services;
    }
}