using System.Globalization;
using BrewUp.Sales.Domain;
using BrewUp.Sales.Facade.Acl;
using BrewUp.Sales.Infrastructure;
using BrewUp.Sales.ReadModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Sales.Facade;

public static class SalesFacadeHelper
{
    public static IServiceCollection AddSalesFacade(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddValidation();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                if (context.ProblemDetails is HttpValidationProblemDetails validationProblemDetails)
                {
                    context.ProblemDetails.Detail =
                        $"Error(s) occurred: {validationProblemDetails.Errors.Values.Sum(x => x.Length)}";
                }

                context.ProblemDetails.Extensions.TryAdd("timestamp",
                    DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
            };
        });
        
        services.AddScoped<ISalesFacade, SalesFacade>();

        services.AddSalesDomain();
        services.AddReadModel();
        services.AddInfrastructure(configurationManager);
        
        services.AddIntegrationEventHandler<CustomerCreatedEventHandler>();
        services.AddIntegrationEventHandler<CustomerUpdatedEventHandler>();
        services.AddIntegrationEventHandler<CustomerDeletedEventHandler>();

        services.AddIntegrationEventHandler<BeerCreatedEventHandler>();
        services.AddIntegrationEventHandler<SagaCustomerBudgetVerifiedSagaCustomerBudgetVerifiedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<SagaSalesOrderAvailabilityCheckedForNotificationIntegrationEventHandler>();
        services.AddIntegrationEventHandler<SagaSalesOrderAvailabilityCheckedIntegrationEventHandler>();

        return services;
    }
}