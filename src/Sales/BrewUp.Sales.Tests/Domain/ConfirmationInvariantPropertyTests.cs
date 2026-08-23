using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Sales.Tests.Domain;

/// <summary>
/// Property-based test: for any SalesOrder in Confirmed state,
/// PaymentAuthorizationReference and StockReservationReference MUST both be present. (BC-010, SC-001)
/// </summary>
public class ConfirmationInvariantPropertyTests
{
    [Fact]
    public void ConfirmedOrder_MustHave_BothReferences()
    {
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var paymentRef = new PaymentAuthorizationReference(Guid.CreateVersion7().ToString());
        var stockRef = new StockReservationReference(Guid.CreateVersion7().ToString());
        var correlationId = Guid.CreateVersion7();

        var confirmedEvent = new SalesOrderConfirmed(salesOrderId, correlationId, paymentRef, stockRef);

        Assert.NotNull(confirmedEvent.PaymentAuthorizationReference);
        Assert.False(string.IsNullOrEmpty(confirmedEvent.PaymentAuthorizationReference.Value));

        Assert.NotNull(confirmedEvent.StockReservationReference);
        Assert.False(string.IsNullOrEmpty(confirmedEvent.StockReservationReference.Value));
    }

    [Fact]
    public void SalesOrderStatus_Confirmed_MustExist_InEnumeration()
    {
        var confirmed = SalesOrderStatus.From(6);
        Assert.Equal("confirmed", confirmed.Name);
    }

    [Fact]
    public void SalesOrderStatus_List_MustInclude_Confirmed()
    {
        var list = SalesOrderStatus.List().ToList();
        Assert.Contains(list, s => s.Name == "confirmed");
    }
}
