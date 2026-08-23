using BrewUp.Sagas.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;

namespace BrewUp.Sagas.Tests.Orchestrators;

public sealed class SalesOrderSagaDecisionReferenceTests
{
    [Fact]
    public void Availability_Checked_Event_Carries_Payment_And_Stock_Reservation_References()
    {
        var sagaId = new IntegrationId(Guid.CreateVersion7().ToString());
        var paymentAuthorizationId = new PaymentAuthorizationId(Guid.CreateVersion7().ToString());
        var stockReservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var rows = new[]
        {
            new ItemRequested(
                new BeerId(Guid.CreateVersion7().ToString()),
                new Quantity(2, "Bottle"),
                new Quantity(5, "Bottle"))
        };

        var @event = new SagaSalesOrderAvailablityChecked(
            sagaId,
            Guid.CreateVersion7(),
            sagaId.Value,
            paymentAuthorizationId,
            stockReservationId,
            rows);

        Assert.Equal(paymentAuthorizationId.Value, @event.PaymentAuthorizationId.Value);
        Assert.Equal(stockReservationId.Value, @event.StockReservationId.Value);
    }
}
