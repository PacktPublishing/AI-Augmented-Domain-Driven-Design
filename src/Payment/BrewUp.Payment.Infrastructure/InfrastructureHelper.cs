using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Payment.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services)
    {
        services.AddKeyedScoped<IPersister, PaymentPersister>("payment");

        return services;
    }
}
