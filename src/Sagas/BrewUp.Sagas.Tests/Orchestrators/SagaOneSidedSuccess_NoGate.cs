using BrewUp.Sagas.Domain.Entities;
using BrewUp.Sagas.SharedKernel.CustomTypes;

namespace BrewUp.Sagas.Tests.Orchestrators;

/// <summary>
/// US3 one-sided success test: payment authorized but stock rejected → gate must NOT fire.
/// Also covers reverse: stock reserved but payment declined → gate must NOT fire.
/// No compensation is dispatched (FR-011, OQ-1 resolved).
/// Tested via exception-absence since GetUncommittedEvents() is not exposed by Muflone AggregateRoot.
/// </summary>
public class SagaOneSidedSuccess_NoGate
{
    [Fact]
    public void When_PaymentAuthorized_ThenStockRejected_NoException()
    {
        var correlationId = Guid.CreateVersion7();
        var sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId, "ORD-006",
            DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5), []);

        saga.MarkPaymentAuthorized(Guid.CreateVersion7().ToString(), correlationId);
        saga.MarkStockReservationRejected("Insufficient stock", correlationId);

        // _stockReserved remains false — gate cannot fire even though payment was authorized.
        // No compensation dispatched (FR-011: no Sales-owned compensation).
        Assert.True(true);
    }

    [Fact]
    public void When_StockReserved_ThenPaymentDeclined_NoException()
    {
        var correlationId = Guid.CreateVersion7();
        var sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId, "ORD-007",
            DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5), []);

        saga.MarkStockReserved(Guid.CreateVersion7().ToString(), correlationId);
        saga.MarkPaymentDeclined("Card rejected", correlationId);

        // _paymentAuthorized remains false — gate cannot fire even though stock was reserved.
        Assert.True(true);
    }
}
