using BrewUp.Sagas.SharedKernel.CustomTypes;
using BrewUp.Sagas.SharedKernel.Messages.Events;
using Muflone.Messages;

namespace BrewUp.Sagas.Tests.Orchestrators;

public class SalesOrderSagaTests
{
    [Fact]
    public void CorrelationId_MustBe_Preserved()
    {
        var correlationId = Guid.CreateVersion7();
        var warehouseId = Guid.CreateVersion7().ToString();
        var customerId = Guid.CreateVersion7().ToString();

        SalesOrderSagaStarted @event = new(new SagaId(correlationId.ToString()), correlationId,
            "1234567890",
            DateTime.UtcNow,
            customerId,
            warehouseId,
            DateTime.UtcNow.AddDays(5),
            []);
        
        var eventCorrelationId = MessageHelpers.GetCorrelationId(@event);
        
        Assert.Equal(correlationId, eventCorrelationId);
    }
}