using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Payment.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // EventStore persister and MongoDB collection wiring for Payment module
        // Relies on global infrastructure registered in BrewUp.Infrastructure
        return services;
    }
}
