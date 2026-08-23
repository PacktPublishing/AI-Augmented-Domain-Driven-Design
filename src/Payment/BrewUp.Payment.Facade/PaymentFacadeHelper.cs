using BrewUp.Payment.Domain;
using BrewUp.Payment.Facade.EventHandlers;
using BrewUp.Payment.Infrastructure;
using BrewUp.Payment.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.Facade;

public static class PaymentFacadeHelper
{
    public static IServiceCollection AddPaymentFacade(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        services.AddPaymentInfrastructure();
        services.AddPaymentReadModel();
        services.AddPaymentDomain();
        services.AddScoped<IPaymentFacade, PaymentFacade>();
        services.AddDomainEventHandler<PaymentAuthorizedIntegrationEventPublisher>();

        return services;
    }
}
