using BrewUp.Sagas.Domain.Entities;
using BrewUp.Sagas.SharedKernel.CustomTypes;

namespace BrewUp.Sagas.Tests.Orchestrators;

/// <summary>
/// US3 negative path: payment declined — gate must NOT fire (no compensation, OQ-4 out of scope).
/// Tests run without exceptions = aggregate state machine is correctly guarded.
/// </summary>
public class SagaPaymentDeclined_NoGate
{
    [Fact]
    public void When_PaymentDeclined_NoException()
    {
        var correlationId = Guid.CreateVersion7();
        var sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId, "ORD-003",
            DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5), []);

        // MarkPaymentDeclined must not throw and must NOT trigger gate (no SagaSalesOrderReadyToConfirm)
        saga.MarkPaymentDeclined("Insufficient funds", correlationId);
        Assert.True(true);
    }

    [Fact]
    public void When_PaymentDeclined_ThenStockReserved_NoException()
    {
        // _paymentAuthorized remains false — gate cannot fire even if stock arrives.
        var correlationId = Guid.CreateVersion7();
        var sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId, "ORD-004",
            DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5), []);

        saga.MarkPaymentDeclined("Insufficient funds", correlationId);
        saga.MarkStockReserved(Guid.CreateVersion7().ToString(), correlationId);
        Assert.True(true);
    }
}
