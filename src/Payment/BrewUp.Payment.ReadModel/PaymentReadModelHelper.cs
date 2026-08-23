using BrewUp.Payment.ReadModel.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.ReadModel;

public static class PaymentReadModelHelper
{
    public static IServiceCollection AddPaymentReadModel(this IServiceCollection services)
    {
        services.AddDomainEventHandler<PaymentAuthorizedEventHandler>();
        services.AddDomainEventHandler<PaymentDeclinedEventHandler>();
        return services;
    }
}
