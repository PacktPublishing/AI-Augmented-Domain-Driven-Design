using BrewUp.Infrastructure.MongoDb;
using BrewUp.Infrastructure.RabbitMq;
using BrewUp.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Muflone.Eventstore.gRPC;
using Muflone.Transport.RabbitMQ;
using Muflone.Transport.RabbitMQ.Models;

namespace BrewUp.Infrastructure;

public static class InfrastructureHelper
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ILoggerFactory loggerFactory,
        IConfigurationManager configurationManager)
    {
        MongoDbSettings mongoDbSettings = new();
        configurationManager.GetSection("BrewUp:MongoDbSettings").Bind(mongoDbSettings);
        services.AddMongoDb(mongoDbSettings);
        
        RabbitMqSettings rabbitMqSettings = new();
        configurationManager.GetSection("BrewUp:RabbitMQ").Bind(rabbitMqSettings);
        
        RabbitMQConfiguration rabbitMqConfiguration = new(rabbitMqSettings.Host,
            rabbitMqSettings.Username,
            rabbitMqSettings.Password,
            rabbitMqSettings.ExchangeCommandName,
            rabbitMqSettings.ExchangeEventName,
            rabbitMqSettings.ClientId);
        services.AddMufloneTransportRabbitMQ(loggerFactory, rabbitMqConfiguration);
        
        EventStoreSettings eventStoreSettings = new();
        configurationManager.GetSection("BrewUp:EventStore").Bind(eventStoreSettings);
        services.AddMufloneEventStore(eventStoreSettings.ConnectionString);


        return services;
    }
}