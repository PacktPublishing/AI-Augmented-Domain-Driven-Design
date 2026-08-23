using BrewUp.Sales.Domain.Mappers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Core;
using Muflone.CustomTypes;

namespace BrewUp.Sales.Domain.Entities;

public class SalesOrder : AggregateRoot
{
    private SalesOrderNumber _salesOrderNumber = null!;
    private SalesOrderDate _salesOrderDate = null!;
    private Customer _customer = null!;
    private SalesOrderDeliveryDate _salesOrderDeliveryDate = null!;
    private List<SalesOrderRow> _rows = [];
    
    private SalesOrderStatus _salesOrderStatus;
    
    private PaymentAuthorizationReference? _paymentAuthorizationReference;
    private StockReservationReference? _stockReservationReference;
    
    protected SalesOrder()
    {}

    internal static SalesOrder Create(SalesOrderId aggregateId, SalesOrderNumber salesOrderNumber,
        SalesOrderDate salesOrderDate, Customer customer, SalesOrderDeliveryDate salesOrderDeliveryDate,
        IEnumerable<SalesOrderRowJson> rows, Guid correlationId)
    {
        return new SalesOrder(aggregateId, salesOrderNumber, salesOrderDate, customer, salesOrderDeliveryDate, rows,
            correlationId);
    }

    private SalesOrder(SalesOrderId aggregateId, SalesOrderNumber salesOrderNumber, SalesOrderDate salesOrderDate,
        Customer customer, SalesOrderDeliveryDate salesOrderDeliveryDate,
        IEnumerable<SalesOrderRowJson> rows, Guid correlationId)
    {
        // Business logic validations can be added here
        var rowArray = rows as SalesOrderRowJson[] ?? rows.ToArray();
        foreach (var row in rowArray)
        {
            if (row.Quantity.Value > 0) 
                continue;
            
            // You must raise an event in case of error.
            RaiseEvent(new QuantityErrorRaised(aggregateId, correlationId, new List<SalesOrderRowJson>
            {
                row
            }, "Quantity must be greater than zero."));
            return;
        }
        
        List<SalesOrderRowJson> rowsList = [];
        if (customer is not null && customer.CustomerType.Equals(CustomerType.Gold))
        {
            rowsList.AddRange(rowArray.Select(row => new SalesOrderRowJson
            {
                BeerId = row.BeerId, 
                BeerName = row.BeerName, 
                Quantity = row.Quantity, 
                Price = new Shared.CustomTypes.Price(row.Price.Value * 0.9m, row.Price.Currency) // Apply a 10% discount for Gold customers
            }));
        }
        
        rowsList = !rowsList.Any()
            ? rowArray.ToList()
            : rowsList;
            
        RaiseEvent(new SalesOrderCreated(aggregateId, salesOrderNumber, salesOrderDate, customer,
            salesOrderDeliveryDate, rowsList, correlationId));
    }

    private void Apply(SalesOrderCreated @event)
    {
        Id = @event.AggregateId;
        _salesOrderNumber = @event.SalesOrderNumber;
        _salesOrderDate = @event.SalesOrderDate;
        _customer = @event.Customer;
        _salesOrderDeliveryDate = @event.SalesOrderDeliveryDate;
        _rows = @event.Rows.Select(r => r.ToEntity()).ToList();
        
        _salesOrderStatus = SalesOrderStatus.Accepted;
    }
    
    private void Apply(QuantityErrorRaised @event)
    {
        // Handle the quantity error, e.g., log it or set a flag
        // For this example, we will just set the sales order status to Rejected
    }

    internal void AddBeers(IEnumerable<SalesOrderRowJson> rows)
    {
        if (Equals(_salesOrderStatus, SalesOrderStatus.Closed))
        {
            // Raise an Error Event!!!
            throw new InvalidOperationException("Cannot add beers to a closed sales order.");
        }
        
        IEnumerable<SalesOrderRowJson> orderRows = _rows.Select(r => r.ToJson()).ToList();
        orderRows = orderRows.Concat(rows).ToList();
        RaiseEvent(new BeersAddedToCart(new SalesOrderId(Id.Value), orderRows));
    }
    
    private void Apply(BeersAddedToCart @event)
    {
        _rows = @event.Rows.Select(r => r.ToEntity()).ToList();
    }
    
    internal void CloseSalesOrder(SalesOrderDeliveryDate salesOrderDeliveryDate, Account account, Guid correlationId)
    {
        if (Equals(_salesOrderStatus, SalesOrderStatus.Closed))
        {
            // Raise an Error Event!!!
            throw new InvalidOperationException("Cannot add beers to a closed sales order.");
        }
        
        RaiseEvent(new SalesOrderClosed(new SalesOrderId(Id.Value), salesOrderDeliveryDate, correlationId));
    }

    internal void AcceptOrder(Guid correlationId)
    {
        RaiseEvent(new SalesOrderAccepted(new SalesOrderId(Id.Value), correlationId));        
    }
    
    private void Apply(SalesOrderAccepted @event)
    {
        _salesOrderStatus = SalesOrderStatus.Accepted;
    }

    internal void ConfirmOrder(PaymentAuthorizationReference paymentAuthorizationReference,
        StockReservationReference stockReservationReference, Guid correlationId)
    {
        // Idempotency guard (FR-009): already confirmed → no-op
        if (Equals(_salesOrderStatus, SalesOrderStatus.Confirmed))
            return;

        // Invariant (BC-010): both external decision references must be present
        if (string.IsNullOrEmpty(paymentAuthorizationReference?.Value) ||
            string.IsNullOrEmpty(stockReservationReference?.Value))
            return;

        RaiseEvent(new SalesOrderConfirmed(new SalesOrderId(Id.Value), correlationId,
            paymentAuthorizationReference, stockReservationReference));
    }

    private void Apply(SalesOrderConfirmed @event)
    {
        _salesOrderStatus = SalesOrderStatus.Confirmed;
        _paymentAuthorizationReference = @event.PaymentAuthorizationReference;
        _stockReservationReference = @event.StockReservationReference;
    }

    private void Apply(SalesOrderClosed @event)
    {
        _salesOrderDeliveryDate = @event.SalesOrderDeliveryDate;
    }
}