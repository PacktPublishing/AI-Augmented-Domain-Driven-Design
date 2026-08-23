using System.Collections;
using System.Linq.Expressions;
using BrewUp.Payment.Domain.CommandHandlers;
using BrewUp.Payment.Facade;
using BrewUp.Payment.Facade.EventHandlers;
using BrewUp.Payment.ReadModel.EventHandlers;
using BrewUp.Payment.ReadModel.Services;
using BrewUp.Payment.SharedKernel.CustomTypes;
using BrewUp.Payment.SharedKernel.DomainIds;
using BrewUp.Payment.SharedKernel.Messages.Commands;
using BrewUp.Payment.SharedKernel.Messages.Events;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone;
using Muflone.Messages;
using Muflone.Messages.Events;
using Muflone.Persistence;
using DomainPaymentAuthorization = BrewUp.Payment.Domain.Entities.PaymentAuthorization;
using PaymentProjection = BrewUp.Payment.ReadModel.Dtos.PaymentAuthorization;

namespace BrewUp.Payment.Tests.ReadModel;

public sealed class ProjectAndPublishAuthorizedOutcome
{
    [Fact]
    public async Task ProjectAndPublishAuthorizedOutcomeOnceAfterDuplicateCallback()
    {
        var authorizationId = new PaymentAuthorizationId(Guid.CreateVersion7().ToString());
        var salesOrder = new SalesOrderReference(Guid.CreateVersion7().ToString());
        var requestCorrelationId = Guid.CreateVersion7();
        var persister = new InMemoryPersister();
        var projectionService = new PaymentAuthorizationService(persister);
        var requestedHandler = new PaymentAuthorizationRequestedEventHandler(
            projectionService,
            NullLoggerFactory.Instance);
        var authorizedHandler = new PaymentAuthorizedEventHandler(
            projectionService,
            NullLoggerFactory.Instance);
        var eventBus = new CapturingEventBus();
        var publisher = new PaymentAuthorizedIntegrationEventPublisher(
            eventBus,
            NullLoggerFactory.Instance);

        await requestedHandler.HandleAsync(
            new PaymentAuthorizationRequested(
                authorizationId,
                salesOrder,
                requestCorrelationId),
            CancellationToken.None);

        var repository = new EventDispatchingRepository(
            authorizationId,
            salesOrder,
            requestCorrelationId,
            authorizedHandler,
            publisher);
        var commandHandler = new RecordPaymentAuthorizedCommandHandler(
            repository,
            NullLoggerFactory.Instance);
        var facade = new PaymentFacade(new HandlingServiceBus(commandHandler));

        await facade.RecordAuthorizedAsync(
            authorizationId,
            "provider-auth-123",
            CancellationToken.None);
        await facade.RecordAuthorizedAsync(
            authorizationId,
            "provider-auth-123",
            CancellationToken.None);

        Assert.NotNull(persister.Stored);
        Assert.Equal(authorizationId.Value, persister.Stored.Id);
        Assert.Equal(salesOrder.Value, persister.Stored.SalesOrderId);
        Assert.Equal("authorized", persister.Stored.Status);
        Assert.Equal("provider-auth-123", persister.Stored.ProviderReference);
        Assert.Equal(1, persister.UpdateCount);

        var integrationEvent = Assert.Single(eventBus.Published);
        Assert.Equal(authorizationId, integrationEvent.AggregateId);
        Assert.Equal("provider-auth-123", integrationEvent.ProviderReference);
        Assert.Equal(
            repository.AuthorizedCorrelationId,
            MessageHelpers.GetCorrelationId(integrationEvent));
    }

    [Fact]
    public async Task FailedAuthorizedProjectionIsPropagatedAsPersistenceFailure()
    {
        var authorizationId = new PaymentAuthorizationId(Guid.CreateVersion7().ToString());
        var persister = new InMemoryPersister
        {
            UpdateResult = Result<bool>.Error("update failed")
        };
        var service = new PaymentAuthorizationService(persister);
        await service.CreatePendingAsync(
            authorizationId,
            new SalesOrderReference(Guid.CreateVersion7().ToString()),
            CancellationToken.None);
        var handler = new PaymentAuthorizedEventHandler(
            service,
            NullLoggerFactory.Instance);

        var exception = await Assert.ThrowsAsync<PersistenceException>(() =>
            handler.HandleAsync(
                new PaymentAuthorized(
                    authorizationId,
                    "provider-auth-123",
                    Guid.CreateVersion7()),
                CancellationToken.None));

        Assert.Contains("authorized payment projection", exception.Message);
    }

