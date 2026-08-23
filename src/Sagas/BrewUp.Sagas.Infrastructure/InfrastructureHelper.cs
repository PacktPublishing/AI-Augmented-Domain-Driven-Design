using BrewUp.Sagas.Infrastructure.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Sagas.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSignalR();
        
        services.AddSingleton<ISagasHubHelper, SagasHubHelper>();
        
        return services;
    }
}