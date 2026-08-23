using BrewUp.Payment.Domain.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.Domain;

public static class PaymentDomainHelper
{
    public static IServiceCollection AddPaymentDomain(this IServiceCollection services)
    {
        services.AddCommandHandler<AuthorizePaymentCommandHandler>();
        return services;
    }
}
