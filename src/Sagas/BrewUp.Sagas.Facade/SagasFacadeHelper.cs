using BrewUp.Sagas.Domain;
using BrewUp.Sagas.Infrastructure;
using BrewUp.Sagas.ReadModel;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Sagas.Facade;

public static class SagasFacadeHelper
{
    public static IServiceCollection AddSagas(this IServiceCollection services)
    {
        services.AddScoped<ISagasFacade, SagasFacade>();

        services.AddDomain();
        services.AddReadModel();
        services.AddInfrastructure();
        
        return services;
    }
}