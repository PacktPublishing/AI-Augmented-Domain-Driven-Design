using BrewUp.Warehouse.Domain.CommandHandlers;
using BrewUp.Warehouse.SharedKernel.Messages.Commands;
using Microsoft.Extensions.DependencyInjection;
using Muflone;
using Muflone.Messages.Commands;

namespace BrewUp.Warehouse.Domain;

public static class DomainHelper
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddCommandHandler<PrepareShipmentCommandHandler>();
        services.AddCommandHandler<AddItemStockCommandHandlerAsync>();
        services.AddCommandHandler<CreateAvailabilityCommandHandler>();

        return services;
    }
}