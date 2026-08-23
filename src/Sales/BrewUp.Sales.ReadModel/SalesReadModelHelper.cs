using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Sales.ReadModel.EventHandlers;
using BrewUp.Sales.ReadModel.Queries;
using BrewUp.Sales.ReadModel.Services;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Sales.ReadModel;

public static class SalesReadModelHelper
{
    public static IServiceCollection AddReadModel(this IServiceCollection services)
    {
        AddReadModelForMcp(services);

        services.AddDomainEventHandler<SalesOrderCreatedEventHandler>();
        // If you want to use Saga, you have to disable this event handler
        services.AddDomainEventHandler<SalesOrderCreatedForIntegrationEventHandler>();
        //services.AddDomainEventHandler<SalesOrderCreatedWithPriceForIntegrationEventHandler>();
        services.AddDomainEventHandler<SalesOrderCreatedForSalesOrderPlacedIntegrationEventHandler>();
        services.AddDomainEventHandler<SalesOrderCreatedForSalesSummaryEventHandler>();
        services.AddDomainEventHandler<SalesOrderCreatedForCustomerSalesEventHandler>();
        services.AddDomainEventHandler<SalesOrderAcceptedEventHandler>();
        services.AddDomainEventHandler<SalesOrderConfirmedEventHandler>();

        services.AddDomainEventHandler<BeersAddedToCartEventHandler>();
        
        return services;
    }
    
    public static IServiceCollection AddReadModelForMcp(this IServiceCollection services)
    {
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBeerService, BeerService>();
        services.AddScoped<ISalesOrderSummaryService, SalesOrderSummaryService>();
        services.AddScoped<ISalesByCustomerService, SalesByCustomerService>();
        
        services.AddScoped<IQueries<SalesOrder>, SalesOrderQueries>();
        services.AddScoped<IQueries<SalesOrderSummary>, SalesOrderSummaryQueries>();
        services.AddScoped<IQueries<Beer>, BeersQueries>();
        
        return services;
    }
}
