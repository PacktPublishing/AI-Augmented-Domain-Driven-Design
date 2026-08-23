using System.Reflection;
using BrewUp.Payment.Domain.CommandHandlers;
using BrewUp.Payment.Facade;
using BrewUp.Payment.Facade.EventHandlers;
using BrewUp.Payment.Infrastructure;
using BrewUp.Payment.ReadModel.Dtos;
using BrewUp.Payment.ReadModel.EventHandlers;
using BrewUp.Payment.ReadModel.Queries;
using BrewUp.Payment.ReadModel.Services;
using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.ReadModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Muflone;
using Muflone.Core;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using PaymentProjection = BrewUp.Payment.ReadModel.Dtos.PaymentAuthorization;

namespace BrewUp.Payment.Tests.Infrastructure;

public sealed class PaymentPersistenceWiring
{
    [Fact]
    public async Task MissingProjectionReplacementReturnsPersistenceError()
    {
        var persister = new PaymentPersister(
            CreateMongoClient(matchedCount: 0),
            NullLoggerFactory.Instance);
        var projection = PaymentProjection.CreatePending(
            new PaymentAuthorizationId(Guid.CreateVersion7().ToString()),
            new SalesOrderReference(Guid.CreateVersion7().ToString()));

        var result = await persister.UpdateAsync(
            projection,
            CancellationToken.None);

        Assert.True(result.IsError);
    }

