using BrewUp.Payment.Domain;
using BrewUp.Payment.Facade.Acl;
using BrewUp.Payment.Infrastructure;
using BrewUp.Payment.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.Facade;

public static class PaymentFacadeHelper
{
    public static IServiceCollection AddPaymentFacade(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPaymentFacade, PaymentFacade>();

        services.AddPaymentDomain();
        services.AddPaymentReadModel();
        services.AddPaymentInfrastructure(configuration);

        services.AddIntegrationEventHandler<SagaRequestsPaymentAuthorizationIntegrationEventHandler>();

        return services;
    }
}
