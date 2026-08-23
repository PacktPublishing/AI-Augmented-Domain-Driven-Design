using BrewUp.Sagas.ReadModel.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Muflone;

namespace BrewUp.Sagas.ReadModel;

public static class SagaReadModelHelper
{
    public static IServiceCollection AddReadModel(this IServiceCollection services)
    {
        services.AddDomainEventHandler<SalesOrderSagaStartedEventHandler>();
        services.AddDomainEventHandler<SalesOrderSagaRejectedEventHandler>();
        services.AddDomainEventHandler<SagaCustomerBudgetVerifiedEventHandler>();
        services.AddDomainEventHandler<SagaSalesOrderAvailablityCheckedEventHandler>();
        services.AddDomainEventHandler<SagaSalesOrderPlacedEventHandler>();
        services.AddDomainEventHandler<SagaSalesOrderSuccessfullyCompletedEventHandler>();
        services.AddDomainEventHandler<SagaSalesOrderReadyToConfirmEventHandler>();
        
        return services;
    }
}