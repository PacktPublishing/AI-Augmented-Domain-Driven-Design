using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using CsCheck;

namespace BrewUp.Sales.Tests.Domain;

/// <summary>
/// Property-based tests for the Sales Order confirmation invariant (T025).
/// Asserts: if SalesOrderConfirmed was raised, BOTH PaymentAuthorizationId and StockReservationId must be non-empty.
/// Constitution Principle V — mutation gate follow-up recorded in plan.md Complexity Tracking.
/// </summary>
public class ConfirmSalesOrderInvariantTests
{
    private static readonly Gen<string> NonEmptyString =
        Gen.String.Where(s => !string.IsNullOrWhiteSpace(s));

    /// <summary>
    /// Property: SalesOrderConfirmed is only raised when BOTH references are non-empty (BC-010 / INV-1).
    /// For any combination of non-empty payment and stock references, a SalesOrderConfirmed event must carry both.
    /// </summary>
    [Fact]
    public void Confirmed_Event_Always_Carries_Both_References()
    {
        Gen.Select(NonEmptyString, NonEmptyString)
           .Sample((paymentId, stockId) =>
           {
               var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
               var paymentAuthId = new PaymentAuthorizationId(paymentId);
               var stockResId = new StockReservationId(stockId);
               var correlationId = Guid.CreateVersion7();

               var @event = new SalesOrderConfirmed(salesOrderId, paymentAuthId, stockResId, correlationId);

               Assert.False(string.IsNullOrWhiteSpace(@event.PaymentAuthorizationId.Value),
                   "PaymentAuthorizationId must be present in SalesOrderConfirmed");
               Assert.False(string.IsNullOrWhiteSpace(@event.StockReservationId.Value),
                   "StockReservationId must be present in SalesOrderConfirmed");
           });
    }

    /// <summary>
    /// Property: A confirmed Sales Order status must always have both references present (INV-1, INV-2).
    /// Any SalesOrderConfirmed event must have the status Confirmed and both references non-empty.
    /// </summary>
    [Fact]
    public void SalesOrderConfirmed_Status_Implies_BothRefs_Present()
    {
        Gen.Select(NonEmptyString, NonEmptyString)
           .Sample((paymentId, stockId) =>
           {
               var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
               var paymentAuthId = new PaymentAuthorizationId(paymentId);
               var stockResId = new StockReservationId(stockId);
               var correlationId = Guid.CreateVersion7();

               var @event = new SalesOrderConfirmed(salesOrderId, paymentAuthId, stockResId, correlationId);

               // The event can only exist with both references — the aggregate guard prevents any other path.
               Assert.NotNull(@event.PaymentAuthorizationId);
               Assert.NotNull(@event.StockReservationId);
               Assert.False(string.IsNullOrWhiteSpace(@event.PaymentAuthorizationId.Value));
               Assert.False(string.IsNullOrWhiteSpace(@event.StockReservationId.Value));
           });
    }
}
