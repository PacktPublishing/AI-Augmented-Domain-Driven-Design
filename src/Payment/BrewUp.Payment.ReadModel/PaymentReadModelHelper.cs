using BrewUp.Payment.ReadModel.Dtos;
using BrewUp.Payment.ReadModel.EventHandlers;
using BrewUp.Payment.ReadModel.Queries;
using BrewUp.Payment.ReadModel.Services;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.ReadModel;

public static class PaymentReadModelHelper
{
    public static IServiceCollection AddPaymentReadModel(this IServiceCollection services)
    {
        services.AddScoped<IPaymentAuthorizationService, PaymentAuthorizationService>();
        services.AddScoped<IQueries<PaymentAuthorization>, PaymentAuthorizationQueries>();
        services.AddDomainEventHandler<PaymentAuthorizationRequestedEventHandler>();
        services.AddDomainEventHandler<PaymentAuthorizedEventHandler>();

        return services;
    }
}
