using BrewUp.Sales.Domain.CommandHandlers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Sales.Tests.Domain;

/// <summary>
/// Guards BC-010 / INV-1: a Sales Order must NOT become Confirmed when
/// either required evidence reference is missing.
/// Three independent cases (separate specs for clarity):
///   Case A — no PaymentAuthorizationId
///   Case B — no StockReservationId
///   Case C — neither reference present
/// Each expects an empty event stream (no SalesOrderConfirmed).
/// </summary>

// ── Case A: missing PaymentAuthorizationId ────────────────────────────────
public sealed class ConfirmRejectedWhenPaymentAuthorizationMissing : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2010");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly Customer _customer;
    private readonly Guid _correlationId = Guid.CreateVersion7();

    // Empty string represents an absent / invalid payment authorization reference.
    private readonly PaymentAuthorizationId _emptyPaymentAuthorizationId = new(string.Empty);
    private readonly StockReservationId _stockReservationId = new(Guid.CreateVersion7().ToString());

    public ConfirmRejectedWhenPaymentAuthorizationMissing()
    {
        var customerId = new CustomerId(Guid.CreateVersion7().ToString());
        _customer = new Customer(customerId, new CustomerName("Test Customer"), CustomerType.Bronze);
    }

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _emptyPaymentAuthorizationId, _stockReservationId, _correlationId);

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        // No SalesOrderConfirmed: payment authorization reference is missing.
        yield break;
    }
}

// ── Case B: missing StockReservationId ───────────────────────────────────
public sealed class ConfirmRejectedWhenStockReservationMissing : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2011");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly Customer _customer;
    private readonly Guid _correlationId = Guid.CreateVersion7();

    private readonly PaymentAuthorizationId _paymentAuthorizationId = new(Guid.CreateVersion7().ToString());
    // Empty string represents an absent / invalid stock reservation reference.
    private readonly StockReservationId _emptyStockReservationId = new(string.Empty);

    public ConfirmRejectedWhenStockReservationMissing()
    {
        var customerId = new CustomerId(Guid.CreateVersion7().ToString());
        _customer = new Customer(customerId, new CustomerName("Test Customer"), CustomerType.Bronze);
    }

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _paymentAuthorizationId, _emptyStockReservationId, _correlationId);

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        // No SalesOrderConfirmed: stock reservation reference is missing.
        yield break;
    }
}

// ── Case C: neither reference present ────────────────────────────────────
public sealed class ConfirmRejectedWhenBothReferencesMissing : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2012");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly Customer _customer;
    private readonly Guid _correlationId = Guid.CreateVersion7();

    private readonly PaymentAuthorizationId _emptyPaymentAuthorizationId = new(string.Empty);
    private readonly StockReservationId _emptyStockReservationId = new(string.Empty);

    public ConfirmRejectedWhenBothReferencesMissing()
    {
        var customerId = new CustomerId(Guid.CreateVersion7().ToString());
        _customer = new Customer(customerId, new CustomerName("Test Customer"), CustomerType.Bronze);
    }

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _emptyPaymentAuthorizationId, _emptyStockReservationId, _correlationId);

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        // No SalesOrderConfirmed: both references are missing.
        yield break;
    }
}