    [Fact]
    public void PaymentCompositionBuildsAndResolvesOwnedScopedBehavior()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(CreateMongoClient(matchedCount: 1));
        services.AddSingleton<IRepository>(new NoOpRepository());
        services.AddSingleton<IServiceBus>(new NoOpServiceBus());
        services.AddSingleton<IEventBus>(new NoOpEventBus());
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);

        services.AddPaymentFacade(new ConfigurationManager());

        AssertScopedRegistration<IPersister, PaymentPersister>(
            services,
            serviceKey: "payment");
        AssertScopedRegistration<IPaymentAuthorizationService, PaymentAuthorizationService>(
            services);
        AssertScopedRegistration<IQueries<PaymentAuthorization>, PaymentAuthorizationQueries>(
            services);
        AssertScopedImplementation<RequestPaymentAuthorizationCommandHandler>(services);
        AssertScopedImplementation<RecordPaymentAuthorizedCommandHandler>(services);
        AssertScopedImplementation<PaymentAuthorizationRequestedEventHandler>(services);
        AssertScopedImplementation<PaymentAuthorizedEventHandler>(services);
        AssertScopedRegistration<IPaymentFacade, PaymentFacade>(services);
        AssertScopedImplementation<PaymentAuthorizedIntegrationEventPublisher>(services);

        using var provider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        Assert.IsType<PaymentPersister>(
            scopedProvider.GetRequiredKeyedService<IPersister>("payment"));
        Assert.IsType<PaymentAuthorizationService>(
            scopedProvider.GetRequiredService<IPaymentAuthorizationService>());
        Assert.IsType<PaymentAuthorizationQueries>(
            scopedProvider.GetRequiredService<IQueries<PaymentAuthorization>>());
        Assert.IsType<PaymentFacade>(
            scopedProvider.GetRequiredService<IPaymentFacade>());
        AssertImplementationResolves<RequestPaymentAuthorizationCommandHandler>(
            scopedProvider,
            services);
        AssertImplementationResolves<RecordPaymentAuthorizedCommandHandler>(
            scopedProvider,
            services);
        AssertImplementationResolves<PaymentAuthorizationRequestedEventHandler>(
            scopedProvider,
            services);
        AssertImplementationResolves<PaymentAuthorizedEventHandler>(
            scopedProvider,
            services);
        AssertImplementationResolves<PaymentAuthorizedIntegrationEventPublisher>(
            scopedProvider,
            services);
    }

    private static void AssertScopedRegistration<TService, TImplementation>(
        IServiceCollection services,
        object? serviceKey = null)
    {
        var descriptor = Assert.Single(
            services,
            candidate =>
                candidate.ServiceType == typeof(TService) &&
                (candidate.IsKeyedService
                    ? candidate.KeyedImplementationType
                    : candidate.ImplementationType) == typeof(TImplementation) &&
                Equals(candidate.ServiceKey, serviceKey));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    private static void AssertScopedImplementation<TImplementation>(
        IServiceCollection services)
    {
        var descriptor = Assert.Single(
            services,
            candidate =>
                candidate.ImplementationType == typeof(TImplementation));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    private static void AssertImplementationResolves<TImplementation>(
        IServiceProvider provider,
        IServiceCollection services)
    {
        var descriptor = Assert.Single(
            services,
            candidate =>
                candidate.ImplementationType == typeof(TImplementation));
        Assert.Single(
            provider.GetServices(descriptor.ServiceType),
            instance => instance?.GetType() == typeof(TImplementation));
    }

    private static IMongoClient CreateMongoClient(long matchedCount)
    {
        var client = DispatchProxy.Create<IMongoClient, MongoClientProxy>();
        ((MongoClientProxy)(object)client).MatchedCount = matchedCount;
        return client;
    }

    private class MongoClientProxy : DispatchProxy
    {
        public long MatchedCount { get; set; }

        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args)
        {
            if (targetMethod?.Name == nameof(IMongoClient.GetDatabase))
            {
                var database = DispatchProxy.Create<IMongoDatabase, MongoDatabaseProxy>();
                ((MongoDatabaseProxy)(object)database).MatchedCount = MatchedCount;
                return database;
            }

            if (targetMethod?.Name == nameof(IDisposable.Dispose))
                return null;

            throw new NotSupportedException(targetMethod?.Name);
        }
    }

    private class MongoDatabaseProxy : DispatchProxy
    {
        public long MatchedCount { get; set; }

        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args)
        {
            if (targetMethod?.Name == nameof(IMongoDatabase.GetCollection))
            {
                var documentType = targetMethod.GetGenericArguments()[0];
                return typeof(MongoDatabaseProxy)
                    .GetMethod(
                        nameof(CreateCollection),
                        BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(documentType)
                    .Invoke(null, [MatchedCount]);
            }

            throw new NotSupportedException(targetMethod?.Name);
        }

        private static IMongoCollection<T> CreateCollection<T>(long matchedCount)
        {
            var collection =
                DispatchProxy.Create<IMongoCollection<T>, MongoCollectionProxy<T>>();
            ((MongoCollectionProxy<T>)(object)collection).MatchedCount = matchedCount;
            return collection;
        }
    }

    private class MongoCollectionProxy<T> : DispatchProxy
    {
        public long MatchedCount { get; set; }

        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args)
        {
            if (targetMethod?.Name == nameof(IMongoCollection<T>.ReplaceOneAsync))
            {
                ReplaceOneResult result = new ReplaceOneResult.Acknowledged(
                    MatchedCount,
                    modifiedCount: 0,
                    upsertedId: null);
                return Task.FromResult(result);
            }

            throw new NotSupportedException(targetMethod?.Name);
        }
    }

    private sealed class NoOpRepository : IRepository
    {
        public Task<TAggregate?> GetByIdAsync<TAggregate>(
            IDomainId id,
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregate =>
            Task.FromResult<TAggregate?>(null);

        public Task<TAggregate?> GetByIdAsync<TAggregate>(
            IDomainId id,
            long version,
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregate =>
            Task.FromResult<TAggregate?>(null);

        public Task SaveAsync(
            IAggregate aggregate,
            Guid commitId,
            Action<IDictionary<string, object>> updateHeaders,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task SaveAsync(
            IAggregate aggregate,
            Guid commitId,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    private sealed class NoOpServiceBus : IServiceBus
    {
        public Task SendAsync<T>(
            T command,
            CancellationToken cancellationToken)
            where T : class, ICommand =>
            Task.CompletedTask;
    }

    private sealed class NoOpEventBus : IEventBus
    {
        public Task PublishAsync<T>(
            T @event,
            CancellationToken cancellationToken)
            where T : class, IEvent =>
            Task.CompletedTask;
    }
}
