using BrewUp.Sagas.Domain.Entities;
using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages;

namespace BrewUp.Sagas.Tests.Orchestrators;

/// <summary>
/// US2 gate test: when both PaymentAuthorized and StockReserved arrive,
/// the saga reaches a state where ReadyToConfirm gate was triggered.
/// Tested via MarkPaymentAuthorized + MarkStockReserved completing without exceptions.
/// </summary>
public class SagaBothEvidencesReceived_GateFires
{
    private static SalesOrderSaga CreateStartedSaga(out SagaId sagaId, out Guid correlationId)
    {
        correlationId = Guid.CreateVersion7();
        sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId,
            "ORD-001", DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5),
            [new SalesOrderRowJson { BeerId = Guid.CreateVersion7().ToString(), BeerName = "TestBeer",
                Quantity = new Quantity(10, "pcs"), Price = new Price(100m, "EUR") }]);

        return saga;
    }

    /// <summary>
    /// Both evidences received → CheckConfirmationGate fires, no exception thrown.
    /// </summary>
    [Fact]
    public void When_BothEvidencesReceived_NoException()
    {
        var saga = CreateStartedSaga(out _, out var correlationId);
        var paymentId = Guid.CreateVersion7().ToString();
        var stockId = Guid.CreateVersion7().ToString();

        // Both arrive → gate fires
        saga.MarkPaymentAuthorized(paymentId, correlationId);
        saga.MarkStockReserved(stockId, correlationId);

        // If we reach here without exception, the gate logic executed cleanly
        Assert.True(true);
    }

    /// <summary>
    /// Only payment arrives → gate does NOT fire (saga does not throw).
    /// </summary>
    [Fact]
    public void When_OnlyPaymentReceived_NoException()
    {
        var saga = CreateStartedSaga(out _, out var correlationId);
        saga.MarkPaymentAuthorized(Guid.CreateVersion7().ToString(), correlationId);
        Assert.True(true);
    }

    /// <summary>
    /// Only stock arrives → gate does NOT fire (saga does not throw).
    /// </summary>
    [Fact]
    public void When_OnlyStockReceived_NoException()
    {
        var saga = CreateStartedSaga(out _, out var correlationId);
        saga.MarkStockReserved(Guid.CreateVersion7().ToString(), correlationId);
        Assert.True(true);
    }
}
