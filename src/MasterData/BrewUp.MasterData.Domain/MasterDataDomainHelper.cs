using BrewUp.MasterData.Domain.CommandHandlers;
using BrewUp.MasterData.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.MasterData.Domain;

public static class MasterDataDomainHelper
{
    public static IServiceCollection AddMasterDataDomain(this IServiceCollection services)
    {
        services.AddScoped<ICustomerDomainService, CustomerDomainService>();
        services.AddScoped<ISupplierDomainService, SupplierDomainService>();
        services.AddScoped<IBeerDomainService, BeerDomainService>();
        services.AddScoped<IWarehouseDomainService, WarehouseDomainService>();
        
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services.AddCommandHandler<VerifyCustomerBudgetCommandHandler>();
        
        return services;
    }
}