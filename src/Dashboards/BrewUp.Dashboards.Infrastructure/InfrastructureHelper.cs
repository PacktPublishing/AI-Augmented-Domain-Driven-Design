using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.Infrastructure.Hubs;
using BrewUp.Dashboards.Infrastructure.Repository;
using BrewUp.Dashboards.SharedKernel.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUp.Dashboards.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        // services.AddDbContext<DashboardsContext>(options =>
        //     options.UseSqlServer(configurationManager["BrewUp:SqlServer:ConnectionString"]!));

        services.AddSignalR();
        
        services.AddScoped<IDashboardsRepository<SalesByCustomers>, SummaryByCustomersRepository>();
        services.AddScoped<IDashboardsRepository<SalesByProducts>, SummaryByProductsRepository>();
        services.AddScoped<IDashboardsRepository<MessagesReceived>, MessagesReceivedRepository>();
        
        services.AddScoped<IMessagesReceivedService, MessagesReceivedService>();

        services.AddSingleton<IDashboardsHubHelper, DashboardsHubHelper>();
        
        return services;
    }
}