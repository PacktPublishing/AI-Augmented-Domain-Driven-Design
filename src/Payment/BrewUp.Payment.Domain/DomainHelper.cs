using BrewUp.Payment.Domain.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Payment.Domain;

public static class DomainHelper
{
    public static IServiceCollection AddPaymentDomain(this IServiceCollection services)
    {
        services.AddCommandHandler<RequestPaymentAuthorizationCommandHandler>();
        services.AddCommandHandler<RecordPaymentAuthorizedCommandHandler>();

        return services;
    }
}