    private sealed class InMemoryPersister : IPersister
    {
        public PaymentProjection? Stored { get; private set; }
        public int UpdateCount { get; private set; }
        public Result<bool> UpdateResult { get; init; } = Result<bool>.Success(true);

        public Task<Result<T>> GetByIdAsync<T>(string id, CancellationToken cancellationToken)
            where T : DtoBase =>
            Stored is T result && result.Id == id
                ? Task.FromResult(Result<T>.Success(result))
                : Task.FromResult(Result<T>.Error("Projection not found."));

        public Task<Result<bool>> InsertAsync<T>(T entity, CancellationToken cancellationToken)
            where T : DtoBase
        {
            Stored = Assert.IsType<PaymentProjection>(entity);
            return Task.FromResult(Result<bool>.Success(true));
        }

        public Task<Result<bool>> UpdateAsync<T>(T entity, CancellationToken cancellationToken)
            where T : DtoBase
        {
            Stored = Assert.IsType<PaymentProjection>(entity);
            UpdateCount++;
            return Task.FromResult(UpdateResult);
        }

        public Task<Result<bool>> DeleteAsync<T>(T entity, CancellationToken cancellationToken)
            where T : DtoBase =>
            Task.FromResult(Result<bool>.Success(true));
    }

    private sealed class CapturingEventBus : IEventBus
    {
        public List<PaymentAuthorizedIntegrationEvent> Published { get; } = [];

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
            where T : class, IEvent
        {
            Published.Add(Assert.IsType<PaymentAuthorizedIntegrationEvent>(@event));
            return Task.CompletedTask;
        }
    }

    private sealed class HandlingServiceBus(
        RecordPaymentAuthorizedCommandHandler commandHandler) : IServiceBus
    {
        public Task SendAsync<T>(T command, CancellationToken cancellationToken)
            where T : class, Muflone.Messages.Commands.ICommand =>
            commandHandler.HandleAsync(
                Assert.IsType<RecordPaymentAuthorized>(command),
                cancellationToken);
    }

    private sealed class EventDispatchingRepository : IRepository
    {
        private readonly DomainPaymentAuthorization _aggregate;
        private readonly PaymentAuthorizedEventHandler _projectionHandler;
        private readonly PaymentAuthorizedIntegrationEventPublisher _publisher;

        public EventDispatchingRepository(
            PaymentAuthorizationId authorizationId,
            SalesOrderReference salesOrder,
            Guid correlationId,
            PaymentAuthorizedEventHandler projectionHandler,
            PaymentAuthorizedIntegrationEventPublisher publisher)
        {
            _projectionHandler = projectionHandler;
            _publisher = publisher;
            _aggregate = (DomainPaymentAuthorization)Activator.CreateInstance(
                typeof(DomainPaymentAuthorization),
                nonPublic: true)!;
            ((IAggregate)_aggregate).ApplyEvent(
                new PaymentAuthorizationRequested(
                    authorizationId,
                    salesOrder,
                    correlationId));
        }

        public Guid AuthorizedCorrelationId { get; private set; }

        public Task<TAggregate?> GetByIdAsync<TAggregate>(
            Muflone.Core.IDomainId id,
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregate =>
            Task.FromResult(_aggregate as TAggregate);

        public Task<TAggregate?> GetByIdAsync<TAggregate>(
            Muflone.Core.IDomainId id,
            long version,
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregate =>
            Task.FromResult(_aggregate as TAggregate);

        public Task SaveAsync(
            IAggregate aggregate,
            Guid commitId,
            Action<IDictionary<string, object>> updateHeaders,
            CancellationToken cancellationToken) =>
            SaveAsync(aggregate, commitId, cancellationToken);

        public async Task SaveAsync(
            IAggregate aggregate,
            Guid commitId,
            CancellationToken cancellationToken)
        {
            foreach (var @event in aggregate.GetUncommittedEvents()
                         .Cast<object>()
                         .OfType<PaymentAuthorized>())
            {
                AuthorizedCorrelationId = MessageHelpers.GetCorrelationId(@event);
                await _projectionHandler.HandleAsync(@event, cancellationToken);
                await _publisher.HandleAsync(@event, cancellationToken);
            }

            aggregate.ClearUncommittedEvents();
        }

        public void Dispose()
        {
        }
    }
}
