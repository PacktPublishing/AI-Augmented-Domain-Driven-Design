using BrewUp.Sagas.Domain.Orchestrators;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Sagas.Domain;

public static class SagasDomainHelper
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ISalesOrderSagaOrchestrator, SalesOrderSagaOrchestrator>();

        services.AddIntegrationEventHandler<SalesOrderSagaOrchestrator>();
        
        return services;
    }
}