using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using Muflone.Messages;

namespace BrewUp.Sagas.Tests.Orchestrators;

/// <summary>
/// Tests for the confirmation step of the SalesOrderSaga (T017).
/// Verifies that saga events carry a correlation id that can be extracted
/// via MessageHelpers.GetCorrelationId — the foundation of the orchestrator's
/// event-routing logic.
/// </summary>
public class SalesOrderSagaConfirmationTests
{
    [Fact]
    public void PaymentAuthorized_CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        SagaPaymentAuthorized @event = new(
            new IntegrationId(correlationId.ToString()),
            correlationId,
            Guid.CreateVersion7().ToString());

        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(@event));
    }

    [Fact]
    public void PaymentAuthorizationFailed_CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        SagaPaymentAuthorizationFailed @event = new(
            new IntegrationId(correlationId.ToString()),
            correlationId,
            "Payment declined");

        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(@event));
    }

    [Fact]
    public void StockReserved_CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        SagaStockReserved @event = new(
            new IntegrationId(correlationId.ToString()),
            correlationId,
            Guid.CreateVersion7().ToString(),
            []);

        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(@event));
    }

    [Fact]
    public void StockReservationFailed_CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        SagaStockReservationFailed @event = new(
            new IntegrationId(correlationId.ToString()),
            correlationId,
            "Insufficient stock");

        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(@event));
    }

    [Fact]
    public void SalesOrderReadyToConfirm_CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        SagaSalesOrderReadyToConfirm @event = new(
            new IntegrationId(correlationId.ToString()),
            correlationId,
            Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString());

        Assert.Equal(correlationId, MessageHelpers.GetCorrelationId(@event));
    }
}
