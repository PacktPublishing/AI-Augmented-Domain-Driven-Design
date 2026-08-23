using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Purchases.Domain;

public static class DomainHelper
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseDomainService, PurchaseDomainService>();
        
        return services; 
    }
}