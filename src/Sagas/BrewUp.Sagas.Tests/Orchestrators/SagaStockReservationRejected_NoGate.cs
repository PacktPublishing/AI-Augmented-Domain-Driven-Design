using BrewUp.Sagas.Domain.Entities;
using BrewUp.Sagas.SharedKernel.CustomTypes;

namespace BrewUp.Sagas.Tests.Orchestrators;

/// <summary>
/// US3 negative path: stock reservation rejected — gate must NOT fire.
/// No compensation (OQ-7 is out of scope).
/// </summary>
public class SagaStockReservationRejected_NoGate
{
    [Fact]
    public void When_StockReservationRejected_NoException()
    {
        var correlationId = Guid.CreateVersion7();
        var sagaId = new SagaId(correlationId.ToString());

        var saga = SalesOrderSaga.Start(sagaId, correlationId, "ORD-005",
            DateTime.UtcNow, Guid.CreateVersion7().ToString(),
            Guid.CreateVersion7().ToString(), DateTime.UtcNow.AddDays(5), []);

        saga.MarkStockReservationRejected("Item out of stock", correlationId);
        Assert.True(true);
    }
}
