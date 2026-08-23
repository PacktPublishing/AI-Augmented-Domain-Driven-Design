using BrewUp.Dashboards.Domain.CommandHandlers;
using BrewUp.Dashboards.SharedKernel.Messages.Commands;
using Microsoft.Extensions.DependencyInjection;
using Muflone.Messages.Commands;

namespace BrewUp.Dashboards.Domain;

public static class DomainHelper
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IDashboardsDomainService, DashboardsDomainService>();

        services.AddScoped<ICommandHandlerAsync<CreateSummaryByCustomer>, CreateSummaryByCustomersCommandHandler>();
        services.AddScoped<ICommandHandlerAsync<IncreaseSummaryByCustomer>, IncreaseSummaryByCustomersCommandHandler>();

        services.AddScoped<ICommandHandlerAsync<CreateSummaryByProducts>, CreateSummaryByProductsCommandHandler>();
        services.AddScoped<ICommandHandlerAsync<IncreaseSummaryByProducts>, IncreaseSummaryByProductsCommandHandler>();
        
        return services;
    }
}